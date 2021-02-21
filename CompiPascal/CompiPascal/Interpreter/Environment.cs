using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Environment
    {
        Dictionary<string, Symbol> variables;
        Dictionary<string, object> funciones;
        Dictionary<string, object> structs;
        Environment parent;

        public Environment(Environment parent)
        {
            this.parent = parent;
            this.variables = new Dictionary<string, Symbol>();
        }

        public void declareVariable(string id, Symbol variable)
        {
            if (!this.variables.ContainsKey(id))
            {
                this.variables.Add(id, variable);
            }
            else
            {
                throw new Exception("The varaible " + id + " already exists in the current environment.");
            }
        }

        public void assignVariableValue(string id, object value) 
        {
            if (this.variables.ContainsKey(id))
            {
                this.variables[id].value = value;
            }
            else 
            {
                throw new Exception("The variable " + id + " doesn´t exist in the current environment, so an assignation isn´t possible.");
            }
        }

        public Symbol ObtainVariable(string id)
        {
            Environment actual = this;
            while (actual != null)
            {
                if (actual.variables.ContainsKey(id))
                    return actual.variables[id];
                actual = actual.parent;
            };
            return null;
        }

        public bool VariableExists(string id)
        {
            return false;
        }
    }
}
