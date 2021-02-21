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

        public LogicOperation(Expression left, Expression right, string tipoOperacion)
        {
            this.left = left;
            this.right = right;
            this.tipoOperacion = tipoOperacion;
        }
        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = this.right.evaluate(env);
            Symbol result;
            Type type = new Type(Types.BOOLEAN, null);

            Types resultantType = TypesTable.getType(left.type, right.type);
            if (resultantType == Types.ERROR)
                throw new PascalError(0, 0, "Incorrect data type", "Semantic");

            switch (tipoOperacion)
            {
                case "=":
                    result = new Symbol(double.Parse(left.ToString()) == double.Parse(right.ToString()), type, null);
                    return result;
                case "<>":
                    result = new Symbol(double.Parse(left.ToString()) != double.Parse(right.ToString()), type, null);
                    return result;
                case ">":
                    result = new Symbol(double.Parse(left.ToString()) > double.Parse(right.ToString()), type, null);
                    return result;
                case "<":
                    result = new Symbol(double.Parse(left.ToString()) < double.Parse(right.ToString()), type, null);
                    return result;
                case ">=":
                    result = new Symbol(double.Parse(left.ToString()) >= double.Parse(right.ToString()), type, null);
                    return result;
                case "<=":
                    result = new Symbol(double.Parse(left.ToString()) <= double.Parse(right.ToString()), type, null);
                    return result;
            }
            return null;
        }
    }
}
