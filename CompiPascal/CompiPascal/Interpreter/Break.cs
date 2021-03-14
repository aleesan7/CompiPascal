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
        }

        public override object execute(Environment env)
        {
            return this.type;
        }

        public override string executeTranslate(Environment env)
        {
            return "break;";
        }
    }
}
