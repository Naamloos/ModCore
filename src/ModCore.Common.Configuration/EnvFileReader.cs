using System.Text;
using System.Text.RegularExpressions;

namespace ModCore.Common.Configuration
{
    internal static partial class EnvFileReader
    {
        public static IEnumerable<KeyValuePair<string, string>> Load(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var values = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            while (reader.Peek() >= 0)
            {
                string? rawLine = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(rawLine))
                    continue;

                string line = rawLine.Trim();

                if (line.StartsWith("#"))
                    continue;

                var separatorIndex = FindUnquotedEquals(line);

                if (separatorIndex <= 0)
                    continue;

                string key = line[..separatorIndex].Trim();
                string rawValue = line[(separatorIndex + 1)..].Trim();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                string value = ParseValue(rawValue);
                value = ExpandVariables(value, values);

                values[key] = value;

                yield return new KeyValuePair<string, string>(key, value);
            }
        }

        private static int FindUnquotedEquals(string line)
        {
            bool inSingleQuotes = false;
            bool inDoubleQuotes = false;
            bool escaped = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inDoubleQuotes)
                {
                    escaped = true;
                    continue;
                }

                if (c == '\'' && !inDoubleQuotes)
                {
                    inSingleQuotes = !inSingleQuotes;
                    continue;
                }

                if (c == '"' && !inSingleQuotes)
                {
                    inDoubleQuotes = !inDoubleQuotes;
                    continue;
                }

                if (c == '=' && !inSingleQuotes && !inDoubleQuotes)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string ParseValue(string rawValue)
        {
            rawValue = StripInlineComment(rawValue).Trim();

            if (rawValue.Length >= 2 &&
                rawValue[0] == '"' &&
                rawValue[^1] == '"')
            {
                return UnescapeDoubleQuotedValue(
                    rawValue[1..^1]);
            }

            if (rawValue.Length >= 2 &&
                rawValue[0] == '\'' &&
                rawValue[^1] == '\'')
            {
                return rawValue[1..^1];
            }

            return rawValue;
        }

        private static string StripInlineComment(string value)
        {
            bool inSingleQuotes = false;
            bool inDoubleQuotes = false;
            bool escaped = false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inDoubleQuotes)
                {
                    escaped = true;
                    continue;
                }

                if (c == '\'' && !inDoubleQuotes)
                {
                    inSingleQuotes = !inSingleQuotes;
                    continue;
                }

                if (c == '"' && !inSingleQuotes)
                {
                    inDoubleQuotes = !inDoubleQuotes;
                    continue;
                }

                if (c == '#' && !inSingleQuotes && !inDoubleQuotes)
                {
                    if (i == 0 || char.IsWhiteSpace(value[i - 1]))
                    {
                        return value[..i];
                    }
                }
            }

            return value;
        }

        private static string UnescapeDoubleQuotedValue(string value)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (c != '\\' || i == value.Length - 1)
                {
                    builder.Append(c);
                    continue;
                }

                char next = value[++i];

                builder.Append(next switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    '"' => '"',
                    '$' => '$',
                    _ => next
                });
            }

            return builder.ToString();
        }

        private static string ExpandVariables(
            string value,
            IReadOnlyDictionary<string, string> values)
        {
            return VariableRegex().Replace(value, match =>
            {
                string key = match.Groups["key"].Value;

                if (values.TryGetValue(key, out string? existingValue))
                    return existingValue;

                return Environment.GetEnvironmentVariable(key) ?? string.Empty;
            });
        }

        [GeneratedRegex(@"\$\{(?<key>[A-Za-z_][A-Za-z0-9_]*)\}")]
        private static partial Regex VariableRegex();
    }
}