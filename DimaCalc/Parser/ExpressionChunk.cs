using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DimaCalc.Parser
{
    public class ExpressionChunk
    {
        public string OriginalText { get; private set; }
        public ChunkTypes ChunkType { get; private set; }
        public double Value { get; private set; }

        public ExpressionChunk(string originalText, ChunkTypes type)
        {
            OriginalText = originalText;
            Value = 0.0;
            ChunkType = type;
        }

        public ExpressionChunk(string originalText, ChunkTypes type, double value)
        {
            OriginalText = originalText;
            Value = value;
            ChunkType = type;
        }
    }

    public enum ChunkTypes
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        Equals,
        ParenthesisLeft,
        ParenthesisRight,
        EOL,
        Assign,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual
    }

}
