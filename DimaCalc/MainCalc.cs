using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc
{
    class BinaryOpeation
    {
        public float leftMember, rightMember, result;
        public char oper;
        public BinaryOpeation(float rightMem, float leftMem, char o)
        {
            leftMember = rightMem;
            rightMember = leftMem;
            oper = o;
            switch (oper)
            {
                case '-': result = leftMember - rightMember; break;
                case '+': result = leftMember + rightMember; break;
                case '*': result = leftMember * rightMember; break;
                case '/': result = leftMember / rightMember; break;
                default: break;
            }
        }
    }
    class MainCalc
    {
        
        BinaryOpeation ConvertStringToOperation(string s)
        {
            if (s.IndexOfAny(new char[] { '-', '+', '*', '/' }) != -1)
            {
                if (s.IndexOfAny(new char[] { '-', '+' }) != -1)
                {
                    int indexOfOper = 0;
                    int i = 0;
                    while (s.IndexOfAny(new char[] { '-', '+' }, i) != -1)
                    {
                        indexOfOper = s.IndexOfAny(new char[] { '-', '+' }, i);
                    }
                    return new BinaryOpeation(ConvertStringToOperation(s.Substring(0, indexOfOper)).leftMember,
                        ConvertStringToOperation(s.Substring(indexOfOper + 1)).leftMember, s[indexOfOper]);
                }
                else
                {
                    if (s.IndexOfAny(new char[] { '/', '*' }) != -1)
                    {
                        int indexOfOper = 0;
                        int i = 0;
                        while (s.IndexOfAny(new char[] { '/', '*' }, i) != -1)
                        {
                            indexOfOper = s.IndexOfAny(new char[] { '/', '*' }, i);
                        }
                        return new BinaryOpeation(ConvertStringToOperation(s.Substring(0, indexOfOper)).leftMember,
                            ConvertStringToOperation(s.Substring(indexOfOper + 1)).leftMember, s[indexOfOper]);
                    }
                    else return null;
                }
            }

            else
            {
                float number;
                if (Single.TryParse(s, out number)) return new BinaryOpeation(number, 0.1f, 'o');
                else
                {
                    return null;
                }
            }
        }
        
        //string GetEquationFromBrackets(string equation)
        //{
        //    string stringToReturn = "";

        //    if ((equation.IndexOf("(") != -1) && (equation.IndexOf(")") != -1))
        //    {
        //        for (int i = equation.IndexOf("("); i < equation.IndexOf(")"); i++)
        //        {
        //            stringToReturn += equation[i];
        //        }

        //        return stringToReturn;
        //    }
        //    return null;
        //}

        
    }
}
