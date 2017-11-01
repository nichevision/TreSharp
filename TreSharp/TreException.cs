using System;
using System.Collections.Generic;
using System.Text;

namespace TreSharp
{
    /// <summary>
    /// Represents an error related to TRE.
    /// </summary>
    public class TreException : Exception
    {
        /// <summary>
        /// Get the TRE error code.
        /// </summary>
        public TreErrorCode ErrorCode { get; }

        public TreException(TreErrorCode errorCode) : 
            this(errorCode, string.Empty)
        {
        }

        public TreException(TreErrorCode errorCode, string message) :
            base(message)
        {
            ErrorCode = errorCode;
        }

        public static TreException FromCode(TreErrorCode code, Tre.regex_t rgx)
        {
            UIntPtr errorSize = Tre.regerror(code, ref rgx, null, new UIntPtr(0));
            StringBuilder sb = new StringBuilder((int)errorSize.ToUInt32());
            Tre.regerror(code, ref rgx, sb, errorSize);
            return new TreException(code, sb.ToString());
        }
    }
}
