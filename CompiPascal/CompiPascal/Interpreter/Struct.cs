using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Struct : Instruction
    {
        public string id;
        public Type type;
        public int line;
        public int column;
        public Dictionary<string, Symbol> members;

        public Struct(string id, Type type, int line, int column, Dictionary<string, Symbol> members)
        {
            this.id = id;
            this.type = type;
            this.line = line;
            this.column = column;
            this.members = members;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            env.AddStruct(this.id, this);
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            throw new NotImplementedException();
        }
    }
}
