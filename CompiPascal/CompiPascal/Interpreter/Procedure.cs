using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Procedure : Instruction
    {
        private string id;
        public string uniqueID;
        public LinkedList<Declare> parameters;
        public LinkedList<Instruction> localVariables;
        public LinkedList<Instruction> instructions;
        public LinkedList<Expression> parameterValues;
        public LinkedList<Procedure> nestedProcedures;
        public int line;
        public int column;

        public Procedure(string id, LinkedList<Declare> parameters, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, LinkedList<Procedure> nestedProcedures, int line, int column) 
        {
            this.id = id;
            this.parameters = parameters;
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.nestedProcedures = nestedProcedures;
            this.line = line;
            this.column = column;
        }

        public Procedure(string id, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, LinkedList<Procedure> nestedProcedures, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.nestedProcedures = nestedProcedures;
            this.line = line;
            this.column = column;
        }

        public Procedure(string id, LinkedList<Instruction> instructions, LinkedList<Procedure> nestedProcedures, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = new LinkedList<Instruction>() ;
            this.instructions = instructions;
            this.nestedProcedures = nestedProcedures;
            this.line = line;
            this.column = column;
        }

        //Procedure with parameters, local variables and instructions
        public Procedure(string id, LinkedList<Declare> parameters, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = parameters;
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.nestedProcedures = new LinkedList<Procedure>();
            this.line = line;
            this.column = column;
        }

        //Procedure with local variables and instructions
        public Procedure(string id, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.nestedProcedures = new LinkedList<Procedure>();
            this.line = line;
            this.column = column;
        }

        //procedure with only instructions
        public Procedure(string id, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = new LinkedList<Instruction>();
            this.instructions = instructions;
            this.nestedProcedures = new LinkedList<Procedure>();
            this.line = line;
            this.column = column;
        }

        public void SetValuesToParameters(LinkedList<Expression> values) 
        {
            this.parameterValues = values;
        }

        public override object execute(Environment env)
        {
            env.AddProcedure(this.id, this);
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            env.AddProcedure(this.id, this);
            Environment newEnv = new Environment(env);
            newEnv.idParent += this.id + "_";
            newEnv.actualFunct = this.id;
            this.uniqueID = newEnv.idParent;


            string cadena = "procedure " + newEnv.idParent + "(";
            foreach (Declare dec in this.parameters)
            {
                cadena += dec.GetId() + " : " + dec.type.type + ";";

            }

            if (this.parameters.Count > 0)
            {
                cadena = cadena.Substring(0, cadena.Length - 1);
            }

            cadena += env.GetStringVar();

            cadena += ");" + System.Environment.NewLine;


            foreach (Instruction instruction in this.localVariables)
            {
                Declare dec = (Declare)instruction;

                cadena += dec.executeTranslate(newEnv) + System.Environment.NewLine;
                dec.previous(newEnv);
            }

            foreach(Procedure proc in this.nestedProcedures) 
            {
                newEnv.GetGlobalEnvironment().GetCodes().AddLast(proc.executeTranslate(newEnv));
            }

            cadena += "begin" + System.Environment.NewLine;

            foreach (Instruction instruction in this.instructions)
            {
                cadena += "\t" + instruction.executeTranslate(newEnv) + System.Environment.NewLine;
            }

            cadena += "end;" + System.Environment.NewLine;

            return cadena;
        }
    }
}
