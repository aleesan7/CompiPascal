﻿using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompiPascal.Interpreter
{
    class While : Instruction
    {
        private Expression condition;
        private LinkedList<Instruction> instructions;
        public int line;
        public int column;

        public While(Expression condition, LinkedList<Instruction> instructions, int line, int column)
        {
            this.condition = condition;
            this.instructions = instructions;
            this.line = line;
            this.column = column;
        }

        public override object execute(Environment env)
        {
            Symbol value = this.condition.evaluate(env);
            object val = null;
            //TODO verificar errores
            if (value.type.type != Types.BOOLEAN)
                throw new PascalError(this.line, this.column, "The condition for the if isn´t boolean", "Semantic");

            while (bool.Parse(value.value.ToString())) 
            {
                try
                {
                    foreach (var instruction in instructions)
                    {
                        val = instruction.execute(env);

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

            return null;
        }

        public override string executeTranslate(Environment env)
        {
            string resultantWhile = string.Empty;
            string condition = this.condition.evaluateTranslate(env);
            string instructions = string.Empty;

            if (this.instructions.Count > 0)
            {
                foreach (Instruction instruction in this.instructions)
                {
                    instructions = instructions + instruction.executeTranslate(env) + System.Environment.NewLine;
                }

            }

            resultantWhile += "\t" + "while ( " + condition + " ) do" + System.Environment.NewLine;
            resultantWhile += "\t" + "begin" + System.Environment.NewLine;
            resultantWhile += "\t" + instructions;
            resultantWhile += "\t" + "end;" + System.Environment.NewLine;

            return resultantWhile;
        }
    }
}
