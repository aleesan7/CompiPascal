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

        public Symbol(object value, Type type, string id)
        {
            this.id = id;
            this.value = value;
            this.type = type;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

    }
}
