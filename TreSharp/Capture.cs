using System;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents a string value captured from another source string.
    /// </summary>
    public class Capture
    {
        Tre.regmatch_t _match;
        string _value;

        /// <summary>
        /// Get the index where the match begins.
        /// </summary>
        public int Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the number of characters that matched.
        /// </summary>
        public int Length
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the value of the match.
        /// </summary>
        public string Value
        {
            get
            {
                if (_value == null)
                {
                    if (Length > 0)
                    {
                        _value = SourceString.Substring(Index, Length);
                    }
                    else
                    {
                        _value = string.Empty;
                    }
                }
                return _value;
            }
        }

        /// <summary>
        /// Get the string that was matched against.
        /// </summary>
        public string SourceString
        {
            get;
            internal set;
        }

        protected Capture() { }

        protected Capture(Tre.regmatch_t match, string sourceString)
        {
            Update(match, sourceString);
        }

        protected void Update(Tre.regmatch_t match, string sourceString)
        {
            _match = match;
            Index = _match.rm_so;
            Length = _match.rm_eo - _match.rm_so;
            SourceString = sourceString;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
