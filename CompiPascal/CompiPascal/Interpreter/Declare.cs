using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Declare : Instruction
    {
        
        private string id;
     
        private Expression value;
        public Type type;
        public bool constant = false;
        public bool isParameter = false;
        public int line;
        public int column;

        public Declare(string id, Expression value, Type type, bool constant, int line, int column)
        {
            this.id = id;
            this.value = value;
            this.type = type;
            this.constant = constant;
            this.line = line;
            this.column = column;
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
            if (this.constant) 
            {
                variable.constant = true;
            }
            else 
            {
                variable.constant = false;
            }
            env.declareVariable(this.id, variable);
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            Symbol variable = this.value.evaluate(env);
            string result = string.Empty;

            if (!isParameter)
            {
                if (variable.type.type == Types.STRING && variable.value.ToString().Equals(""))
                {
                    result = "var " + this.id + " : " + variable.type.type.ToString().ToLower() + " = " + "''" + ";";
                }
                else
                {
                    result = "var " + this.id + " : " + variable.type.type.ToString().ToLower() + " = " + this.value.evaluateTranslate(env) + ";";
                }
            }
            else 
            {
                result = this.id + " : " + variable.type.type.ToString().ToLower() + ";";
            }
            result += System.Environment.NewLine;

            env.declareVariable(this.id, variable);
            return result;
        }

        public void previous(Environment env)
        {
            env.declareVariable(this.id, new Symbol(this.value.evaluateTranslate(env), this.type, this.id, this.line, this.column));
        }
    }
}
