using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Symbol
    {
        public object value;
        public string id;
        public Type type;
        public int line;
        public int column;

        public Symbol(object value, Type type, string id, int line, int column)
        {
            this.id = id;
            this.value = value;
            this.type = type;
            this.line = line;
            this.column = column;
        }


        public override string ToString()
        {
            return this.value.ToString();
        }

    }
}
