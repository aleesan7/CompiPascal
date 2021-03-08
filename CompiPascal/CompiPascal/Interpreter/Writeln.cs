using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Writeln : Instruction
    {

        private LinkedList<Expression> content;

        public Writeln(LinkedList<Expression> content)
        {
            this.content = content;
            this.results = new LinkedList<string>();
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

            results.AddLast(finalResult);
            System.Diagnostics.Debug.WriteLine(finalResult);
            return null;
        }
    }
}
