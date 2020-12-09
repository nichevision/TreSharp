using System;
using System.Runtime.InteropServices;
using System.Text;

using size_t = System.UIntPtr;

namespace TreSharp
{
    /// <summary>
    /// Provides bindings to the native TRE library.
    /// </summary>
    public static class Tre
    {
#if x86
        public const string TreLibrary = "lib/win/x86/tre.dll";
#elif x64
        public const string TreLibrary = "lib/win/x64/tre.dll";
#else
    #error x86 or x64 must be specified as an msbuild property
#endif

        /// <summary>
        /// Check the specified error code, raised by use of the specified regex, and raise
        /// an exception if necessary.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="rgx"></param>
        public static void CheckError(TreErrorCode code, regex_t rgx)
        {
            switch (code)
            {
                case TreErrorCode.REG_OK:
                case TreErrorCode.REG_NOMATCH:
                    break;
                default:
                    throw TreException.FromCode(code, rgx);
            }
        }

        /// <summary>
        /// Get the version of TRE being used.
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var ptr = tre_version();
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Represents a compiled TRE regular expression.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct regex_t : IDisposable
        {
            /// <summary>
            /// Number of parenthesized subexpressions in regex.
            /// </summary>
            public uint re_nsub;
            /// <summary>
            /// For internal use only.
            /// </summary>
            public UIntPtr value;

            /// <summary>
            /// Get whether this regex has been disposed.
            /// </summary>
            public bool IsDisposed
            {
                get;
                private set;
            }

            /// <summary>
            /// Free resources used by this regex.
            /// </summary>
            public void Dispose()
            {
                if (!IsDisposed)
                {
                    regfree(ref this);
                    IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// Results of a single group of a match.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct regmatch_t
        {
            /// <summary>
            /// Offset from start of string to start of substring.
            /// </summary>
            public int rm_so;
            /// <summary>
            /// Offset from start of string to the first character after the substring.
            /// </summary>
            public int rm_eo;
        }

        /// <summary>
        /// Approximate matching results.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct regamatch_t
        {
            /// <summary>
            /// Length of pmatch array.
            /// </summary>
            public uint nmatch;
            /// <summary>
            /// Submatch data.
            /// </summary>
            public IntPtr pmatch;
            /// <summary>
            /// Cost of the approximate match.
            /// </summary>
            public int cost;
            /// <summary>
            /// Number of inserts in the match.
            /// </summary>
            public int num_ins;
            /// <summary>
            /// Number of deletions in the match.
            /// </summary>
            public int num_del;
            /// <summary>
            /// Number of substitutions in the match.
            /// </summary>
            public int num_subst;
        }

        /// <summary>
        /// Approximate matching parameters.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct regaparams_t
        {
            /// <summary>
            /// Cost of an inserted character.
            /// </summary>
            public int cost_ins;
            /// <summary>
            /// Cost of a deleted character.
            /// </summary>
            public int cost_del;
            /// <summary>
            /// Cost of a substituted character.
            /// </summary>
            public int cost_subst;
            /// <summary>
            /// Maximum allowed cost of a match.
            /// </summary>
            public int max_cost;

            /// <summary>
            /// Maximum allowed number of inserts.
            /// </summary>
            public int max_ins;
            /// <summary>
            /// Maximum allowed number of deletes.
            /// </summary>
            public int max_del;
            /// <summary>
            /// Maximum allowed number of substitutions.
            /// </summary>
            public int max_subst;
            /// <summary>
            /// Maximum allowed number of total errors.
            /// </summary>
            public int max_err;
        }

        /// <summary>
        /// Returns a pointer to the version string.
        /// </summary>
        /// <returns></returns>
        [DllImport(TreLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr tre_version();

        /// <summary>
        /// Compile the specified regular expresion pattern and options and stores the result in the
        /// referenced regex_t.
        /// </summary>
        /// <param name="preg"></param>
        /// <param name="regex"></param>
        /// <param name="cflags"></param>
        /// <returns></returns>
        [DllImport(TreLibrary, EntryPoint = "tre_regcomp", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern TreErrorCode regcomp(
            ref regex_t preg,
            string regex,
            TrePatternOptions cflags
        );

        /// <summary>
        /// Turn the specified error code into a human-readable error message. If called with
        /// a null errorBuffer and errbuf_size of 0, this function returns the size required
        /// for errorBuffer to contain the null-terminated error message string.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="preg"></param>
        /// <param name="errorBuffer"></param>
        /// <param name="errbuf_size"></param>
        /// <returns></returns>
        [DllImport(TreLibrary, EntryPoint = "tre_regerror", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern UIntPtr regerror(
            TreErrorCode code,
            ref regex_t preg,
            StringBuilder errorBuffer,
            size_t errbuf_size
        );

        /// <summary>
        /// Free memory used by the specified compiled regex.
        /// </summary>
        /// <param name="preg"></param>
        [DllImport(TreLibrary, EntryPoint = "tre_regfree", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void regfree(
            ref regex_t preg
        );

        /// <summary>
        /// Sets approximate match parameters to default.
        /// </summary>
        /// <param name="parameters"></param>
        [DllImport(TreLibrary, EntryPoint = "tre_regaparams_default", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void regaparams_default(
            ref regaparams_t parameters
        );

        /// <summary>
        /// Match a null-terminated string against the specified, pre-compiled pattern buffer preg.
        /// nmatch and pmatch are used to provide information regarding the location of any matches. 
        /// </summary>
        /// <param name="preg">The pre-compiled regular expression pattern buffer.</param>
        /// <param name="input">The null-terminated input string to match against.</param>
        /// <param name="nmatch">The size of the pmatch array.</param>
        /// <param name="pmatch">A buffer to store match subdata.</param>
        /// <param name="flags">Bitwise-or of TRESearchOptions.</param>
        /// <returns></returns>
        [DllImport(TreLibrary, EntryPoint = "tre_regexec", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern TreErrorCode regexec(
            ref regex_t preg,
            string input,
            size_t nmatch,
            [In, Out] regmatch_t[] pmatch,
            TRESearchOptions flags
        );

        /// <summary>
        /// Approximately match a null-terminated string against the specified, pre-compiled pattern buffer preg.
        /// </summary>
        /// <param name="preg">The pre-compiled regular expression pattern buffer.</param>
        /// <param name="input">The null-terminated input string to match against.</param>
        /// <param name="match">Stores results of the approximate match.</param>
        /// <param name="parameters">Parameters for the approximate match.</param>
        /// <param name="flags">Bitwise-or of TRESearchOptions.</param>
        /// <returns></returns>
        [DllImport(TreLibrary, EntryPoint = "tre_regaexec", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern TreErrorCode regaexec(
            ref regex_t preg,
            string input,
            ref regamatch_t match,
            regaparams_t parameters,
            TRESearchOptions flags
        );
    }
}
