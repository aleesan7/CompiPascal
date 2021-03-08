using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Declare : Instruction
    {
        
        private string id;
     
        private Expression value;

        public int line;
        public int column;
        
        public Declare(string id, Expression value, int line, int column)
        {
            this.id = id;
            this.value = value;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
        }

        public string GetId() 
        {
            return this.id;
        }
        public override object execute(Environment env)
        {
            Symbol variable = this.value.evaluate(env);
            variable.id = this.id;
            variable.line = this.line;
            variable.column = this.column;
            env.declareVariable(this.id, variable);
            return null;
        }
    }
}
