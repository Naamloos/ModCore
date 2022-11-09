using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModCore.Utils
{
    /// <summary>
    /// String tokenizer by uwx
    /// </summary>
    public class StringTokenizer : IEnumerable<string>, IEnumerator<string>
    {
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public string Current => _current;
        
        // ReSharper disable ConvertToAutoPropertyWhenPossible
        internal string String => _string;
        internal int Index => _index;
        // ReSharper enable ConvertToAutoPropertyWhenPossible

        private string _current;
        private readonly string _string;
        private int _index;

        private static readonly char[] WhiteSpaceChars = Enumerable.Range(char.MinValue, char.MaxValue)
            .Select(e => (char)e)
            .Where(c => c == 32 || c >= 9 && c <= 13 || (c == 160 || c == 133))
            .ToArray();

        public StringTokenizer(string from)
        {
            _string = from;
            _current = null;
            _index = 0;
        }

        public IEnumerator<string> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_index == -1) return false;

            while (char.IsWhiteSpace(_string[_index]))
            {
                _index++;
                if (_index >= _string.Length) return false;
            }
            int newIndex;
            // support quoted text
            if (_string[_index] == '"')
            {
                _index++;//remove initial quote
                newIndex = _string.IndexOf('"', _index+1);
            }
            else
            {
                newIndex = _string.IndexOfAny(WhiteSpaceChars, _index);
            }
            _current = _string.Substring(_index, (newIndex == -1 ? _string.Length : newIndex) - _index);//substring(startIndex,length)
            // remove closing quote
            if (newIndex != -1 && newIndex < _string.Length && _string[newIndex] == '"')
            {
                _index = newIndex + 1;
            }
            else
            {
                _index = newIndex;
            }
            return true;
        }

        /// <summary>
        /// Assigns the next element in the tokenizer to a string
        /// </summary>
        /// <param name="str">the string to assign</param>
        /// <returns>true if successful</returns>
        public bool Next(out string str)
        {
            var b = MoveNext();
            str = _current;
            return b;
        }

        public string Remaining()
        {
            var tokens = new StringBuilder();
            foreach (var token in this)
            {
                tokens.Append(' ').Append(token);
            }

            return tokens.Length == 0 ? "" : tokens.ToString().Substring(1); // remove first space
        }

        public StringTokenizer Clone()
        {
            return _index == -1
                ? new StringTokenizer(null)
                {
                    _index = -1
                }
                : new StringTokenizer(_string.Substring(_index));
        }

        public void Reset()
        {
            _index = 0;
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // empty
        }
    }
}