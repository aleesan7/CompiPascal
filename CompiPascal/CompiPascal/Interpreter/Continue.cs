using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Continue : Instruction
    {
        private string type;

        public Continue(string type)
        {
            this.type = type;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            return this.type;
        }

        public override string executeTranslate(Environment env)
        {
            return "continue;";
        }
    }
}
