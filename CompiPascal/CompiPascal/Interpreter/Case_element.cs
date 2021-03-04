using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Case_element : Instruction
    {
        private Expression condition;
        private Instruction instruction;

        public Case_element(Expression condition, Instruction instruction)
        {
            this.condition = condition;
            this.instruction = instruction;
        }

        public Expression GetCondition() 
        {
            return this.condition;
        }

        public void SetCondition(Expression newCond) 
        {
            this.condition = newCond;
        }

        public override object execute(Environment env)
        {
            try
            {
                Symbol value = this.condition.evaluate(env);

                //TODO verificar errores
                if (value.type.type != Types.BOOLEAN)
                    throw new PascalError(0, 0, "The condition for the if isn´t boolean", "Semantic");

                if (bool.Parse(value.value.ToString()))
                {
                    try
                    {
                        object val = this.instruction.execute(env);

                        if (val != null)
                        {
                            if (val.ToString().ToLower().Equals("break"))
                            {
                                return val;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }
    }
}
