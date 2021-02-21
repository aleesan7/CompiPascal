using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class While : Instruction
    {
        private Expression condition;
        private LinkedList<Instruction> instructions;

        public While(Expression condition, LinkedList<Instruction> instructions)
        {
            this.condition = condition;
            this.instructions = instructions;
        }

        public override object execute(Environment env)
        {
            Symbol value = this.condition.evaluate(env);

            //TODO verificar errores
            if (value.type.type != Types.BOOLEAN)
                throw new PascalError(0, 0, "The condition for the if isn´t boolean", "Semantic");

            while (bool.Parse(value.value.ToString())) 
            {
                try
                {
                    foreach (var instruccion in instructions)
                    {
                        instruccion.execute(env);
                    }

                    value = this.condition.evaluate(env);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return null;
        }

    }
}
