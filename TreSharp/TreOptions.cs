using System;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents options that can be set for TRE matching.
    /// </summary>
    public class TreOptions
    {
        public TRESearchOptions SearchOptions { get; set; } = TRESearchOptions.None;

        /// <summary>
        /// Get or set the maximum allowed cost of the match. If this is set to zero, an exact matching is searched for, and results equivalent to those returned by the regexec() functions are returned.
        /// </summary>
        public int MaxCost { get; set; } = 0;

        /// <summary>
        /// Get or set the cost of inserted characters.
        /// </summary>
        public int CostOfInsertions { get; set; } = 1;
        /// <summary>
        /// Get or set the cost of deleted characters.
        /// </summary>
        public int CostOfDeletions { get; set; } = 1;
        /// <summary>
        /// Get or set the cost of substituted characters.
        /// </summary>
        public int CostOfSubstitutions { get; set; } = 1;

        /// <summary>
        /// Get or set the maximum allowed number of errors (inserts + deletes + substitutes).
        /// </summary>
        public int MaxErrors { get; set; } = int.MaxValue;
        /// <summary>
        /// Get or set the maximum allowed number of inserts.
        /// </summary>
        public int MaxInsertions { get; set; } = int.MaxValue;
        /// <summary>
        /// Get or set the maximum allowed number of substitutions.
        /// </summary>
        public int MaxSubstitutions { get; set; } = int.MaxValue;
        /// <summary>
        /// Get or set the maximum allowed number of deletions.
        /// </summary>
        public int MaxDeletions { get; set; } = int.MaxValue;

        /// <summary>
        /// Create approximation parameters that can be used by the TRE library.
        /// </summary>
        /// <returns></returns>
        public Tre.regaparams_t CreateParameters()
        {
            Tre.regaparams_t parameters = new Tre.regaparams_t();
            Tre.regaparams_default(ref parameters);
            parameters.max_cost = MaxCost;
            parameters.cost_del = CostOfDeletions;
            parameters.cost_ins = CostOfInsertions;
            parameters.cost_subst = CostOfSubstitutions;
            parameters.max_del = MaxDeletions;
            parameters.max_err = MaxErrors;
            parameters.max_ins = MaxInsertions;
            parameters.max_subst = MaxSubstitutions;
            return parameters;
        }
    }
}
