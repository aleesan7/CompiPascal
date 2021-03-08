using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Procedure : Instruction
    {
        private string id;
        public LinkedList<Declare> parameters;
        public LinkedList<Instruction> localVariables;
        public LinkedList<Instruction> instructions;
        public LinkedList<Expression> parameterValues;
        public int line;
        public int column;
        
        //Procedure with parameters, local variables and instructions
        public Procedure(string id, LinkedList<Declare> parameters, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = parameters;
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
        }

        //Procedure with local variables and instructions
        public Procedure(string id, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = localVariables;
            this.instructions = instructions;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
        }

        //procedure with only instructions
        public Procedure(string id, LinkedList<Instruction> instructions, int line, int column)
        {
            this.id = id;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = new LinkedList<Instruction>();
            this.instructions = instructions;
            this.line = line;
            this.column = column;
            this.results = new LinkedList<string>();
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
    }
}
