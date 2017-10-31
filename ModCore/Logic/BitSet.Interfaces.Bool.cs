using System;
using System.Collections;
using System.Collections.Generic;

namespace ModCore.Logic
{
    public partial class BitSet : IList<bool>, IReadOnlyList<bool>
    {
        public void Add(bool item) => throw new NotSupportedException();

        void ICollection<bool>.Clear() => ClearAll();

        public bool Contains(bool item)
        {
            var l = Bits.Length * 8;
            for (var i = 0; i < l; i++)
            {
                if (IsSet(i) == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(bool[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var b in this as IEnumerable<bool>)
            {
                array[i++] = b;
            }
        }

        public bool Remove(bool item) => throw new NotSupportedException();

        int ICollection<bool>.Count => Bits.Length * 8;

        bool ICollection<bool>.IsReadOnly => true;

        public int IndexOf(bool item)
        {
            var l = Bits.Length * 8;
            for (var i = 0; i < l; i++)
            {
                if (IsSet(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, bool item) => throw new NotSupportedException();

        void IList<bool>.RemoveAt(int index) => throw new NotSupportedException();

        bool IList<bool>.this[int index]
        {
            get => IsSet(index);
            set
            {
                if (value)
                    Set(index);
                else
                    Clear(index);
            }
        }

        int IReadOnlyCollection<bool>.Count => Bits.Length * 4;

        bool IReadOnlyList<bool>.this[int index] => IsSet(index);
    }
}