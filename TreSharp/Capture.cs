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
        public int Index {
            get;
            protected set;
        }

        /// <summary>
        /// Get the number of characters that matched.
        /// </summary>
        public int Length {
            get;
            protected set;
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
                    _value = SourceString.Substring(_match.rm_so, _match.rm_eo - _match.rm_so);
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
            protected set;
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
