using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Environment
    {
        Dictionary<string, Symbol> variables;
        Dictionary<string, Function> functions;
        Dictionary<string, Procedure> procedures;
        Dictionary<string, object> structs;
        Environment parent;
        private string environmentName;

        public Environment(Environment parent)
        {
            this.parent = parent;
            this.variables = new Dictionary<string, Symbol>();
            this.functions = new Dictionary<string, Function>();
            this.procedures = new Dictionary<string, Procedure>();
        }

        public string GetEnvName() 
        {
            return this.environmentName;
        }

        public void SetEnvName(string name) 
        {
            this.environmentName = name;
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
                Environment globalEnv = this.GetGlobalEnvironment();

                if (globalEnv.variables.ContainsKey(id)) 
                {
                    globalEnv.variables[id].value = value;
                }
                else 
                {
                    throw new Exception("The variable " + id + " doesn´t exist, so an assignation isn´t possible.");
                }
            }
        }

        public void AssignArrayValue(string id, object value, LinkedList<int> indexes) 
        {
            Environment actual = this;
            while (actual != null)
            {
                if (actual.variables.ContainsKey(id)) 
                {
                    actual.variables[id].SetValue(value, indexes);
                    return;
                }
                actual = actual.parent;
            };

            //foreach (KeyValuePair<string, Symbol> variable in this.variables)
            //{
            //    if (variable.Key.ToString().Equals(id)) 
            //    {
            //        variable.Value.SetValue(value, indexes);
            //        return;
            //    }
            //}

            throw new CompiPascal.Utils.PascalError(0, 0, "The variable " + id + " doesn´t exist in the current environment, and for that reason "
                + "a value cannot be assigned.", "Semantic");

        }

        public Symbol GetArrayValue(string id, LinkedList<int> indexes)
        {
            Environment actual = this;
            while (actual != null)
            {
                if (actual.variables.ContainsKey(id)) 
                {
                    Symbol tempSymbol = this.ObtainVariable(id);
                    Symbol symbolToReturn = new Symbol(tempSymbol.GetValue(id, indexes), tempSymbol.type, tempSymbol.id, tempSymbol.line, tempSymbol.column);
                    
                    return symbolToReturn;
                }
                actual = actual.parent;
            };

            //foreach (KeyValuePair<string, Symbol> variable in this.variables)
            //{
            //    if (variable.Key.ToString().Equals(id))
            //    {
            //        Symbol tempSymbol = this.ObtainVariable(id);
            //        Symbol symbolToReturn = new Symbol(variable.Value.GetValue(id, indexes), tempSymbol.type, tempSymbol.id, tempSymbol.line, tempSymbol.column);

            //        return symbolToReturn;
                    
            //    }
            //}

            throw new CompiPascal.Utils.PascalError(0, 0, "The variable " + id + " doesn´t exist in the current environment, and for that reason "
                + "a value cannot be accessed.", "Semantic");

            
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

        public Dictionary<string, Symbol> GetVariables() 
        {
            return this.variables;
        }

        public Dictionary<string, Function> GetFunctions() 
        {
            return this.functions;
        }

        public Dictionary<string, Procedure> GetProcedures()
        {
            return this.procedures;
        }

        public Environment GetParent() 
        {
            return this.parent;
        }

        public bool VariableExists(string id)
        {
            return false;
        }

        public void AddFunction(string id, Function func) 
        {
            if (!this.functions.ContainsKey(id))
            {
                this.functions.Add(id, func);
            }
            else
            {
                throw new Exception("The function " + id + " already exists in the current environment.");
            }
        }

        public Function ObtainFunction(string id) 
        {
            Environment actual = this;
            while (actual != null)
            {
                if (actual.functions.ContainsKey(id))
                    return actual.functions[id];
                actual = actual.parent;
            };
            return null;
        }

        public void AddProcedure(string id, Procedure proc)
        {
            if (!this.procedures.ContainsKey(id))
            {
                this.procedures.Add(id, proc);
            }
            else
            {
                throw new Exception("The procedure " + id + " already exists in the current environment.");
            }
        }

        public Procedure ObtainProcedure(string id)
        {
            Environment actual = this;
            while (actual != null)
            {
                if (actual.procedures.ContainsKey(id))
                    return actual.procedures[id];
                actual = actual.parent;
            };
            return null;
        }

        public Environment GetGlobalEnvironment() 
        {
            Environment actual = this;
            while (actual.parent != null) 
            {
                actual = actual.parent;
            }
            return actual;
        }
    }
}
