using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents a compiled TRE regular expression that can
    /// be used for matching.
    /// </summary>
    public class Regex : IDisposable
    {
        /// <summary>
        /// Get the regular expression pattern.
        /// </summary>
        public string Pattern { get; }
        
        /// <summary>
        /// Get the pattern options that this regular expression was
        /// created with.
        /// </summary>
        public TrePatternOptions Options { get; }

        /// <summary>
        /// Get the underlying Tre regular expression object.
        /// </summary>
        private Tre.regex_t TreRegex { get; set; }

        /// <summary>
        /// Create a regular expression with the specified pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public Regex(string pattern) : this(pattern, TrePatternOptions.REG_EXTENDED)
        {
        }

        /// <summary>
        /// Create a regular expression with the specified pattern and options.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        public Regex(string pattern, TrePatternOptions options)
        {
            Pattern = pattern;
            Options = options;

            Tre.regex_t rgx = new Tre.regex_t();
            
            TreErrorCode rc = Tre.regcomp(ref rgx, Pattern, Options);
            Tre.CheckError(rc, rgx);

            TreRegex = rgx;
        }

        /// <summary>
        /// Returns true if this regular expression matches the specified input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsMatch(string input)
        {
            return Match(input).Success;
        }

        /// <summary>
        /// Returns true if this regular expression matches the specified input,
        /// using the specified options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool IsMatch(string input, TreOptions options)
        {
            return Match(input, options).Success;
        }

        /// <summary>
        /// Returns the match results of this regular expression for the 
        /// specified input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Match Match(string input)
        {
            return Match(input, new TreOptions());
        }

        /// <summary>
        /// Returns the match results of this regular expression for the 
        /// specified input and options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Match Match(string input, TreOptions options)
        { 
            if (TreRegex.IsDisposed)
            {
                throw new ObjectDisposedException("Preg", "TRE regular expression has been disposed.");
            }

            Tre.regex_t rgx = TreRegex;

            Tre.regamatch_t match = new Tre.regamatch_t()
            {
                nmatch = rgx.re_nsub + 1
            };

            Tre.regmatch_t[] pmatch = new Tre.regmatch_t[rgx.re_nsub + 1];
            GCHandle pinnedArray = GCHandle.Alloc(pmatch, GCHandleType.Pinned);
            
            TreErrorCode rc;
            try
            {
                IntPtr ptr = pinnedArray.AddrOfPinnedObject();
                match.pmatch = ptr;

                Tre.regaparams_t parameters = options.CreateParameters();
                rc = Tre.regaexec(
                    ref rgx,
                    input,
                    ref match,
                    parameters,
                    options.SearchOptions
                );
                Tre.CheckError(rc, rgx);
            }
            finally
            {
                pinnedArray.Free();
            }
            return new Match(match, pmatch, input);
        }

        /// <summary>
        /// Free resources used by this regex.
        /// </summary>
        public void Dispose()
        {
            Tre.regex_t tmp = TreRegex;
            tmp.Dispose();
            TreRegex = tmp;
        }
    }
}
