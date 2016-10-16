using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DimaCalc.Operations;

namespace DimaCalc.Parser
{
    /// <summary>
    /// Напишу по-русски. Этот класс берет коллекцию входных элементов, и, в соответствии с правилами нашей 
    /// грамматики, составляет вычислмые выражения, потомки Operation.
    /// </summary>
    public class OperationBuilder
    {
        #region Переменные и типы
        List<ExpressionChunk> _source;
        int _chunkPosition;
        #endregion

        /// <summary>
        /// Вся логика - в private методах
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Operation Build(List<ExpressionChunk> source)
        {
            Initialize(source);
            if(!IsEOL)
            {
                return ReadOperation();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Квадратные скобки означают, что эта часть может существовать, а может и нет.
        /// Квадратные скобки со звездочкой - что часть может повторяться 0 или более раз.
        /// 
        /// Операция := ВыражениеПриоритета_2 [ ЗнакПриоритета_1 ВыражениеПриоритета_2 ]*; 
        /// ВыражениеПриоритета_2 := ВыражениеПриоритета_3 [ ЗнакПриоритета_2 ВыражениеПриоритета_3]*; 
        /// ВыражениеПриоритета_3 := ['-'] Число ИЛИ ОперацияСоСкобками
        /// 
        /// ОперацияСоСкобками := '(' Операция ')'
        /// 
        /// ЗнакПриоритета1 := '+' ИЛИ '-'
        /// ЗнакПриоритета2 := '*' ИЛИ ':'
        /// </summary>
        /// <returns></returns>
        private Operation ReadOperation()
        {
            // читаем левую часть - она должна быть выше приоритетом, чем + или -
            Operation left, right;
            left = ReadPriority2();            
            while(true)
            {
                if(Is(ChunkTypes.Minus))
                {
                    // переходим к следующему куску и читаем вес_2
                    MoveNext(); 
                    right = ReadPriority2();
                    // готовое выражение - теперь новая левая часть
                    left = new BinaryOperation(left, right, MathOperations.Subtract);
                } 
                else if (Is(ChunkTypes.Plus))
                {
                    MoveNext();
                    right = ReadPriority2();
                    left = new BinaryOperation(left, right, MathOperations.Add);
                }
                else
                {
                    // если плюсы и минусы кончились, выходим
                    break;
                }
            }

            return left;
        }

        /// <summary>
        /// Абсолютно та же логика, что и ReadOperation, только с * и :
        /// </summary>
        /// <returns></returns>
        private Operation ReadPriority2()
        {
            // читаем левую часть - она должна быть выше приоритетом, чем * или :
            Operation left, right;
            left = ReadPriority3();
            while (true)
            {
                if (Is(ChunkTypes.Multiply))
                {
                    // переходим к следующему куску и читаем вес_3
                    MoveNext();
                    right = ReadPriority3();
                    // готовое выражение - теперь новая левая часть
                    left = new BinaryOperation(left, right, MathOperations.Multiply);
                }
                else if (Is(ChunkTypes.Divide))
                {
                    MoveNext();
                    right = ReadPriority3();
                    left = new BinaryOperation(left, right, MathOperations.Divide);
                }
                else
                {
                    // если * и : кончились, выходим
                    break;
                }
            }

            return left;
        }

        /// <summary>
        /// Читаем или выражение в скобках или число. Перед ними модет стоять минус, тогда - отрицание.
        /// </summary>
        /// <returns></returns>
        private Operation ReadPriority3()
        {
            var doNegate = false; 
            if (Is(ChunkTypes.Minus))
            {
                // отбрасывваем
                Eat(ChunkTypes.Minus);
                doNegate = true;
            }

            // теперь только число или скобки
            Operation op;
            if (Is(ChunkTypes.ParenthesisLeft))
            {
                // скобки просто гарантируют порядок создания операций, сами по себе они на на фиг не нужны 
                Eat(ChunkTypes.ParenthesisLeft);
                op = ReadOperation();
                Eat(ChunkTypes.ParenthesisRight);

            }
            else
            {
                // ReadConstant сожрет число. Если там не число, будет ошибка
                op = ReadConstantOperation();
            }

            return doNegate ? new NegateOperation(op) : op;
        }

        Operation ReadConstantOperation()
        {
            var number = Eat(ChunkTypes.Number);
            return new ConstantOperation(number.Value);
        }

        #region Helper methods

        private void Initialize(List<ExpressionChunk> source)
        {
            _source = source;
            _chunkPosition = 0;
        }


        /// <summary>
        /// Проверяет тип текущего куска. 
        /// Просто для упрощения выражений грамматики и уменьшения количества переменных
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Is(ChunkTypes type)
        {
            return _chunkPosition < _source.Count && _source[_chunkPosition].ChunkType == type;
        }

        /// <summary>
        /// По уму, должен проерять, не закончилась ли цепочка
        /// </summary>
        bool IsEOL
        {
            get
            {
                return Is(ChunkTypes.EOL);
            }
        }

        /// <summary>
        /// Вернуть true если текущий кусок принадлежит к одному из типов. Как Is, только несколько аргументов.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        bool IsIn(params ChunkTypes[] types)
        {
            if (_chunkPosition < _source.Count)
            {
                foreach (var type in types)
                {
                    if (_source[_chunkPosition].ChunkType == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет тип следущего куска без прибаления индекса. 
        /// Просто для упрощения выражений грамматики и уменьшения количества переменных.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsNext(ChunkTypes type)
        {
            return !IsEOL && _source[_chunkPosition + 1].ChunkType == type;
        }

        /// <summary>
        /// Требует, чтобы тип текущего куска совпадал с type и "сжирает" его.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Проглоченный кусок</returns>
        ExpressionChunk Eat(ChunkTypes type)
        {
            if (!Is(type))
            {
                throw new SyntaxException("Expected " + type, 0);
            }

            var ch = _source[_chunkPosition];
            MoveNext();
            return ch;
        }

        /// <summary>
        /// перемещается на 1, если еще можно
        /// </summary>
        void MoveNext()
        {
            if (!IsEOL)
            {
                ++_chunkPosition;
            }
        }

        #endregion
    }
}
