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
            bool isFunction = false;
            Access returnVar = null;
            Environment globalEnv = env.GetGlobalEnvironment();
            Environment funcOrProcEnv = new Environment(globalEnv);

            funcOrProcEnv.SetEnvName(this.id);

            if (globalEnv.ObtainFunction(this.id) != null)
            {
                isFunction = true;
            }
            else
            {
                //TODO let know that the call doesn´t correspond to neither a function nor a procedure
                throw new Exception("The call doesn´t correspond to a function.");
            }

            if (isFunction)
            {
                Function func = env.ObtainFunction(this.id);


                if (func.localVariables.Count > 0)
                {
                    //pass the func variables to the new local environment
                    foreach (var variable in func.localVariables)
                    {
                        variable.execute(funcOrProcEnv);
                    }
                }

                //if we have passed parameters to the function
                if (this.parameters.Count > 0)
                {
                    //if the amount of parameters than the function expects, it´s the same than the amount of supplied parameters
                    if (func.parameters.Count == this.parameters.Count)
                    {
                        //We assign the values passed to the function
                        func.SetValuesToParameters(this.parameters);

                        LinkedList<Expression> paramValues = func.parameterValues;
                        Assign assign;
                        int paramPosition = 0;
                        foreach (var parameter in func.parameters)
                        {
                            //We declare the variable in the environment of the proc 
                            parameter.execute(funcOrProcEnv);

                            //now we assign the value to that parameter
                            //TODO validate if the types are valid or not
                            assign = new Assign(parameter.GetId(), paramValues.ElementAt(paramPosition));
                            assign.execute(funcOrProcEnv);

                            paramPosition++;
                        }
                    }
                    else
                    {
                        throw new Exception("The amount of supplied parameters to the function doesn´t match the ones that the function expects.");
                    }
                }

                //We obtain the id of the function
                string funcName = func.id;
                bool returnPresent = false;
                object retVal = null;
                Assign tempAssign;
                Exit tempExit;

                //Then we need to determine if the function has it´s 'return' statement, if not, we throw an exception indicating that a return is expected
                foreach (Instruction ins in func.instructions)
                {
                    if (ins.GetType().Name.ToString().ToLower().Equals("assign"))
                    {
                        tempAssign = (Assign)ins;

                        if (tempAssign.GetId().Equals(funcName))
                        {
                            returnPresent = true;
                            tempAssign = null;
                            //returnDeclare = new Declare(funcName, tempAssign.GetValue());
                            //returnDeclare.execute(funcOrProcEnv);
                            //break;
                        }
                    }
                }

                if (!returnPresent)
                {
                    throw new Exception("The function doesn´t have any return value specified, please add it.");
                }

                foreach (Instruction instruction in func.instructions)
                {
                    if (instruction.GetType().Name.ToString().ToLower().Equals("assign"))
                    {
                        tempAssign = (Assign)instruction;

                        if (tempAssign.GetId().Equals(funcName))
                        {
                            //If we have reached the return, then we return the value
                            return tempAssign.GetValue().evaluate(funcOrProcEnv);
                        }
                        else 
                        {
                            instruction.execute(funcOrProcEnv);
                        }
                    }
                    else 
                    {
                        if (instruction.GetType().Name.ToString().ToLower().Equals("exit"))
                        {
                            tempExit = (Exit)instruction;

                            return tempExit.GetValue().evaluate(funcOrProcEnv);
                        }
                        else 
                        {
                            instruction.execute(funcOrProcEnv);
                        }
                    }
                }
            }

            return returnVar.evaluate(funcOrProcEnv);
        }
    }
}
