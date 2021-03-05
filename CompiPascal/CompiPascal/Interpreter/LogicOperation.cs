using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class LogicOperation : Expression
    {
        private Expression left;
        private Expression right;
        private string tipoOperacion;
        public int line;
        public int column;

        public LogicOperation(Expression left, Expression right, string tipoOperacion, int line, int column)
        {
            this.left = left;
            this.right = right;
            this.tipoOperacion = tipoOperacion;
            this.line = line;
            this.column = column;
        }
        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = this.right.evaluate(env);
            Symbol result = null;
            Type type = new Type(Types.BOOLEAN, null);

            Types resultantType = TypesTable.getType(left.type, right.type);
            if (resultantType == Types.ERROR)
                throw new PascalError(0, 0, "Incorrect data type", "Semantic");

            switch (tipoOperacion)
            {
                case "=":
                    if(resultantType == Types.BOOLEAN) 
                    {
                        result = new Symbol(left.ToString().Equals(right.ToString()), type, null, this.line, this.column);
                    }
                    else 
                    {
                        result = new Symbol(double.Parse(left.ToString()) == double.Parse(right.ToString()), type, null, this.line, this.column);
                    }
                    return result;
                case "<>":
                    result = new Symbol(double.Parse(left.ToString()) != double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case ">":
                    result = new Symbol(double.Parse(left.ToString()) > double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case "<":
                    result = new Symbol(double.Parse(left.ToString()) < double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case ">=":
                    result = new Symbol(double.Parse(left.ToString()) >= double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case "<=":
                    result = new Symbol(double.Parse(left.ToString()) <= double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
            }
            return null;
        }
    }
}
