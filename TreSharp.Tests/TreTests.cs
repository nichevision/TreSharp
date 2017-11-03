using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TreSharp.Tests
{
    [TestClass]
    public class TreTests
    {
        [TestMethod]
        public void TestSimpleMatchSuccess()
        {
            using (var rgx = new Regex("hello world"))
            {
                Assert.IsTrue(rgx.IsMatch("hello world"));
            }
        }

        [TestMethod]
        public void TestNextMatchReturnsAllMatches()
        {
            using (var rgx = new Regex("ABCD"))
            {
                var first = rgx.Match("ABCDABCDABCDABCD");
                var second = first.NextMatch();
                var third = second.NextMatch();
                var fourth = third.NextMatch();
                var fifth = fourth.NextMatch();

                Assert.AreEqual(0, first.Index);
                Assert.AreEqual(4, second.Index);
                Assert.AreEqual(8, third.Index);
                Assert.AreEqual(12, fourth.Index);
                Assert.IsFalse(fifth.Success);
            }
        }

        [TestMethod]
        public void TestCanStartNextMatchInsidePreviousMatch()
        {
            using (var rgx = new Regex("AAAA"))
            {
                var first = rgx.Match("AAAAAAAAAAAA");
                var second = first.NextMatch(false);
                var third = second.NextMatch(false);

                Assert.AreEqual(0, first.Index);
                // If we did not specify startAfterEntireCurrentMatch = false, the index of the
                // second match would be 4 instead of 1.
                Assert.AreEqual(1, second.Index, "Second match has unexpected index.");
                Assert.AreEqual(2, third.Index, "Third match has unexpected index.");
            }
        }

        [TestMethod]
        public void TestNextMatchSetsCorrectMatchGroupIndices()
        {
            using (var rgx = new Regex("AB(CD)"))
            {
                var first = rgx.Match("ABCDABCD");
                var second = first.NextMatch();

                Assert.AreEqual(2, first.Groups[1].Index, "Incorrect subgroup match index.");
                Assert.AreEqual(6, second.Groups[1].Index, "Incorrect subgroup match index.");
            }
        }

        [TestMethod]
        public void TestSimpleMatchFail()
        {
            using (var rgx = new Regex("jello world"))
            {
                Assert.IsFalse(rgx.IsMatch("hello world"));
            }
            using (var rgx = new Regex("hello world"))
            {
                Assert.IsFalse(rgx.IsMatch("jello world"));
            }
        }

        [TestMethod]
        public void TestImbalancedParenthesisThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("a(bc");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_EPAREN, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_EPAREN");
        }

        [TestMethod]
        public void TestImbalancedBracketsThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("a[bc");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_EBRACK, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_EBRACK");
        }

        [TestMethod]
        public void TestImbalancedBracesThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("abc{");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_EBRACE, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_EBRACE");
        }

        [TestMethod]
        public void TestBracesWithInvalidContentThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("a{bc");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_BADBR, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_BADBR");
        }

        [TestMethod]
        public void TestInvalidRepetitionSpecifierThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("abc++");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_BADRPT, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_BADRPT");
        }

        [TestMethod]
        public void TestInvalidBackReferenceThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("abc\\1d");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_ESUBREG, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_ESUBREG");
        }

        [TestMethod]
        public void TestTrailingBackslashThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("abc\\");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_EESCAPE, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_EESCAPE");
        }

        [TestMethod]
        public void TestInvalidCharacterRangeThrowsException()
        {
            TreException ex = null;
            try
            {
                new Regex("abc[z-a]");
            }
            catch (TreException e)
            {
                ex = e;
            }
            Assert.AreEqual(TreErrorCode.REG_ERANGE, ex.ErrorCode, "Invalid regex did not throw exception with error code REG_ERANGE");
        }

        [TestMethod]
        public void TestDisposedRegexThrowsExceptionWhenUsed()
        {
            var rgx = new Regex("abc\\d");
            rgx.IsMatch("abc1");
            rgx.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => rgx.IsMatch("abc1"));
        }

        [TestMethod]
        public void TestMatchWithGroups()
        {
            using (var rgx = new Regex("h(el)lo wor(ld)"))
            {
                var match = rgx.Match("hello world");
                Assert.IsTrue(match.Success, "Match with groups failed.");
                // First group is entire match
                Assert.AreEqual(3, match.Groups.Count);
                Assert.AreEqual("hello world", match.Groups[0].Value, "Group did not have expected match value.");
                Assert.AreEqual("el", match.Groups[1].Value, "Group did not have expected match value.");
                Assert.AreEqual("ld", match.Groups[2].Value, "Group did not have expected match value.");
            }
        }

        [TestMethod]
        public void TestApproxRegexWithSubstitutions()
        {
            using (var rgx = new Regex("ABCDEFG"))
            {
                Assert.IsFalse(rgx.IsMatch("ABYDEFG"), "Unexpected match.");

                // If we allow approximate matching, but match still fails, expect
                // certain results in properties of that match.
                var failApproximateMatch = rgx.Match("ABYDEFQ", new TreOptions() { MaxCost = 1 });
                Assert.IsFalse(failApproximateMatch.Success);
                Assert.IsFalse(failApproximateMatch.IsApproximate);
                Assert.AreEqual(0, failApproximateMatch.Cost);
                Assert.AreEqual(0, failApproximateMatch.NumberOfDeletions);
                Assert.AreEqual(0, failApproximateMatch.NumberOfInsertions);
                Assert.AreEqual(0, failApproximateMatch.NumberOfSubstitutions);

                Assert.IsTrue(
                    rgx.IsMatch("ABYDEFG", new TreOptions()
                    {
                        MaxCost = 1
                    }),
                    "Approximate match failed."
                );

                var match = rgx.Match("ABYDEFG", new TreOptions()
                {
                    MaxCost = 2,
                    CostOfSubstitutions = 2
                });

                Assert.AreEqual(1, match.NumberOfSubstitutions);
                Assert.AreEqual(0, match.NumberOfDeletions);
                Assert.AreEqual(0, match.NumberOfInsertions);
            }
        }

        [TestMethod]
        public void TestApproxRegexWithDeletions()
        {
            using (var rgx = new Regex("ABCDEFG"))
            {
                string input = "ABDEFG";
                Assert.IsFalse(rgx.IsMatch(input), "Unexpected match.");

                Assert.IsTrue(
                    rgx.IsMatch(input, new TreOptions()
                    {
                        MaxCost = 1
                    }),
                    "Approximate match failed."
                );

                var match = rgx.Match(input, new TreOptions()
                {
                    MaxCost = 2,
                    CostOfDeletions = 2
                });

                Assert.AreEqual(0, match.NumberOfSubstitutions);
                Assert.AreEqual(1, match.NumberOfDeletions);
                Assert.AreEqual(0, match.NumberOfInsertions);
            }
        }

        [TestMethod]
        public void TestApproxRegexWithInsertions()
        {
            using (var rgx = new Regex("ABCDEFG"))
            {
                string input = "ABCCDEFG";
                Assert.IsFalse(rgx.IsMatch(input), "Unexpected match.");

                Assert.IsTrue(
                    rgx.IsMatch(input, new TreOptions()
                    {
                        MaxCost = 1
                    }),
                    "Approximate match failed."
                );

                var match = rgx.Match(input, new TreOptions()
                {
                    MaxCost = 2,
                    CostOfDeletions = 3,
                    CostOfSubstitutions = 3,
                    CostOfInsertions = 2
                });

                Assert.AreEqual(0, match.NumberOfSubstitutions);
                Assert.AreEqual(0, match.NumberOfDeletions);
                Assert.AreEqual(1, match.NumberOfInsertions);
            }
        }

        [TestMethod]
        public void TestApproximationSyntax()
        {
            // (Cost of Deletion = 1) + (Cost of Insertion = 0) + (Cost of Substitution = 0) < ((Max Cost = 1) + 1)
            using (var rgx = new Regex("{ 1d + 0i + 0s < 2 }ABC"))
            {
                var match = rgx.Match("BC");
                Assert.IsTrue(match.IsApproximate, "Approximate match via syntax failed.");
                Assert.AreEqual(1, match.NumberOfDeletions, "Approximate match via syntax failed.");
                Assert.AreEqual(1, match.Cost, "Approximate match via syntax failed.");

                // Syntax overrides TreOptions
                match = rgx.Match("BC", new TreOptions() { MaxCost = 0 });
                Assert.IsTrue(match.IsApproximate, "Regex syntax did not override TreOptions");
                Assert.AreEqual(1, match.NumberOfDeletions, "Regex syntax did not override TreOptions");
                Assert.AreEqual(1, match.Cost, "Regex syntax did not override TreOptions");
            }

            using (var rgx = new Regex("{ 5d + 0i + 0s < 2 }ABC"))
            {
                var match = rgx.Match("BC");
                Assert.IsFalse(match.Success, "Approximate match via syntax failed.");
            }

            // There must be a space between the cost equation and its opening bracket
            Assert.ThrowsException<TreException>(() => new Regex("{1d + 0i + 0s < 2}ABC"));
        }
    }
}
