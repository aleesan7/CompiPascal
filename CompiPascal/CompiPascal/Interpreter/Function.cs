using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Function : Instruction
    {
        public string id;
        public FunctionTypes functionType;
        public LinkedList<Declare> parameters;
        public LinkedList<Instruction> localVariables;
        public LinkedList<Instruction> instructions;
        public LinkedList<Expression> parameterValues;


        public enum FunctionTypes
        {
            INTEGER = 0,
            REAL = 1,
            STRING = 2,
            BOOLEAN = 3,
            VOID = 4
        }

        //Function with local vars, parameters and instructions
        public Function(string id, FunctionTypes type, LinkedList<Instruction> localVariables, LinkedList<Declare> parameters, LinkedList<Instruction> instructions)
        {
            this.id = id;
            this.functionType = type;
            this.parameters = parameters;
            this.localVariables = localVariables;
            this.instructions = instructions;
        }

        //Function with local vars and instructions
        public Function(string id, FunctionTypes type, LinkedList<Instruction> localVariables, LinkedList<Instruction> instructions) 
        {
            this.id = id;
            this.functionType = type;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = localVariables;
            this.instructions = instructions;
        }

        //Function with parameters and instructions
        public Function(string id, FunctionTypes type, LinkedList<Declare> parameters, LinkedList<Instruction> instructions)
        {
            this.id = id;
            this.functionType = type;
            this.parameters = parameters;
            this.localVariables = new LinkedList<Instruction>();
            this.instructions = instructions;
        }

        //Function with only instructions
        public Function(string id, FunctionTypes type, LinkedList<Instruction> instructions)
        {
            this.id = id;
            this.functionType = type;
            this.parameters = new LinkedList<Declare>();
            this.localVariables = new LinkedList<Instruction>();
            this.instructions = instructions;
        }

        public void SetValuesToParameters(LinkedList<Expression> values)
        {
            this.parameterValues = values;
        }

        public override object execute(Environment env)
        {
            env.AddFunction(this.id, this);
            return null;
        }
    }
}
