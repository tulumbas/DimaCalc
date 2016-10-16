using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Parser
{
    /// <summary>
    /// Class that reports an syntax exception in the input text expression. Tries to point the position in the string.
    /// </summary>
    public class SyntaxException: ApplicationException
    {

        /// <summary>
        /// Closest possible position to an error
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Constructor that passes the error messages to the base ApplicationException class and save the postion
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <param name="position">Position of an error within the string</param>
        public SyntaxException(string message, int position): base(message)
        {
            Position = position;
        }

        /// <summary>
        /// Constructor passes the real exception which happened during parsing
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <param name="position">Position of an error within the string</param>
        /// <param name="innerException">Original exception</param>
        public SyntaxException(string message, int position, Exception innerException) 
            : base(message, innerException)
        {
            Position = position;
        }
    }
}
