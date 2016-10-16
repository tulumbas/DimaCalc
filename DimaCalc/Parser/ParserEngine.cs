using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Parser
{
    public class ParserEngine
    {
        string _source;
        int _charPosition, _length;
        Dictionary<string, ChunkTypes> _knownOperations;
        string _knownCharacters;

        #region Class interface
        public ParserEngine()
        {
            _knownOperations = new Dictionary<string, ChunkTypes>();
            _knownOperations.Add("-", ChunkTypes.Minus);
            _knownOperations.Add("+", ChunkTypes.Plus);
            _knownOperations.Add("*", ChunkTypes.Multiply);
            _knownOperations.Add(":", ChunkTypes.Divide);
            _knownOperations.Add("=", ChunkTypes.Equals);
            _knownOperations.Add(":=", ChunkTypes.Assign);
            _knownOperations.Add(">", ChunkTypes.Greater);
            _knownOperations.Add(">=", ChunkTypes.GreaterOrEqual);
            _knownOperations.Add("<", ChunkTypes.Less);
            _knownOperations.Add("<=", ChunkTypes.LessOrEqual);

            var sb = new StringBuilder();
            foreach (var key in _knownOperations.Keys)
            {
                sb.Append(key);
            }
            _knownCharacters = sb.ToString();
        }

        public List<ExpressionChunk> Parse(string source)
        {
            Initialize(source);
            var expr = new List<ExpressionChunk>();
            string asText;

            do
            {
                char c = CurrentChar();
                if (Char.IsWhiteSpace(c))
                {
                    c = NextCharSkipWhitespaces();
                }

                if (Char.IsDigit(c))
                {
                    double number = ReadANumber(out asText);
                    expr.Add(new ExpressionChunk(asText, ChunkTypes.Number, number));
                }
                else if (_knownCharacters.Contains(c))
                {
                    var type = ReadOperation(out asText);
                    expr.Add(new ExpressionChunk(asText, type));
                }
                else if (c == '(')
                {
                    expr.Add(new ExpressionChunk("(", ChunkTypes.ParenthesisLeft));
                    NextChar(); // since there is no "Read..." operation for parenthesis, I need to skip a char
                }
                else if (c == ')')
                {
                    expr.Add(new ExpressionChunk(")", ChunkTypes.ParenthesisRight));
                    NextChar(); // since there is no "Read..." operation for parenthesis, I need to skip a char
                }
                else
                {
                    throw CreateASyntaxError("Unsupported character or syntax error " + c);
                }
            } while (IsNotEOL());

            expr.Add(new ExpressionChunk("", ChunkTypes.EOL));

            //if (_source[_charPosition] == ' ')
            //{
            //    _charPosition++;
            //}

            //switch (_source[_charPosition])
            //{
            //    //case ' ':
            //    //    _charPosition++;
            //    //    break;

            //    case '(':
            //        expr.Add(new ExpressionChunk("(", ChunkTypes.ParenthesisLeft));
            //        //_charPosition++; 
            //        break;

            //    case ')':
            //        expr.Add(new ExpressionChunk(")", ChunkTypes.ParenthesisRight));
            //        //_charPosition++;
            //        break;

            //    default:
            //        break;
            //}

            //if (knonwOperations.Contains(_source[_charPosition].ToString()))
            //{
            //    expr.Add(new ExpressionChunk(_source[_charPosition].ToString(), ChunkTypes.OperationChar));
            //    // increment in the end of loop
            //    //_charPosition++;
            //}

            //c = NextCharSkipWhitespaces();

            return expr;
        }
        #endregion

        /// <summary>
        /// Resets required fields
        /// </summary>
        /// <param name="source">Expression to be parsed</param>
        private void Initialize(string source)
        {
            _source = source;
            _charPosition = 0;
            _length = source.Length;
        }

        /// <summary>
        /// Allows to save the beginning of the substring
        /// </summary>
        /// <returns></returns>
        private int StorePosition()
        {
            return _charPosition;
        }

        /// <summary>
        /// Just check we havent reached the end of _source
        /// </summary>
        /// <returns></returns>
        private bool IsNotEOL()
        {
            return _charPosition < _length;
        }

        /// <summary>
        /// Returns current char or 0
        /// </summary>
        /// <returns>Char atcurrent charpos if before EOL or 0.</returns>
        private char CurrentChar()
        {
            return IsNotEOL() ? _source[_charPosition] : '\0';
        }

        /// <summary>
        /// Returns next char if available. Increases currentpos
        /// </summary>
        /// <returns>Char at next position if before end of line, 0 otherwise.</returns>
        private char NextChar()
        {
            if(_charPosition + 1 >= _length)
            {
                _charPosition = _length;
                return '\0';
            }
            return _source[++_charPosition];
        }

        /// <summary>
        /// Skips all whitespaces until reaches non-blank char.
        /// </summary>
        /// <returns>First non-whitespace char if available or 0 if reaches end of line</returns>
        private char NextCharSkipWhitespaces()
        {
            while(IsNotEOL() && char.IsWhiteSpace(_source[_charPosition]))
            {
                _charPosition++;
            }
            return CurrentChar();
        }

        /// <summary>
        /// Just a helper method to avoid using _charPosition directly in Read* functions.
        /// For some reason I don't want that
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Exception CreateASyntaxError(string message)
        {
            return new SyntaxException(message, _charPosition);
        }

        /// <summary>
        /// Returns part of the source
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string Substring(int start, int end)
        {
            return end < _length ?
                _source.Substring(start, end - start + 1) :
                _source.Substring(start, _length - start + 1);
        }

        /// <summary>
        /// Reads number starting from current position. Number consists of decimal digits and 1 decimal point at most.
        /// Number starts with decimal digit. Currently doesnt support: scientific notation, complex numbers.
        /// Method is called on a first digit and ends up after it succesfully read all of it with a current 
        /// position pointing on first char after.
        /// </summary>
        /// <param name="numberAsText">Returns parsed text</param>
        /// <returns></returns>
        private double ReadANumber(out string numberAsText)
        {
            double result = 0.0;
            // position of first char of number
            int startOf = StorePosition();
            // if dot was found
            bool hasDotAlready = false, isFirstChar = true;
            // first char
            char c = CurrentChar();
            do // assuming current position is first one to check
            {
                if (!char.IsDigit(c))
                {
                    if (isFirstChar) // check the first char
                    {
                        throw CreateASyntaxError("Number should start from digit (" + c + ")");
                    }

                    if (c == '.')
                    {
                        if (hasDotAlready)
                        {
                            throw CreateASyntaxError("Second decimal dot in a number not allowed");
                        }

                        hasDotAlready = true;
                    }
                    else // not a dot, not a number - we reached the end
                    {
                        break; // exit the loop
                    }
                }

                c = NextChar(); // take next char from source
                isFirstChar = false;

            } while (IsNotEOL()); // exit if the source reached the end

            // so now we know where the number starts and ends
            var endOf = IsNotEOL() ? StorePosition() - 1 : _length - 1;

            // get the number as a substring
            numberAsText = Substring(startOf, endOf);
                       
            if(!double.TryParse(numberAsText, out result))
            {
                // here we specify beginning of the number string
                throw new SyntaxException("Could not parse a number", startOf); 
            }

            // we are happy
            return result;
        }

        /// <summary>
        /// Reads an operator of any possible length. 1 char (+,-...); 2 char (!=, ==); etc
        /// </summary>
        /// <param name="text">textual representation</param>
        /// <returns>Encoded type</returns>
        private ChunkTypes ReadOperation(out string opAsText)
        {
            // first char of an operation
            var c = CurrentChar();
            var startOf = StorePosition();
            do
            {
                if(!_knownCharacters.Contains(c))
                {
                    break;
                }
                c = NextChar();
            } while (IsNotEOL());

            // so now we know where the operator starts and ends
            var endOf = IsNotEOL() ? StorePosition() - 1 : _length - 1;

            // get the number as a substring
            opAsText = Substring(startOf, endOf);

            ChunkTypes type;
            if(!_knownOperations.TryGetValue(opAsText, out type))
            {
                // here we specify beginning of the number string
                throw new SyntaxException("Unknown operator " + opAsText, startOf);
            }

            return type;
        }

        private bool ReadNumber(out double result)
        {
            result = 0.0;
            int curPos = _charPosition;
            string value = "";
            //TODO: while digits are on charPos, add them to result. If one (and only one decimal point), use it
            //
            while (Char.IsDigit(_source[_charPosition]) || (_source[_charPosition] == '.'))
            {
                value += _source[_charPosition];
                _charPosition++;
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
