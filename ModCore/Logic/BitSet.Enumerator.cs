using System.Collections;
using System.Collections.Generic;

namespace ModCore.Logic
{
    public partial class BitSet
    {
        IEnumerator IEnumerable.GetEnumerator() => GetBoolEnumerator();

        public IEnumerator<bool> GetEnumerator() => GetBoolEnumerator();

        IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => GetBoolEnumerator();
        
        public IEnumerator<bool> GetBoolEnumerator()
        {
            for (var i = 0; i < Bits.Length * 8; i++)
            {
                yield return IsSet(i);
            }
        }
        
        IEnumerator<byte> IEnumerable<byte>.GetEnumerator() => Sequence.GetEnumerator();

        public IEnumerator<byte> GetByteEnumerator() => Sequence.GetEnumerator();
    }
}