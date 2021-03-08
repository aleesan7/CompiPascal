using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Break : Instruction
    {
        private string type;

        public Break(string type)
        {
            this.type = type;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            return this.type;
        }
    }
}
