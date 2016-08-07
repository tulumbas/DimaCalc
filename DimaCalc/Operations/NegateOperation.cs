using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Operations
{
    public class NegateOperation : Operation
    {
        Operation _child;

        public NegateOperation(Operation child)
        {
            _child = child;
        }

        public override Operation Calculate()
        {
            var res = _child.Calculate();
            if (res is ConstantOperation)
            {
                return new ConstantOperation(-(res as ConstantOperation).Value);
            }
            else
            {
                return new NegateOperation(res);
            }
        }

        public override string ToString()
        {
            return string.Format("-{0}", _child);
        }
    }

}
