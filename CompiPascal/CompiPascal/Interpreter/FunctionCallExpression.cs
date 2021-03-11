using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompiPascal.Interpreter
{
    class FunctionCallExpression : Expression
    {
        private string id;
        private LinkedList<Expression> parameters;

        public FunctionCallExpression(string id)
        {
            this.id = id;
            this.parameters = new LinkedList<Expression>();
        }

        public FunctionCallExpression(string id, LinkedList<Expression> parameters)
        {
            this.id = id;
            this.parameters = parameters;
        }

        public override Symbol evaluate(Environment env)
        {
            Function func = env.ObtainFunction(this.id);
            Environment newEnv = new Environment(env.GetGlobalEnvironment());
            object val = null;

            if (func != null) 
            {
                
                newEnv.SetEnvName(this.id);

                switch (func.GetFunctionType())
                {
                    case Function.FunctionTypes.BOOLEAN:
                        newEnv.declareVariable(this.id, new Symbol(null, new Type(Types.BOOLEAN, null), this.id, 0, 0));
                        break;
                    case Function.FunctionTypes.INTEGER:
                        newEnv.declareVariable(this.id, new Symbol(null, new Type(Types.INTEGER, null), this.id, 0, 0));
                        break;
                    case Function.FunctionTypes.STRING:
                        newEnv.declareVariable(this.id, new Symbol(null, new Type(Types.STRING, null), this.id, 0, 0));
                        break;
                    case Function.FunctionTypes.REAL:
                        newEnv.declareVariable(this.id, new Symbol(null, new Type(Types.REAL, null), this.id, 0, 0));
                        break;

                }

                for (int i=0;i<this.parameters.Count; i++) 
                {
                    Symbol tempSymbol = this.parameters.ElementAt(i).evaluate(env);
                    newEnv.declareVariable(func.parameters.ElementAt(i).GetId(), tempSymbol);
                }

                //foreach(Expression expr in this.parameters) 
                //{
                //    Symbol tempSymbol = expr.evaluate(env);
                //    newEnv.declareVariable(this.id, tempSymbol);
                //}

                foreach(Instruction instruction in func.instructions) 
                {
                    val = instruction.execute(newEnv);

                    if (val != null)
                    {
                        return newEnv.ObtainVariable(this.id);
                    }
                }
            }
            return newEnv.ObtainVariable(this.id);
        }

        public override string evaluateTranslate(Environment env)
        {
            throw new NotImplementedException();
        }

        //public override Symbol evaluate(Environment env)
        //{
        //    bool isFunction = false;
        //    Access returnVar = null;
        //    Environment globalEnv = env.GetGlobalEnvironment();
        //    Environment funcOrProcEnv = new Environment(globalEnv);

        //    funcOrProcEnv.SetEnvName(this.id);

        //    if (globalEnv.ObtainFunction(this.id) != null)
        //    {
        //        isFunction = true;
        //    }
        //    else
        //    {
        //        //TODO let know that the call doesn´t correspond to neither a function nor a procedure
        //        throw new Exception("The call doesn´t correspond to a function.");
        //    }

        //    if (isFunction)
        //    {
        //        Function func = env.ObtainFunction(this.id);

        //        //We set up a var with the same name as the function in order to return a value
        //        switch (func.GetFunctionType()) 
        //        {
        //            case Function.FunctionTypes.BOOLEAN:
        //                funcOrProcEnv.declareVariable(this.id, new Symbol(null, new Type(Types.BOOLEAN, null), this.id, 0, 0));
        //                break;
        //            case Function.FunctionTypes.INTEGER:
        //                funcOrProcEnv.declareVariable(this.id, new Symbol(null, new Type(Types.INTEGER, null), this.id, 0, 0));
        //                break;
        //            case Function.FunctionTypes.STRING:
        //                funcOrProcEnv.declareVariable(this.id, new Symbol(null, new Type(Types.STRING, null), this.id, 0, 0));
        //                break;
        //            case Function.FunctionTypes.REAL:
        //                funcOrProcEnv.declareVariable(this.id, new Symbol(null, new Type(Types.REAL, null), this.id, 0, 0));
        //                break;

        //        }



        //        if (func.localVariables.Count > 0)
        //        {
        //            //pass the func variables to the new local environment
        //            foreach (var variable in func.localVariables)
        //            {
        //                variable.execute(funcOrProcEnv);
        //            }
        //        }

        //        //if we have passed parameters to the function
        //        if (this.parameters.Count > 0)
        //        {
        //            //if the amount of parameters than the function expects, it´s the same than the amount of supplied parameters
        //            if (func.parameters.Count == this.parameters.Count)
        //            {
        //                //We assign the values passed to the function
        //                func.SetValuesToParameters(this.parameters);

        //                LinkedList<Expression> paramValues = func.parameterValues;
        //                Assign assign;
        //                int paramPosition = 0;
        //                foreach (var parameter in func.parameters)
        //                {
        //                    //We declare the variable in the environment of the proc 
        //                    parameter.execute(funcOrProcEnv);

        //                    //now we assign the value to that parameter
        //                    //TODO validate if the types are valid or not
        //                    assign = new Assign(parameter.GetId(), paramValues.ElementAt(paramPosition));
        //                    assign.execute(funcOrProcEnv);

        //                    paramPosition++;
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("The amount of supplied parameters to the function doesn´t match the ones that the function expects.");
        //            }
        //        }

        //        //We obtain the id of the function
        //        string funcName = func.id;
        //        bool returnPresent = false;
        //        object val = null;
        //        Assign tempAssign;


        //        //Then we need to determine if the function has it´s 'return' statement, if not, we throw an exception indicating that a return is expected
        //        foreach (Instruction ins in func.instructions)
        //        {
        //            if (ins.GetType().Name.ToString().ToLower().Equals("assign"))
        //            {
        //                tempAssign = (Assign)ins;

        //                if (tempAssign.GetId().Equals(funcName))
        //                {
        //                    returnPresent = true;
        //                    tempAssign = null;
        //                    break;
        //                }
        //            }
        //        }

        //        if (!returnPresent)
        //        {
        //            throw new Exception("The function doesn´t have any return value specified, please add it.");
        //        }

        //        foreach (Instruction instruction in func.instructions)
        //        {
        //            val = instruction.execute(funcOrProcEnv);

        //            if (val != null) 
        //            {
        //                return funcOrProcEnv.ObtainVariable(this.id);
        //            }
        //        }
        //    }

        //    return funcOrProcEnv.ObtainVariable(this.id);
        //}
    }
}
