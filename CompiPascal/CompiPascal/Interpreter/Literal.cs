using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Literal : Expression
    {
        private Types type;
        private object value;
        private string id;

        public Literal(Types type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public Literal(Types type, string id) 
        {
            this.type = type;
            this.id = id;
        }

        public override Symbol evaluate(Environment env)
        {
            //TODO tipos
            switch (this.type) 
            {
                case Types.INTEGER:
                    return new Symbol(this.value, new Type(Types.INTEGER, null), null, 0, 0);
                case Types.REAL:
                    return new Symbol(this.value, new Type(Types.REAL, null), null, 0, 0);
                case Types.STRING:
                    return new Symbol(this.value, new Type(Types.STRING, null), null, 0, 0);
                case Types.BOOLEAN:
                    return new Symbol(this.value, new Type(Types.BOOLEAN, null), null, 0, 0);
                case Types.IDENTIFIER:
                    Access accessedVar = new Access(this.id);
                    //Symbol identificador = env.ObtainVariable(this.id);
                    return accessedVar.evaluate(env);
                default:
                    return null;
            }
        }
    }
}
