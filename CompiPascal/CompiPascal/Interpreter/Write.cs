using System;
using System.Collections.Generic;
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
                finalResult = finalResult + toWrite.value.ToString();
            }
            System.Diagnostics.Debug.Write(finalResult);
            return null;
        }
    }
}
