using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Case_element : Instruction
    {
        private Expression condition;
        private LinkedList<Instruction> instructions;
        private Expression elementValue;
        public bool elseFlag = false;
        public bool executed = false;
        public int line;
        public int column;

        public Case_element(Expression condition, LinkedList<Instruction> instructions, Expression elementValue, bool elseFlag, int line, int column)
        {
            this.condition = condition;
            this.instructions = instructions;
            this.elementValue = elementValue;
            this.elseFlag = elseFlag;
            this.line = line;
            this.column = column;
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

                if (bool.Parse(value.value.ToString()) || this.elseFlag)
                {
                    try
                    {
                        foreach(Instruction instruction in this.instructions) 
                        {
                            object val = instruction.execute(env);
                            this.executed = true;

                            if (val != null)
                            {
                                //if (val.ToString().ToLower().Equals("break") || val.ToString().ToLower().Equals("break"))
                                //{
                                    return val;
                                //}
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

        public override string executeTranslate(Environment env)
        {
            string caseElement = string.Empty;
            string instructions = string.Empty;

            try
            {
                foreach(Instruction instruction in this.instructions) 
                {
                    instructions += "\t" + instruction.executeTranslate(env);
                }

                if (!elseFlag) 
                {
                    caseElement += "\t" + this.elementValue.evaluateTranslate(env) + " : " + System.Environment.NewLine + "\t begin" + System.Environment.NewLine + instructions + System.Environment.NewLine + "\t end;";
                }
                else 
                {
                    caseElement += "\t" + " else " + System.Environment.NewLine + "\t begin" + System.Environment.NewLine + instructions + System.Environment.NewLine + "\t end;";
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return caseElement;
        }
    }
}
