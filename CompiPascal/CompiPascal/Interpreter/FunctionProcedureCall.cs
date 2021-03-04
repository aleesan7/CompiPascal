﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompiPascal.Interpreter
{
    class FunctionProcedureCall : Instruction
    {
        private string id;
        private LinkedList<Expression> parameters;

        public FunctionProcedureCall(string id)
        {
            this.id = id;
            this.parameters = new LinkedList<Expression>();
        }

        public FunctionProcedureCall(string id, LinkedList<Expression> parameters) 
        {
            this.id = id;
            this.parameters = parameters;
        }

        public override object execute(Environment env)
        {
            bool isFunction = false;
            bool isProcedure = false;
            Environment globalEnv = env.GetGlobalEnvironment();
            Environment funcOrProcEnv = new Environment(globalEnv);
            
            if (globalEnv.ObtainFunction(this.id) != null)
            {
                isFunction = true;
            }
            else 
            {
                if (globalEnv.ObtainProcedure(this.id) != null)
                {
                    isProcedure = true;
                }
                else 
                {
                    //TODO let know that the call doesn´t correspond to neither a function nor a procedure
                    throw new Exception("The call doesn´t correspond to neither a function nor a procedure.");
                }
            }

            if (isFunction) 
            {
                Function func = env.ObtainFunction(this.id);


                if (func.localVariables.Count > 0)
                {
                    //pass the proc variables to the new local environment
                    foreach (var variable in func.localVariables)
                    {
                        variable.execute(funcOrProcEnv);
                    }
                }

                //if we have passed parameters to the procedure
                if (this.parameters.Count > 0)
                {
                    //if the amount of parameters than the procedure expects, it´s the same than the amount of supplied parameters
                    if (func.parameters.Count == this.parameters.Count)
                    {
                        //We assign the values passed to the procedure
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
                Assign tempAssign;

                //Then we need to determine if the function has it´s 'return' statement, if not, we throw an exception indicating that a return is expected
                foreach (Instruction ins in func.instructions)
                {
                    if (ins.GetType().Name.ToString().ToLower().Equals("assign"))
                    {
                        tempAssign = (Assign)ins;

                        if (tempAssign.GetId().Equals(funcName))
                        {
                            returnPresent = true;
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
                    instruction.execute(funcOrProcEnv);
                }
                

                
            }
            else
            {
                if (isProcedure) 
                {
                    Procedure proc = env.ObtainProcedure(this.id);

                    if(proc.localVariables.Count > 0) 
                    { 
                        //pass the proc variables to the new local environment
                        foreach (var variable in proc.localVariables)
                        {
                            variable.execute(funcOrProcEnv);
                        }
                    }

                    //if we have passed parameters to the procedure
                    if (this.parameters.Count > 0) 
                    {
                        //if the amount of parameters than the procedure expects, it´s the same than the amount of supplied parameters
                        if (proc.parameters.Count == this.parameters.Count)
                        {
                            //We assign the values passed to the procedure
                            proc.SetValuesToParameters(this.parameters);

                            LinkedList<Expression> paramValues = proc.parameterValues;
                            Assign assign;
                            int paramPosition = 0;
                            foreach (var parameter in proc.parameters) 
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
                            throw new Exception("The amount of supplied parameters to the procedure doesn´t match the ones that the procedure expects."); 
                        }
                    }

                    foreach (Instruction instruction in proc.instructions)
                    {
                        instruction.execute(funcOrProcEnv);
                    }
                }
            }
            
            return null;
        }
    }
}
