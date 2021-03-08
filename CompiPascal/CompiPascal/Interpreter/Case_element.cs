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
        public int line;
        public int column;

        public Case_element(Expression condition, Instruction instruction, int line, int column)
        {
            this.condition = condition;
            this.instruction = instruction;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
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
                    throw new PascalError(this.line, this.column, "The condition for the if isn´t boolean", "Semantic");

                if (bool.Parse(value.value.ToString()))
                {
                    try
                    {
                        object val = this.instruction.execute(env);

                        if (instruction.results.Count > 0)
                        {
                            foreach (string result in instruction.results)
                            {
                                this.results.AddLast(result);
                            }
                            instruction.results.Clear();
                        }

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
