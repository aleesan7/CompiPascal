using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Writeln : Instruction
    {

        private LinkedList<Expression> content;

        public Writeln(LinkedList<Expression> content)
        {
            this.content = content;
        }

        public override object execute(Environment env)
        {
            string finalResult = "";
            Symbol toWrite;
            foreach (Expression expression in this.content) 
            {
                toWrite = expression.evaluate(env);
                finalResult = finalResult + toWrite.value.ToString();
            }
            export(finalResult);

            System.Diagnostics.Debug.WriteLine(finalResult);
            Console.WriteLine(finalResult);
            return null;
        }

        public override string executeTranslate(Environment env) 
        {
            string finalResult = "writeln(";
            foreach (Expression expression in this.content)
            {
                finalResult = finalResult + expression.evaluateTranslate(env) + ",";
            }

            finalResult = finalResult.Substring(0, finalResult.Length - 1);

            finalResult = finalResult + ");" + System.Environment.NewLine;

            System.Diagnostics.Debug.WriteLine(finalResult);

            return  finalResult;
        }

        public void export(string content)
        {
            string path = "results.txt";
            try
            {

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, content + System.Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(path, content + System.Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
