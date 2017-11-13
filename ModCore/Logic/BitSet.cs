using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ModCore.Logic
{
    /// <inheritdoc cref="ICloneable" />
    /// <inheritdoc cref="ISerializable" />
    /// <summary>Represents a set of boolean values stored efficiently as array of bytes.</summary>
    [Serializable]
    public partial class BitSet : ICloneable, ISerializable
    {
//        public const byte True = 0b11111111;
//        public const byte False = 0b00000000;

        /// <summary>
        /// The actual bits.
        /// @serial the i'th bit is in bits[i/64] at position i%64 (where position
        /// 0 is the least significant).
        /// </summary>
        internal byte[] Bits;

        /// <inheritdoc />
        /// <summary>
        /// Create a new empty bit set of size 64. All bits are initially false.
        /// </summary>
        public BitSet()
            : this(64)
        {
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public ReadOnlyCollection<byte> Sequence => Array.AsReadOnly(Bits);

        public BitSet(int initialSize)
        {
            Bits = initialSize % 8 != 0 ? new byte[initialSize / 8 + 1] : new byte[initialSize / 8];
        }

        public BitSet(BitSet from)
        {
            Bits = from.Bits.Clone() as byte[];
        }

        public BitSet(IEnumerable<byte> from)
        {
            Bits = from.ToArray();
        }

        public void Set(int bit)
        {
            var b = bit / 8;
            if (b >= Bits.Length)
            {
                Resize(b + 1);
            }
            var bitIndex = bit % 8;
            Bits[b] |= (byte) (1 << bitIndex);
        }

        public void Clear(int bit)
        {
            var b = bit / 8;
            if (b >= Bits.Length)
            {
                Resize(b + 1);
                return; // do nothing, will already be cleared
            }

            var bitIndex = bit % 8;
            Bits[b] &= (byte) ~(1 << bitIndex);
        }

        private void Resize(int size)
        {
#if EMZI
            var result = new byte[size];
            Array.Copy(Bits, result, Bits.Length);
            Bits = result;
#else
            Array.Resize(ref Bits, size);
#endif
        }

        public bool IsSet(int bit)
        {
            var b = bit / 8;
            if (b >= Bits.Length)
                return false;

            var bitIndex = bit % 8;

            var mask = (byte) (1L << bitIndex);
            return (Bits[b] & mask) != 0;
        }

        public void ClearAll()
        {
            for (var i = 0; i < Bits.Length; i++)
            {
                Bits[i] = 0;
            }
        }

        public object Clone()
        {
            return new BitSet(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return new string(Bits.SelectMany(e => Convert.ToString(e, 2).PadLeft(8, '0')).ToArray());
        }
        
#if DEBUG
        public string SerializeToString()
        {
            var len = Bits.Length;
            var b = len % 8 != 0;
            var i1 = b ? len / 8 + 1 : len / 8;
            var longs = new long[i1];
            for (var i = 0; i < i1; i += 1)
            {
                longs[i] = BitConverter.ToInt64(Bits, i * 8);
            }
            return string.Join(',', Bits.Select(e => e == 0 ? "" : Convert.ToString(e)));
        }

        public static BitSet FromSerializedString(string str)
        {
            return new BitSet(str.Trim().Split(',').Select(e => e.Length == 0 ? (byte) 0 : byte.Parse(e)));
        }

        public string SerializeToString2()
        {
            return string.Join(',', Bits.Select(e => e == 0 ? "" : Convert.ToString(e)));
        }

        public static BitSet FromSerializedString2(string str)
        {
            return new BitSet(str.Trim().Split(',').Select(e => e.Length == 0 ? (byte) 0 : byte.Parse(e)));
        }
#endif
    }
}