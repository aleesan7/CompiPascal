using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Exit : Instruction
    {
        private string type;
        private Expression value;

        public Exit(string type, Expression value)
        {
            this.type = type;
            this.value = value;
            this.results = new LinkedList<string>();
        }

        public Expression GetValue()
        {
            return this.value;
        }

        public override object execute(Environment env)
        {
            env.assignVariableValue(env.GetEnvName(), this.value.evaluate(env));
            return this.value.evaluate(env);
        }
    }
}
