using System;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents a match capture group.
    /// </summary>
    public class Group : Capture
    {
        /// <summary>
        /// Get whether the group successfully matched or not.
        /// </summary>
        public bool Success => Index >= 0 && Length > 0;

        public Group(Tre.regmatch_t match, string sourceString) : base(match, sourceString) { }
    }
}
