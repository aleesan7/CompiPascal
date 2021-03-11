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

        public override string executeTranslate(Environment env)
        {
            Symbol variable = this.value.evaluate(env);
            string result = string.Empty;

            if (variable.type.type == Types.STRING && variable.value.ToString().Equals("")) 
            {
                result = "var " + this.id + " : " + variable.type.type.ToString().ToLower() + " = " + "''" + ";";
            }
            else 
            {
                result = "var " + this.id + " : " + variable.type.type.ToString().ToLower() + " = " + this.value.evaluateTranslate(env) + ";";
            }

            result += System.Environment.NewLine;
            
            return result;
        }
    }
}
