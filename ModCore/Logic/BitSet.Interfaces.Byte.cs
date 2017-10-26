using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ModCore.Logic
{
    public partial class BitSet : IList<byte>, IReadOnlyList<byte>
    {   
        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return Sequence.GetEnumerator();
        }

        public IEnumerator<byte> GetByteEnumerator()
        {
            return Sequence.GetEnumerator();
        }

        public void Add(byte item) => throw new NotSupportedException();

        void ICollection<byte>.Clear() => ClearAll();

        public bool Contains(byte item) => Bits.Contains(item);

        public void CopyTo(byte[] array, int arrayIndex) => Bits.CopyTo(array, arrayIndex);

        public bool Remove(byte item) => throw new NotSupportedException();

        int ICollection<byte>.Count => Bits.Length;

        bool ICollection<byte>.IsReadOnly => true;

        public int IndexOf(byte item) => Array.IndexOf(Bits, item);

        public void Insert(int index, byte item) => throw new NotSupportedException();

        void IList<byte>.RemoveAt(int index) => throw new NotSupportedException();

        byte IList<byte>.this[int index]
        {
            get => Bits[index];
            set => Bits[index] = value;
        }

        int IReadOnlyCollection<byte>.Count => Bits.Length;

        byte IReadOnlyList<byte>.this[int index] => Bits[index];

    }
}