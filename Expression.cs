using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooleanEvaluator
{
    public class Expression
    {
        public const char notChar = '~';
        public const char orChar = '+';
        public const char andChar = '&';
        public const char impliesChar = '>';
        public const char equivalenceChar = '<';
        public char[] operationChars;

        public char name;

        private Input[] inputs;
        private Expression[] innerExpressions;
        private string expressionString;

        public Expression(string expressionString_, char name_)
        {
            char[] temp = { orChar, andChar, notChar, impliesChar, equivalenceChar};
            operationChars = temp;

            expressionString = expressionString_;

            CreateInputs();
            CreateInners();

            name = name_;
        }

        private void CreateInners()
        {
            innerExpressions = new Expression[0];
            var inners = new List<Expression>();
            int closingCount = 0;
            bool waiting = false;
            int startIndex = -1;
            for(int i = 0; i < expressionString.Length; i++)
            {
                char c = expressionString[i];
                if(c == '(')
                {
                    closingCount++;
                    if (!waiting)
                    {
                        waiting = true;
                        startIndex = i;
                    }
                }
                else if(c == ')')
                {
                    closingCount--;
                    if(waiting && (closingCount == 0))
                    {
                        var length = i - startIndex;
                        var thisInner = new Expression(expressionString.Substring(startIndex + 1, length - 1), CreateInnerName());
                        inners.Add(thisInner);
                        expressionString = expressionString.Substring(0, startIndex) + thisInner.name + expressionString.Substring(i + 1);
                        i = 0;
                        waiting = false;

                        innerExpressions = inners.ToArray();
                    }
                }
            }

            innerExpressions = inners.ToArray();
        }

        private char CreateInnerName()
        {
            char newName = (char)(inputs[0].GetName() + 1);
            var inputNames = new List<char>();
            foreach(Input i in inputs)
            {
                inputNames.Add(i.GetName());
            }
            foreach(Expression i in innerExpressions)
            {
                inputNames.Add(i.name);
            }

            while (inputNames.Contains(newName) || operationChars.Contains(newName) || (newName == notChar))
            {
                inputNames.Add(newName);
                newName = (char)(newName + 1);
            }

            return newName;
        }

        private void CreateInputs()
        {
            var inputsList = new List<Input>();
            var charList = new List<char>();
            foreach(char c in expressionString)
            {
                if (!operationChars.Contains(c) && !charList.Contains(c))
                {
                    inputsList.Add(new Input(c));
                    charList.Add(c);
                }
            }

            inputs = inputsList.ToArray();
        }

        public bool Evaluate()
        {
            bool lastInput = false;
            bool isNot = false;
            char nextOp = 'X';
            char c;
            for(int i = 0; i < expressionString.Length; i++)
            {
                c = expressionString[i];
                var inpIndex = FindInput(c);

                if (c == notChar)
                {
                    isNot = true;
                    i++;
                }
                else
                {
                    isNot = false;
                }

                if (operationChars.Contains(c))
                {
                    nextOp = c;
                }
                else if(inpIndex >= 0)
                {
                    if (operationChars.Contains(nextOp))
                    {
                        return CompleteOp(nextOp, lastInput, inputs[inpIndex].state);
                    }
                    else
                    {
                        lastInput = inputs[inpIndex].state;
                        if (isNot)
                        {
                            lastInput = !lastInput;
                            isNot = false;
                        }
                    }
                }
                else if (FindExpression(c) >= 0)
                {
                    if (operationChars.Contains(nextOp))
                    {
                        return CompleteOp(nextOp, lastInput, innerExpressions[FindExpression(c)].Evaluate());
                    }
                    else
                    {
                        lastInput = innerExpressions[FindExpression(c)].Evaluate();
                        if (isNot)
                        {
                            lastInput = !lastInput;
                            isNot = false;
                        }
                    }
                }
                else
                {
                    throw new Exception("Expression format error");
                }
            }

            throw new Exception("Expression format error");
        }

        private int FindInput(char name)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                if(inputs[i].GetName() == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindExpression(char name)
        {
            for (int i = 0; i < innerExpressions.Length; i++)
            {
                if (innerExpressions[i].name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool CompleteOp(char op, bool inp1, bool inp2)
        {
            switch (op)
            {
                case andChar:
                    return (inp1 && inp2);
                    break;

                case orChar:
                    return (inp1 || inp2);
                    break;

                case impliesChar:
                    if(inp1 && (!inp2))
                    {
                        return false;
                    }
                    return true;
                    break;
                case equivalenceChar:
                    if(inp1 && inp2)
                    {
                        return true;
                    }
                    if((!inp1) && (!inp2))
                    {
                        return true;
                    }
                    return false;
                    break;
                default:
                    throw new Exception(op + " is not a recognised operation");
                    break;
            }
        }

        public void SetInput(char inputName, bool state)
        {
            if(FindInput(inputName) >= 0)
            {
                inputs[FindInput(inputName)].state = state;
            }
            for(int i = 0; i < innerExpressions.Length; i++)
            {
                innerExpressions[i].SetInput(inputName, state);
            }
        }

        public string PrintExpression()
        {
            var res = expressionString;

            for(int i = 0; i < innerExpressions.Length; i++)
            {
                res += "\n" + innerExpressions[i].PrintExpression();
            }

            return res;
        }

        public bool GetInput(char name)
        {
            return inputs[FindInput(name)].state;
        }
    }
}
