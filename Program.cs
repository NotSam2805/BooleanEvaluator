using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace BooleanEvaluator
{
    class Program
    {
        public const char notChar = '~';
        public const char orChar = '+';
        public const char andChar = '&';
        public const char impliesChar = '>';
        public const char equivalenceChar = '<';

        static void Main(string[] args)
        {
            char[] operationChars = { orChar, andChar, notChar, impliesChar, equivalenceChar, '(', ')' };

            var table = "";
            while (true)
            {
                var ans = Console.ReadLine();

                if(ans == "quit")
                {
                    break;
                }

                var expression = new Expression(ans, 't');

                var inputs = new List<char>();
                foreach (char c in ans)
                {
                    if (!operationChars.Contains(c) && !inputs.Contains(c))
                    {
                        inputs.Add(c);
                    }
                }

                Console.WriteLine();
                table = GetTruthTable(expression, inputs.ToArray(), ans);
                Console.Write(table);
                Console.WriteLine();
            }


            /*
            foreach(char c in inputs)
            {
                Console.WriteLine("Set Input " + c);
                expression.SetInput(c, Convert.ToBoolean(Console.ReadLine()));
            }

            Console.WriteLine(expression.Evaluate());
            */

            //Console.WriteLine(expression.PrintExpression());

            

            Console.WriteLine();
            Console.WriteLine("Save truth table?");
            var ans1 = Console.ReadLine();
            if(ans1 == "y")
            {
                Console.WriteLine("File name: ");
                using (StreamWriter outputFile = new StreamWriter(Console.ReadLine() + ".csv"))
                {
                    outputFile.Write(TruthTableToCSV(table));
                }
            }
        }

        static string GetTruthTable(Expression expression, char[] inputs, string expressionString)
        {
            string table = "";
            foreach(char i in inputs)
            {
                table += i + "|";
            }
            table += expressionString;

            table += "\n";

            var combos = GetEveryCombo(inputs.Length);

            for(int i = 0; i < combos.Length; i++)
            {
                for(int j = 0; j < combos[i].Length; j++)
                {
                    expression.SetInput(inputs[j], combos[i][j]);
                    if (expression.GetInput(inputs[j]))
                    {
                        table += "1|";
                    }
                    else
                    {
                        table += "0|";
                    }
                }

                if (expression.Evaluate())
                {
                    table += "1";
                }
                else
                {
                    table += "0";
                }
                table += "\n";
            }

            return table;
        }

        static bool[][] GetEveryCombo(int n)
        {
            if(n <= 0)
            {
                throw new Exception("Cannot have combinations of less than 1");
            }

            if(n == 1)
            {
                bool[][] res = new bool[2][];
                res[0] = new bool[1];
                res[1] = new bool[1];
                res[0][0] = true;
                res[1][0] = false;
                return res;
            }

            bool[][] result = new bool[(int)Math.Pow(2, n)][];
            var temp = GetEveryCombo(n - 1);
            for(int i = 0; i < temp.Length; i++)
            {
                result[i] = new bool[n];
                result[i][0] = false;
                for(int j = 0; j < temp[i].Length; j++)
                {
                    result[i][j + 1] = temp[i][j];
                }
            }

            for(int i = temp.Length; i < result.Length; i++)
            {
                result[i] = new bool[n];
                result[i][0] = true;
                for (int j = 0; j < temp[i - temp.Length].Length; j++)
                {
                    result[i][j + 1] = temp[i - temp.Length][j];
                }
            }

            return result;
        }

        static string TruthTableToCSV(string truthTable)
        {
            var lines = truthTable.Split('\n');
            var list = new List<string[]>();
            var result = "";
            for(int a = 0; a < lines.Length; a++)
            {
                string[] elements = lines[a].Split('|');
                for(int b = 0; b < elements.Length; b++)
                {
                    result += elements[b] + ",";
                }
                result.Remove(result.Length - 1);
                result += "\n";
            }

            result.Remove(result.Length - 1);

            return result;
        }
    }
}
