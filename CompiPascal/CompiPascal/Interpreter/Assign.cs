using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Assign : Instruction
    {
        private string id;
        private Expression value;

        public Assign(string id, Expression value)
        {
            this.id = id;
            this.value = value;
        }

        public override object execute(Environment env)
        {
            env.assignVariableValue(this.id, value.evaluate(env));
            return null;
        }
    }
}
