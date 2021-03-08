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
        private LinkedList<int> dimentions;

        public Symbol(object value, Type type, string id, int line, int column)
        {
            this.id = id;
            this.value = value;
            this.type = type;
            this.line = line;
            this.column = column;
            this.dimentions = null;
        }

        //Array constructor
        public Symbol(Type type, string id, LinkedList<int> dimentions, int line, int column)
        {
            this.id = id;
            ArrayNode node = new ArrayNode();
            node.initializeNode(dimentions.Count, 1, dimentions);
            this.value = node;
            this.type = type;
            this.line = line;
            this.column = column;
            this.dimentions = dimentions;
        }


        public override string ToString()
        {
            return this.value.ToString();
        }

    }
}
