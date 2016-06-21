using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Parser
{
    class Parser
    {
        string _source;
        int charPosition, length;

        string[] knonwOperations = new string[] {"+", "-", ":", "*"};

        public List<ExpressionChunk> Parse(string source)
        {
            _source = source;
            charPosition = 0;
            length = source.Length;

            var expr = new List<ExpressionChunk>();

            
            while (charPosition < length)
            {
                char c = _source[charPosition];

                if (Char.IsDigit(c))
                {
                    var p1 = charPosition;
                    double number;
                    if(ReadNumber(out number))
                    {
                        var text = _source.Substring(p1, charPosition - p1);
                        expr.Add(new ExpressionChunk(text, ChunkTypes.Number, number));
                    }
                }

                if (_source[charPosition] == ' ')
                {
                    charPosition++;
                }

                switch (_source[charPosition])
                {
                    case ' ':
                        charPosition++;
                        break;

                    case '(':
                        expr.Add(new ExpressionChunk(_source[charPosition].ToString(), ChunkTypes.ParenthesisLeft));
                        charPosition++;
                        break;

                    case ')':
                        expr.Add(new ExpressionChunk(_source[charPosition].ToString(), ChunkTypes.ParenthesisRight));
                        charPosition++;
                        break;

                    default:
                        break;
                }

                if (knonwOperations.Contains(_source[charPosition].ToString()))
                {
                    expr.Add(new ExpressionChunk(_source[charPosition].ToString(), ChunkTypes.OperationChar));
                    charPosition++;
                }
            }

            return expr;
        }

        private bool ReadNumber(out double result)
        {
            result = 0.0;
            int curPos = charPosition;
            string value = "";
            //TODO: while digits are on charPos, add them to result. If one (and only one decimal point), use it
            //
            while (Char.IsDigit(_source[charPosition]) || (_source[charPosition] == '.'))
            {
                value += _source[charPosition];
                charPosition++;
            }
            try
            {
                result = Convert.ToDouble(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
