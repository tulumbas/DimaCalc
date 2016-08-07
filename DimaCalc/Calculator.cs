using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DimaCalc.Operations;
using DimaCalc.Parser;

namespace DimaCalc
{
    class Calculator
    {
        public static string Run(string question)
        {
            var parser = new ParserEngine();
            var builder = new OperationBuilder();

            try
            {
                var problem = builder.Build(parser.Parse(question));
                var result = problem.Calculate();
                return string.Format("{0} = {1}", problem, result);
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }

    }

}
