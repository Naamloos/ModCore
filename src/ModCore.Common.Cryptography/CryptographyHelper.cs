using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Cryptography
{
    public class CryptoEnvelope
    {
        [JsonPropertyName("t")]
        public CryptographyType Type { get; set; }

        [JsonPropertyName("d")]
        public string Data { get; set; } = "";
    }

    public enum CryptographyType
    {
        None = 0,
        AESGCM_HKDF = 1
    }

    public static class CryptographyHelper
    {
        /// <summary>
        /// Returns an encrypted string wrapped in a structured envelope that includes metadata about the encryption type.
        /// </summary>
        /// <param name="plainText">The plaintext input</param>
        /// <param name="contextId">Context ID for the input</param>
        /// <param name="base64Key">Encryption master key</param>
        public static string Encrypt(string plainText, string contextId, string base64Key)
        {
            // e.g. in debug environments someone might not want to set up decryption keys.
            // in that case, we will fallback to returning JSON-wrapped plaintext with an explicit "None" type to maintain structural consistency in the database.
            if (string.IsNullOrEmpty(base64Key))
            {
                return JsonSerializer.Serialize(new CryptoEnvelope
                {
                    Type = CryptographyType.None,
                    Data = plainText
                });
            }
            
            return EncryptAESGCM_HKDF(plainText, contextId, base64Key);
        }

        private static string EncryptAESGCM_HKDF(string plainText, string contextId, string base64Key)
        {
            if (plainText == null || string.IsNullOrEmpty(base64Key)) return plainText!;

            byte[] masterKey = Convert.FromBase64String(base64Key);
            if (masterKey.Length != 32)
            {
                throw new InvalidOperationException("Encryption master key must be 32 bytes.");
            }
            byte[] salt = Encoding.UTF8.GetBytes("modcore:aesgcm-hkdf");
            byte[] info = Encoding.UTF8.GetBytes($"context:{contextId}");
            byte[] derivedKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, salt, info);

            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];
            byte[] aad = Encoding.UTF8.GetBytes($"modcore:{contextId}");

            using var aesGcm = new AesGcm(derivedKey, tag.Length);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag, aad);

            byte[] result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

            // Pack it into a rigid structural envelope
            var envelope = new CryptoEnvelope
            {
                Type = CryptographyType.AESGCM_HKDF,
                Data = Convert.ToBase64String(result)
            };

            return JsonSerializer.Serialize(envelope);
        }

        /// <summary>
        /// Returns the decrypted plaintext if the input is a valid encrypted envelope, or returns the original string if it's not in the expected format (e.g., legacy plaintext). 
        /// Throws an exception if tampering is detected or if decryption fails due to invalid context/key.
        /// </summary>
        /// <param name="databaseValue">Input encrypted envelope string (JSON)</param>
        /// <param name="contextId">Context ID used for encryption</param>
        /// <param name="base64Key">Base64 Master Key</param>
        /// <exception cref="InvalidOperationException">Data tampering was detected</exception>
        public static string Decrypt(string databaseValue, string contextId, string base64Key)
        {
            if (string.IsNullOrEmpty(databaseValue) || string.IsNullOrEmpty(base64Key)) return databaseValue;

            CryptoEnvelope envelope;
            try
            {
                // Try to parse the database string as our structured envelope
                envelope = JsonSerializer.Deserialize<CryptoEnvelope>(databaseValue)!;

                // If it parsed but doesn't look like our envelope, treat it as legacy plain text
                if (envelope == null || string.IsNullOrEmpty(envelope.Data))
                {
                    return databaseValue;
                }
            }
            catch (JsonException)
            {
                // Parsing failed! This means the row contains raw legacy text (e.g., "Call mom")
                return databaseValue;
            }

            // If it parsed but doesn't look like our structural envelope, treat it as legacy plain text
            if (envelope == null || string.IsNullOrEmpty(envelope.Data))
            {
                return databaseValue;
            }

            // Validate the Enum explicitly. If an arbitrary JSON string happened to map a random integer
            // to the Type property that doesn't match our defined enum suite, treat it as legacy text.
            if (!Enum.IsDefined(typeof(CryptographyType), envelope.Type))
            {
                return databaseValue;
            }

            if (envelope.Type == CryptographyType.None)
            {
                // Explicitly marked as no encryption, return as-is
                return envelope.Data;
            }

            // Proceed with normal authenticated decryption using envelope.d
            byte[] rawData = Convert.FromBase64String(envelope.Data);

            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] cipherBytes = new byte[rawData.Length - 28];
            byte[] aad = Encoding.UTF8.GetBytes($"modcore:{contextId}");

            Buffer.BlockCopy(rawData, 0, nonce, 0, 12);
            Buffer.BlockCopy(rawData, 12, tag, 0, 16);
            Buffer.BlockCopy(rawData, 28, cipherBytes, 0, cipherBytes.Length);

            byte[] masterKey = Convert.FromBase64String(base64Key);
            if (masterKey.Length != 32)
            {
                throw new InvalidOperationException("Encryption master key must be 32 bytes.");
            }
            byte[] salt = Encoding.UTF8.GetBytes("modcore:aesgcm-hkdf");
            byte[] info = Encoding.UTF8.GetBytes($"context:{contextId}");
            byte[] derivedKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, salt, info);

            using var aesGcm = new AesGcm(derivedKey, tag.Length);
            byte[] plainBytes = new byte[cipherBytes.Length];

            try
            {
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes, aad);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (CryptographicException)
            {
                // If the JSON structure was valid but the AES-GCM signature fails,
                // someone explicitly forged a bad payload or the key context is wrong.
                throw new InvalidOperationException("Data tampering detected or invalid cryptographic context mapping.");
            }
        }
    }
}