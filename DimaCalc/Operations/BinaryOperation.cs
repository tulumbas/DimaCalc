using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Operations
{
    public class BinaryOperation : Operation
    {
        public MathOperations Operand { get; private set; }
        public Operation Left { get; private set; }
        public Operation Right { get; private set; }

        public BinaryOperation(Operation left, Operation right, MathOperations operand)
        {
            Operand = operand;
            Left = left;
            Right = right;
        }

        public override Operation Calculate()
        {
            var op1 = Left.Calculate();
            var op2 = Right.Calculate();

            if (op1 is ConstantOperation && op2 is ConstantOperation)
            {
                var l = (op1 as ConstantOperation).Value;
                var r = (op2 as ConstantOperation).Value;

                double res;
                switch (Operand)
                {
                    case MathOperations.Add:
                        res = l + r;
                        break;

                    case MathOperations.Subtract:
                        res = l - r;
                        break;

                    case MathOperations.Divide:
                        //if (Math.Abs(r) < Double.MinValue)
                        //{
                        //    throw new DivideByZeroException("Ы! (divide by zero)");
                        //}
                        res = l / r;
                        break;

                    case MathOperations.Multiply:
                        res = l * r;
                        break;

                    case MathOperations.Power:
                        res = Math.Exp(r * Math.Log(l));
                        break;
                    
                    default:
                        throw new ApplicationException("Unknown binary operation");
                }

                return new ConstantOperation(res);
            }
            else
            {
                return new BinaryOperation(op1, op2, Operand);
            }

        }

        public override string ToString()
        {
            string op;
            switch (Operand)
            {
                case MathOperations.Add: op = "+"; break;
                case MathOperations.Subtract: op = "-"; break;
                case MathOperations.Divide: op = "/"; break;
                case MathOperations.Multiply: op = "*"; break;
                default:
                    op = "(unknown)";
                    break;
            }

            return string.Format("({0} {1} {2})", Left, op, Right);
        }
    }


}
