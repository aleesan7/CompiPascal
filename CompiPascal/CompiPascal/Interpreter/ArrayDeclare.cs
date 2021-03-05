using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class ArrayDeclare 
    {
        private string id;
        public Symbol[] values;
        public int min;
        public int max;

        public ArrayDeclare(string id, Symbol[] values, int min, int max)
        {
            this.id = id;
            this.values = values;
            this.min = min;
            this.max = max;
        }

        public Symbol GetAttribute(int index) 
        {
            return this.values[index];
        }

        public void SetAtribute(int index, Symbol value) 
        {
            this.values[index] = value;
        }

    }
}
