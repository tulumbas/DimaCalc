using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DimaCalc.Operations;

namespace DimaCalc
{
    class Calculator
    {
        public static string Run()
        {
            var problem =
                new BinaryOperation(
                    new BinaryOperation(new ConstantOperation(12.3), new NegateOperation(new ConstantOperation(11.3)), MathOperations.Add),
                    new BinaryOperation(new ConstantOperation(10), new ConstantOperation(8), MathOperations.Subtract),
                    MathOperations.Multiply);
            var result = problem.Calculate();
            return string.Format("{0} = {1}", problem, result);
        }

    }

}
