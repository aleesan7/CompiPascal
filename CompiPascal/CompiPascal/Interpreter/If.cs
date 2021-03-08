using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class If : Instruction
    {
        private Expression value;
        private LinkedList<Instruction> Instructions;
        private LinkedList<Instruction> _else;
        public int line;
        public int column;

        public If(Expression value, LinkedList<Instruction> Instructions, LinkedList<Instruction> _else, int line, int column)
        {
            this.value = value;
            this.Instructions = Instructions;
            this._else = _else;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            try
            {
                Symbol value = this.value.evaluate(env);

                //TODO verificar errores
                if (value.type.type != Types.BOOLEAN)
                    throw new PascalError(this.line, this.column, "The condition for the if isn´t boolean", "Semantic");

                if (bool.Parse(value.value.ToString()))
                {
                    try
                    {
                        foreach (var instruccion in Instructions)
                        {
                            object val = instruccion.execute(env);

                            
                            if (instruccion.results.Count> 0) 
                            {
                                foreach(string result in instruccion.results) 
                                {
                                    this.results.AddLast(result);
                                }
                                instruccion.results.Clear();
                            }

                            if (val != null)
                            {
                                //if (val.ToString().ToLower().Equals("break"))
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
                else
                {
                    if (_else != null)
                    {
                        try
                        {
                            foreach (var instruccion in _else)
                            {
                                object val = instruccion.execute(env);

                                if (instruccion.results.Count > 0)
                                {
                                    foreach (string result in instruccion.results)
                                    {
                                        this.results.AddLast(result);
                                    }
                                    instruccion.results.Clear();
                                }

                                if (val != null) 
                                {
                                    //if (val.ToString().ToLower().Equals("break")) 
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
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            return null;
        }
    }
}
