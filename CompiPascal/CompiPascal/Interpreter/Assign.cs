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
            this.results = new LinkedList<string>();
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
            Symbol tempVar = env.ObtainVariable(this.id);
            Symbol value = this.value.evaluate(env);
            
            if(tempVar.type.type == value.type.type) 
            {
                env.assignVariableValue(this.id, this.value.evaluate(env));
            }
            else 
            {
                throw new CompiPascal.Utils.PascalError(tempVar.line, tempVar.column, "The assignation can´t be done since you´re trying to assign a " + value.type.type + " value (" + value.value.ToString() + ") to a " + tempVar.type.type + " variable (" + tempVar.id + ")", "Semantic");
            }
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            Symbol tempVar = env.ObtainVariable(this.id);
            string value = this.value.evaluateTranslate(env);

            return this.id + " := " + value + ";" + System.Environment.NewLine;
        }
    }
}
