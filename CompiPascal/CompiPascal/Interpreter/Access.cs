using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Access : Expression
    {
        private string id;

        public Access(string id) 
        {
            this.id = id;
        }

        public override Symbol evaluate(Environment env)
        {
            Symbol variable = env.ObtainVariable(this.id);
            if (variable == null) 
            {
                Environment globalEnv = env.GetGlobalEnvironment();

                variable = globalEnv.ObtainVariable(this.id);

                if (variable == null) { 
                    throw new Exception("The variable '" + id + "' doesn´t exist.");
                }
                else 
                {
                    return variable;
                }
            }
            else 
            {
                return variable;
            }
        }

        public override string evaluateTranslate(Environment env)
        {
            throw new NotImplementedException();
        }
    }
}
