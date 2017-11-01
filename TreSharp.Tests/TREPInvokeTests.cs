using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Runtime.InteropServices;

namespace TreSharp.Tests
{
    [TestClass]
    public class TrePInvokeTests
    {
        [TestMethod]
        public void Test_tre_version_returns_version()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Tre.GetVersion()), "tre_version did not return a version string.");
        }

        [TestMethod]
        public void TestCompileValidRegex()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            TreErrorCode rc = Tre.regcomp(ref rgx, "abc", TrePatternOptions.REG_EXTENDED);
            Assert.AreEqual(TreErrorCode.REG_OK, rc);
            rgx.Dispose();
        }

        [TestMethod]
        public void TestCompileInvalidRegex()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            TreErrorCode rc = Tre.regcomp(ref rgx, "ab(", TrePatternOptions.REG_EXTENDED);
            Assert.AreEqual(TreErrorCode.REG_EPAREN, rc);
            rgx.Dispose();
        }

        [TestMethod]
        public void TestCompileInvalidRegexGetErrorMessage()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            TreErrorCode rc = Tre.regcomp(ref rgx, "ab(", TrePatternOptions.REG_EXTENDED);
            
            var errorSize = Tre.regerror(rc, ref rgx, null, new UIntPtr(0));
            StringBuilder errorMsg = new StringBuilder();
            Tre.regerror(rc, ref rgx, errorMsg, errorSize);

            Assert.IsTrue(errorSize.ToUInt32() > 0, "Error message size retrieved from regerror was not greater than 0.");
            Assert.IsTrue(!string.IsNullOrEmpty(errorMsg.ToString()), "regerror returned an empty error message.");
            rgx.Dispose();
        }

        [TestMethod]
        public void TestFreeRegex()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            Tre.regcomp(ref rgx, "abc", TrePatternOptions.REG_EXTENDED);
            rgx.Dispose();
        }

        [TestMethod]
        public void TestMatchWithGroups()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            string pattern = ".*(cd(e)).*";
            var rc1 = Tre.regcomp(ref rgx, pattern, TrePatternOptions.REG_EXTENDED);

            var output = new Tre.regmatch_t[rgx.re_nsub + 1];
            TreErrorCode rc = Tre.regexec(ref rgx, "abcdefg", new UIntPtr(rgx.re_nsub + 1), output, TRESearchOptions.None);

            Assert.IsTrue(output.Length == 3);
            Assert.AreEqual(0, output[0].rm_so, "First match does not start at beginning of string.");
            Assert.AreEqual(7, output[0].rm_eo, "First match does not encompass entire string.");

            Assert.AreEqual(2, output[1].rm_so, "Second match group does not start where expected.");
            Assert.AreEqual(5, output[1].rm_eo, "Second match group does not end where expected.");

            Assert.AreEqual(4, output[2].rm_so, "Third match group does not end where expected.");
            Assert.AreEqual(5, output[2].rm_eo, "Third match group does not end where expected.");
            rgx.Dispose();
        }

        [TestMethod]
        public void TestApproximateFunctionMatchesWithoutRunningApproximateSearch()
        {
            // Make sure approx regex function works even if we aren't looking for an approximate match.
            Tre.regex_t rgx = new Tre.regex_t();
            string pattern = ".*(cd(e)).*";
            var rc1 = Tre.regcomp(ref rgx, pattern, TrePatternOptions.REG_EXTENDED);

            Tre.regamatch_t match = new Tre.regamatch_t();
            match.nmatch = rgx.re_nsub + 1;

            Tre.regmatch_t[] pmatch = new Tre.regmatch_t[rgx.re_nsub + 1];
            GCHandle pinnedArray = GCHandle.Alloc(pmatch, GCHandleType.Pinned);
            IntPtr ptr = pinnedArray.AddrOfPinnedObject();
            match.pmatch = ptr;

            try
            {
                Tre.regaparams_t parameters = new Tre.regaparams_t();
                Tre.regaparams_default(ref parameters);
                Tre.regaexec(
                    ref rgx,
                    "abcdefg",
                    ref match, parameters, TRESearchOptions.None);
            }
            finally
            {
                pinnedArray.Free();
                rgx.Dispose();
            }

            Assert.IsTrue(pmatch.Length == 3);
            Assert.AreEqual(0, pmatch[0].rm_so, "First match does not start at beginning of string.");
            Assert.AreEqual(7, pmatch[0].rm_eo, "First match does not encompass entire string.");

            Assert.AreEqual(2, pmatch[1].rm_so, "Second match group does not start where expected.");
            Assert.AreEqual(5, pmatch[1].rm_eo, "Second match group does not end where expected.");

            Assert.AreEqual(4, pmatch[2].rm_so, "Third match group does not end where expected.");
            Assert.AreEqual(5, pmatch[2].rm_eo, "Third match group does not end where expected.");
        }

        [TestMethod]
        public void TestApproximateMatch()
        {
            Tre.regex_t rgx = new Tre.regex_t();
            string pattern = "abcxeq";
            var rc1 = Tre.regcomp(ref rgx, pattern, TrePatternOptions.REG_EXTENDED);

            Tre.regamatch_t match = new Tre.regamatch_t();
            match.nmatch = rgx.re_nsub + 1;
            //match.pmatch = new TRE.regmatch_t[rgx.re_nsub + 1];
            
            Tre.regmatch_t[] pmatch = new Tre.regmatch_t[rgx.re_nsub + 1];
            GCHandle pinnedArray = GCHandle.Alloc(pmatch, GCHandleType.Pinned);
            IntPtr ptr = pinnedArray.AddrOfPinnedObject();
            match.pmatch = ptr;

            try
            {
                Tre.regaparams_t parameters = new Tre.regaparams_t();
                Tre.regaparams_default(ref parameters);
                parameters.max_cost = 3;
                parameters.cost_del = parameters.cost_ins = parameters.cost_subst = 1;
                
                TreErrorCode rc = Tre.regaexec(
                    ref rgx,
                    "abcde",
                    ref match, parameters, TRESearchOptions.None);
            }
            finally
            {
                pinnedArray.Free();
                rgx.Dispose();
            }

            Assert.AreEqual(2, match.cost);
            Assert.AreEqual(1, match.num_del);
        }
    }
}
