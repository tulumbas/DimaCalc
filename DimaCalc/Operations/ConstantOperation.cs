using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Operations
{
    public class ConstantOperation : Operation
    {
        public double Value { get; private set; }

        public ConstantOperation(double value)
        {
            Value = value;
        }

        public override Operation Calculate()
        {
            return this;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
