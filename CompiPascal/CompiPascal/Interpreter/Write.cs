using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Write : Instruction
    {
        private LinkedList<Expression> content;

        public Write(LinkedList<Expression> content)
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
                export(toWrite.value.ToString());
                finalResult = finalResult + toWrite.value.ToString();
            }

            System.Diagnostics.Debug.Write(finalResult);
            Console.Write(finalResult);
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            string finalResult = "write(";
            foreach (Expression expression in this.content)
            {
                finalResult = finalResult + expression.evaluateTranslate(env) + ",";
            }

            finalResult = finalResult.Substring(0, finalResult.Length - 1);

            finalResult = finalResult + ");" + System.Environment.NewLine;

            System.Diagnostics.Debug.Write(finalResult);

            return finalResult;
        }

        public void export(string content)
        {
            string path = "results.txt";
            try
            {

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, content);
                }
                else
                {
                    File.AppendAllText(path, content);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
