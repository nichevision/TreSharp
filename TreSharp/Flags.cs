using System;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    [Flags]
    public enum TRESearchOptions : int
    {
        None = 0,
        /// <summary>
        /// When this flag is used, the match-beginning-of-line operator ^ does not match the empty string at the beginning of string. 
        /// If REG_NEWLINE was used when compiling preg the empty string immediately after a newline character will still be matched.
        /// </summary>
        REG_NOTBOL = 1,
        /// <summary>
        /// When this flag is used, the match-end-of-line operator $ does not match the empty string at the end of string. 
        /// If REG_NEWLINE was used when compiling preg the empty string immediately before a newline character will still be matched.
        /// </summary>
        REG_NOTEOL = REG_NOTBOL << 1,
    }

    [Flags]
    public enum TrePatternOptions : int
    {
        REG_BASIC = 0,
        /// <summary>
        /// Use POSIX Extended Regular Expression (ERE) compatible syntax when compiling regex. The default syntax is the POSIX Basic Regular Expression (BRE) syntax, but it is considered obsolete.
        /// </summary>
        REG_EXTENDED = 1,
        /// <summary>
        /// Ignore case. Subsequent searches with the regexec family of functions using this pattern buffer will be case insensitive.
        /// </summary>
        REG_ICASE = REG_EXTENDED << 1,
        /// <summary>
        /// Normally the newline character is treated as an ordinary character.  When this flag is used, the newline character ('\n', ASCII code 10) is treated specially as follows:
        /// <list type="">
        /// <item>The match-any-character operator (dot "." outside a bracket expression) does not match a newline.</item>
        /// <item>A non-matching list ([^...]) not containing a newline does not match a newline.</item>
        /// <item>The match-beginning-of-line operator ^ matches the empty string immediately after a newline as well as the empty string at the beginning of the string (but see the REG_NOTBOL regexec() flag below).</item>
        /// <item>The match-end-of-line operator $ matches the empty string immediately before a newline as well as the empty string at the end of the string (but see the REG_NOTEOL regexec() flag below).</item>
        /// </list>
        /// </summary>
        REG_NEWLINE = REG_ICASE << 1,
        /// <summary>
        /// Do not report submatches. Subsequent searches with the regexec family of functions will only report whether a match was found or not and will not fill the submatch array.
        /// </summary>
        REG_NOSUB = REG_NEWLINE << 1,
        /// <summary>
        /// Interpret the entire regex argument as a literal string, that is, all characters will be considered ordinary. This is a nonstandard extension, compatible with but not specified by POSIX.
        /// </summary>
        REG_LITERAL = REG_NOSUB << 1,
        /// <summary>
        /// Same as REG_LITERAL. This flag is provided for compatibility with BSD.
        /// </summary>
        REG_NOSPEC = REG_LITERAL,
        /// <summary>
        /// By default, concatenation is left associative in TRE, as per the grammar given in the base specifications on regular expressions of Std 1003.1-2001 (POSIX). This flag flips associativity of concatenation to right associative. Associativity can have an effect on how a match is divided into submatches, but does not change what is matched by the entire regexp.
        /// </summary>
        REG_RIGHT_ASSOC = REG_LITERAL << 1,
        /// <summary>
        /// By default, repetition operators are greedy in TRE as per Std 1003.1-2001 (POSIX) and can be forced to be non-greedy by appending a ? character. This flag reverses this behavior by making the operators non-greedy by default and greedy when a ? is specified.
        /// </summary>
        REG_UNGREEDY = REG_RIGHT_ASSOC << 1,

        REG_APPROX_MATCHER = 0x1000,
        REG_BACKTRACKING_MATCHER = REG_APPROX_MATCHER << 1
    }

    public enum TreErrorCode : int
    {
        REG_OK = 0,     /* No error. */
                        /* POSIX tre_regcomp() return error codes.  (In the order listed in the
                           standard.)	 */
        REG_NOMATCH,        /* No match. */
        /// <summary>
        /// Invalid regexp. TRE returns this only if a multibyte character set is used in the current locale, and regex contained an invalid multibyte sequence.
        /// </summary>
        REG_BADPAT,
        /// <summary>
        /// Invalid collating element referenced. TRE returns this whenever equivalence classes or multicharacter collating elements are used in bracket expressions (they are not supported yet).
        /// </summary>
        REG_ECOLLATE,
        /// <summary>
        /// Unknown character class name in [[:name:]].
        /// </summary>
        REG_ECTYPE,
        /// <summary>
        /// The last character of regex was a backslash (\).
        /// </summary>
        REG_EESCAPE,
        /// <summary>
        /// Invalid back reference; number in \digit invalid.
        /// </summary>
        REG_ESUBREG,
        /// <summary>
        /// [] imbalance.
        /// </summary>
        REG_EBRACK,
        /// <summary>
        /// \(\) or () imbalance.
        /// </summary>
        REG_EPAREN,
        /// <summary>
        /// \{\} or {} imbalance.
        /// </summary>
        REG_EBRACE,
        /// <summary>
        /// {} content invalid: not a number, more than two numbers, first larger than second, or number too large.
        /// </summary>
        REG_BADBR,
        /// <summary>
        /// Invalid character range, e.g. ending point is earlier in the collating order than the starting point.
        /// </summary>
        REG_ERANGE,
        /// <summary>
        /// Out of memory, or an internal limit exceeded.
        /// </summary>
        REG_ESPACE,
        /// <summary>
        /// Invalid use of repetition operators: two or more repetition operators have been chained in an undefined way.
        /// </summary>
        REG_BADRPT
    }
}
