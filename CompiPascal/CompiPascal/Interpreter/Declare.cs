using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Declare : Instruction
    {
        
        private string id;
     
        private Expression value;
        
        public Declare(string id, Expression value)
        {
            this.id = id;
            this.value = value;
        }


        public override object execute(Environment env)
        {
            Symbol variable = this.value.evaluate(env);
            env.declareVariable(this.id, variable);
            return null;
        }
    }
}
