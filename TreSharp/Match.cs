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
        /// Get the regular expression pattern that was used to
        /// find this match.
        /// </summary>
        public Regex Pattern { get; private set; }

        /// <summary>
        /// Get the options that were used to find this match.
        /// </summary>
        public TreOptions OptionsUsed { get; private set; }

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

        public Match(Regex regex, TreOptions options, Tre.regamatch_t match, Tre.regmatch_t[] pmatch, string sourceString) : base()
        {
            Pattern = regex;
            OptionsUsed = options;

            Cost = match.cost == int.MaxValue ? 0 : match.cost;
            NumberOfDeletions = match.num_del == int.MaxValue ? 0 : match.num_del;
            NumberOfInsertions = match.num_ins == int.MaxValue ? 0 : match.num_ins;
            NumberOfSubstitutions = match.num_subst == int.MaxValue ? 0: match.num_subst;

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

        /// <summary>
        /// Returns a new match starting at the end of this match.
        /// </summary>
        /// <returns></returns>
        public Match NextMatch()
        {
            return NextMatch(true);
        }

        /// <summary>
        /// Returns a new match starting at the end of this match, or if
        /// startAfterEntireCurrentMatch is false, starting immediately at the
        /// current match Index + 1.
        /// </summary>
        /// <param name="startAfterEntireCurrentMatch"></param>
        /// <returns></returns>
        public Match NextMatch(bool startAfterEntireCurrentMatch)
        { 
            Match next = null;
            if (Success)
            {
                int start = Index + (startAfterEntireCurrentMatch ? Length : 1);
                string nextInput = SourceString.Substring(start);
                next = Pattern.Match(nextInput, OptionsUsed);
                if (next != null && next.Success)
                {
                    // Update indices and source string so that it is apparent that
                    // the next match and its groups came from the larger, whole source 
                    // string, and not just the substring created above.
                    next.Index = start + next.Index;
                    next.SourceString = SourceString;
                    foreach (Group matchGroup in next.Groups)
                    {
                        matchGroup.Index = start + matchGroup.Index;
                        matchGroup.SourceString = SourceString;
                    }
                }
            }
            if (next == null)
            {
                next = new Match(Pattern, OptionsUsed, default(Tre.regamatch_t), null, SourceString);
            }
            return next;
        }
    }
}
