using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents a regular expression match.
    /// </summary>
    public class Match : Capture
    {
        /// <summary>
        /// Get whether this match was a success.
        /// </summary>
        public bool Success => Index >= 0 && Length > 0;

        /// <summary>
        /// Get whether this match is approximate or not.
        /// </summary>
        public bool IsApproximate => Cost != 0;

        /// <summary>
        /// Get a collection of match groups.
        /// </summary>
        public GroupCollection Groups { get; }

        /// <summary>
        /// Get the total approximation cost of this match.
        /// </summary>
        public int Cost { get; }

        /// <summary>
        /// Get the number of character deletions that were made to make this match.
        /// </summary>
        public int NumberOfDeletions { get; }

        /// <summary>
        /// Get the number of character insertions that were made to make this match.
        /// </summary>
        public int NumberOfInsertions { get; }
        
        /// <summary>
        /// Get the number of character substitutions that were made to make this match.
        /// </summary>
        public int NumberOfSubstitutions { get; }

        public Match(Tre.regamatch_t match, Tre.regmatch_t[] pmatch, string sourceString) : base()
        {
            Cost = match.cost;
            NumberOfDeletions = match.num_del;
            NumberOfInsertions = match.num_ins;
            NumberOfSubstitutions = match.num_subst;

            if (pmatch != null && pmatch.Length > 0)
            {
                Update(pmatch[0], sourceString);

                Groups = new GroupCollection(pmatch.Length);
                foreach (Tre.regmatch_t m in pmatch)
                {
                    Groups.Add(new Group(m, sourceString));
                }
            }
        }
    }
}
