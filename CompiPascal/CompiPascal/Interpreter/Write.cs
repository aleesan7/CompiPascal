using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Write : Instruction
    {
        private Expression content;

        public Write(Expression content)
        {
            this.content = content;
        }

        public override object execute(Environment env)
        {
            Symbol toWrite = this.content.evaluate(env);
            System.Diagnostics.Debug.Write(toWrite.value.ToString());
            return toWrite.value;
        }
    }
}
