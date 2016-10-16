using DimaCalc.Operations;
using DimaCalc.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimaCalc.Test.Parser
{
    [TestClass]
    public class TestOperationBuilder
    {
        [TestCategory("Builder")]
        [TestMethod]
        public void TestConstructor()
        {
            var c = new OperationBuilder();           
        }

        [TestMethod]
        public void TestBuild_OperationType()
        {
            var b = new OperationBuilder();

            CheckOperationType(b, "2", typeof(ConstantOperation));
            CheckOperationType(b, "2.4", typeof(ConstantOperation));
            CheckOperationType(b, "-2", typeof(NegateOperation));
            CheckOperationType(b, "2*3", typeof(BinaryOperation));
            CheckOperationType(b, "2+5", typeof(BinaryOperation));
        }


        [TestMethod]
        public void TestBuild_SimpleOperations1()
        {
            var b = new OperationBuilder();

            CompareTrees(
                new BinaryOperation(
                    new ConstantOperation(2.13),
                    new ConstantOperation(13.45),
                    MathOperations.Add),
                CheckOperationType(b, "2.13+ 13.45", typeof(BinaryOperation)));
        }

        [TestMethod]
        public void TestBuild_SimpleOperations2()
        {
            var b = new OperationBuilder();

            CompareTrees(
                new BinaryOperation(
                    new BinaryOperation(
                        new ConstantOperation(123),
                        new ConstantOperation(135),
                        MathOperations.Subtract),
                    new ConstantOperation(45),
                    MathOperations.Add),
                CheckOperationType(b, "(123 -135)+ 45", typeof(BinaryOperation)));
        }

        [TestMethod]
        public void TestBuild_SimpleOperations3()
        {
            var b = new OperationBuilder();
            var op = CheckOperationType(b, "21.3*((16+8):6 - (1 + 2))",
                typeof(BinaryOperation));

            // 21.3*((16+8):6 - (1 + 2)) = 21.3
            CompareTrees(
                new BinaryOperation( //*
                    new ConstantOperation(21.3),
                    new BinaryOperation( // -
                        new BinaryOperation( // :
                            new BinaryOperation( // +
                                new ConstantOperation(16),
                                new ConstantOperation(8),
                                MathOperations.Add),
                            new ConstantOperation(6),
                            MathOperations.Divide),
                        new BinaryOperation( // 1+2
                            new ConstantOperation(1),
                            new ConstantOperation(2),
                            MathOperations.Add),
                        MathOperations.Subtract),
                    MathOperations.Multiply),
                op);

            var result = op.Calculate();
            Assert.IsInstanceOfType(result, typeof(ConstantOperation));
            Assert.AreEqual(21.3, ((ConstantOperation)result).Value);
        }

        [TestMethod]
        public void TestBuild_SimpleOperations4()
        {
            var b = new OperationBuilder();
            var op = CheckOperationType(b, "123 -135+ 45", typeof(BinaryOperation));

            CompareTrees(
                new BinaryOperation(
                    new BinaryOperation(
                        new ConstantOperation(123),
                        new ConstantOperation(135),
                        MathOperations.Subtract),
                    new ConstantOperation(45),
                    MathOperations.Add),
                op);
        }

        [TestMethod]
        public void TestBuild_SimpleOperations5()
        {
            var b = new OperationBuilder();

            CompareTrees(
                new BinaryOperation(
                    new BinaryOperation(
                        new ConstantOperation(123),
                        new ConstantOperation(135),
                        MathOperations.Multiply),
                    new ConstantOperation(45),
                    MathOperations.Subtract),
                CheckOperationType(b, "123* 135- 45", typeof(BinaryOperation)));
        }


        private Operation CheckOperationType(OperationBuilder b, string sample, Type t)
        {
            List<ExpressionChunk> e = GetChunks(sample);

            var o = b.Build(e);
            Assert.IsNotNull(o, "Operation is null for '{0}'", sample);
            Assert.IsInstanceOfType(o, t, "Wrong operation type for '{0}': {1}", sample, o.GetType());
            return o;
        }

        private static List<ExpressionChunk> GetChunks(string sample)
        {
            List<ExpressionChunk> e = null;
            try
            {
                var p = new ParserEngine();
                e = p.Parse(sample);
                Assert.IsNotNull(e);
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not parse {0}: ", sample, ex);
            }

            return e;
        }

        private void CompareTrees(Operation expected, Operation tested)
        {
            if (expected == null) Assert.IsNull(tested); else Assert.IsNotNull(tested);

            Assert.IsInstanceOfType(tested, expected.GetType());

            if(expected is BinaryOperation)
            {
                var tree_e = expected as BinaryOperation;
                var tree_t = tested as BinaryOperation;
                Assert.AreEqual(tree_e.Operand, tree_t.Operand,
                    "Operation mismatched: '{0}' vs. '{1}'", tree_e.Operand, tree_t.Operand);
                CompareTrees(tree_e.Left, tree_t.Left);
                CompareTrees(tree_e.Right, tree_t.Right);
            }
            else if(expected is ConstantOperation)
            {
                Assert.AreEqual((expected as ConstantOperation).Value,
                    (tested as ConstantOperation).Value);
            }
        }
    }
}
