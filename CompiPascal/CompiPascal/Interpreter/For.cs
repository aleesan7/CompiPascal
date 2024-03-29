﻿using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class For : Instruction
    {
        private Assign assignment;
        private Expression condition;
        private Assign increment; 
        private LinkedList<Instruction> instructions;
        public int line;
        public int column;

        public For(Assign assignment, Expression condition, Assign increment, LinkedList<Instruction> instructions, int line, int column)
        {
            this.assignment = assignment;
            this.condition = condition;
            this.increment = increment;
            this.instructions = instructions;
            this.line = line;
            this.column = column;
        }

        public override object execute(Environment env)
        {
            this.assignment.execute(env);

            Symbol cond = this.condition.evaluate(env);
            object val = null;
            //TODO verificar errores
            if (cond.type.type != Types.BOOLEAN)
                throw new PascalError(this.line, this.column, "The condition for the if isn´t boolean", "Semantic");

            

            for (int i = int.Parse(this.assignment.GetValue(env)); bool.Parse(cond.value.ToString()); i++) 
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
                            this.increment.execute(env);
                            cond = this.condition.evaluate(env);
                            continue;
                        }
                    }

                    this.increment.execute(env);

                    cond = this.condition.evaluate(env);
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
            string resultantFor = string.Empty;
            string condition = this.condition.evaluateTranslate(env);
            string[] conditionParts = condition.Split('=');
            string instructions = string.Empty;

            if (this.instructions.Count > 0)
            {
                foreach (Instruction instruction in this.instructions)
                {
                    instructions = instructions + instruction.executeTranslate(env) + System.Environment.NewLine;
                }

            }

            resultantFor += "\t" + "for " + this.assignment.GetId() + " := " + this.assignment.GetValue().evaluateTranslate(env) + " to " + conditionParts[1] + " do" + System.Environment.NewLine;
            resultantFor += "\t" + "begin" + System.Environment.NewLine;
            resultantFor += "\t" + instructions;
            resultantFor += "\t" + "end;" + System.Environment.NewLine;

            return resultantFor;
        }
    }
}
