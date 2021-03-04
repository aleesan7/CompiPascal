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

        public string GetId() 
        {
            return this.id;
        }
        public override object execute(Environment env)
        {
            Symbol variable = this.value.evaluate(env);
            variable.id = this.id;
            env.declareVariable(this.id, variable);
            return null;
        }
    }
}
