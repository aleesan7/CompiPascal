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

        public string GetId() 
        {
            return this.id;
        }

        public Expression GetValue() 
        {
            return this.value;
        }

        public string GetValue(Environment env) 
        {
            return this.value.evaluate(env).ToString();
        }

        public override object execute(Environment env)
        {
            env.assignVariableValue(this.id, this.value.evaluate(env));
            return null;
        }
    }
}
