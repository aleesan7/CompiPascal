using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Repeat : Instruction
    {
        private Expression condition;
        private LinkedList<Instruction> instructions;

        public Repeat(Expression condition, LinkedList<Instruction> instructions)
        {
            this.condition = condition;
            this.instructions = instructions;
        }

        public override object execute(Environment env)
        {
            Symbol value = this.condition.evaluate(env);
            object val = null;
            //TODO verificar errores
            if (value.type.type != Types.BOOLEAN)
                throw new PascalError(0, 0, "The condition for the if isn´t boolean", "Semantic");

            do 
            {
                try
                {
                    foreach (var instruction in instructions)
                    {
                       val  = instruction.execute(env);

                        if (val != null)
                        {
                            if (val.ToString().ToLower().Equals("break"))
                            {
                                return null;
                            }
                            else 
                            {
                                if (val.ToString().ToLower().Equals("continue"))
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (val != null)
                    {
                        if (val.ToString().ToLower().Equals("continue"))
                        {
                            value = this.condition.evaluate(env);
                            continue;
                        }
                    }

                    value = this.condition.evaluate(env);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            } 
            while (!bool.Parse(value.value.ToString()));

            return null;
        }
    }
}
