using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DimaCalc.Parser;
using System.Collections.Generic;
using System.Linq;

namespace DimaCalc.Test.Parser
{
    [TestClass]    
    public class TestParserEngine
    {
        [TestMethod]
        public void TestConstructor()
        {
            var p = new ParserEngine();
        }

        [TestMethod]
        public void Test_Parse1()
        {
            var p = new ParserEngine();

            Parse_ChunkCount(p, "2", 2);
            Parse_ChunkCount(p, "2+", 3);
        }

        [TestMethod]
        public void Test_Parse2()
        {
            var p = new ParserEngine();

            Parse_ChunkCount(p, "2+3", 4);
            Parse_ChunkCount(p, "2.0+3.0", 4);
            Parse_ChunkCount(p, "2.0 +3.0", 4);
            Parse_ChunkCount(p, "2.0*11.30 - 42", 6);

        }

        [TestMethod]
        public void TestParse_SyntaxError()
        {
            var p = new ParserEngine();

            Parse_Exception(p, "2a+3", "Unsupported character or syntax error a");
            Parse_Exception(p, "2++2");
            Parse_Exception(p, "2.2. + 3");
        }

        [TestMethod]
        public void TestParse_CorrectChunktypes()
        {
            var p = new ParserEngine();

            Parse_Sequence(p, "345.123()+ - * : = > >= < <=", new ExpressionChunk[] {
                new ExpressionChunk("345.123", ChunkTypes.Number, 345.123),
                new ExpressionChunk("(", ChunkTypes.ParenthesisLeft),
                new ExpressionChunk(")", ChunkTypes.ParenthesisRight),
                new ExpressionChunk("+", ChunkTypes.Plus),
                new ExpressionChunk("-", ChunkTypes.Minus),
                new ExpressionChunk("*", ChunkTypes.Multiply),
                new ExpressionChunk(":", ChunkTypes.Divide),
                new ExpressionChunk("=", ChunkTypes.Equals),
                new ExpressionChunk(">", ChunkTypes.Greater),
                new ExpressionChunk(">=", ChunkTypes.GreaterOrEqual),
                new ExpressionChunk("<", ChunkTypes.Less),
                new ExpressionChunk("<=", ChunkTypes.LessOrEqual),
                new ExpressionChunk("", ChunkTypes.EOL)
            });
        }


        [TestMethod]
        public void TestParse_CorrectSequence()
        {
            var p = new ParserEngine();

            Parse_Sequence(p, "2.0*(11.3+34-22):5",
                new ExpressionChunk[] {
                    new ExpressionChunk("2.0", ChunkTypes.Number, 2.0),
                    new ExpressionChunk("*", ChunkTypes.Multiply),
                    new ExpressionChunk("(", ChunkTypes.ParenthesisLeft),
                    new ExpressionChunk("11.3", ChunkTypes.Number, 11.3),
                    new ExpressionChunk("+", ChunkTypes.Plus),
                    new ExpressionChunk("34", ChunkTypes.Number, 34),
                    new ExpressionChunk("-", ChunkTypes.Minus),
                    new ExpressionChunk("22", ChunkTypes.Number, 22),
                    new ExpressionChunk(")", ChunkTypes.ParenthesisRight),
                    new ExpressionChunk(":", ChunkTypes.Divide),
                    new ExpressionChunk("5", ChunkTypes.Number, 5),
                    new ExpressionChunk("", ChunkTypes.EOL)
                });
        }


        private void Parse_Exception(ParserEngine p, string sample, string testMessage = null)
        {
            var passed = false;
            Exception ex = null;
            try
            {
                p.Parse(sample);
                passed = true;
            }
            catch (Exception err)
            {
                ex = err;
            }

            if (passed)
            {
                Assert.Fail("Expected syntax failure for: " + sample);
            }

            Assert.IsNotNull(ex, "Syntax error was not raised");
            Assert.IsInstanceOfType(ex, typeof(SyntaxException),
                "Parse generated wrong exception " + ex.GetType());
            if (testMessage != null)
            {
                Assert.AreEqual(testMessage, ex.Message);
            }

        }

        private void Parse_ChunkCount(ParserEngine p, 
            string sample,
            int expectedCount
            )
        {
            List<ExpressionChunk> chunks = p.Parse(sample);
            Assert.IsNotNull(chunks, string.Format("Parse of {0} returned null instead of list", sample));

            var count = chunks.Count;
            Assert.AreEqual(expectedCount, chunks.Count,
                string.Format("{0} returned {1} items instead of {2}", sample, count, expectedCount));

            var lastItemIs = chunks.Last().ChunkType;
            Assert.AreEqual(
                ChunkTypes.EOL,
                lastItemIs,
                string.Format("Last item type for {0} is {1} instead of EOL", sample, lastItemIs));


        }

        private void Parse_Sequence(ParserEngine p, string sample, IEnumerable<ExpressionChunk> chunks_e)
        {
            var testChunks = p.Parse(sample);
            var chunks = chunks_e.ToList();

            Assert.AreEqual(chunks.Count, testChunks.Count, "Lists did not matched in length");

            for (int i = 0; i < testChunks.Count; i++)
            {
                Assert.AreEqual(chunks[i].ChunkType, testChunks[i].ChunkType);
                if(chunks[i].ChunkType == ChunkTypes.Number)
                {
                    Assert.AreEqual(chunks[i].Value, testChunks[i].Value);
                }
            }

        }
    }
}
