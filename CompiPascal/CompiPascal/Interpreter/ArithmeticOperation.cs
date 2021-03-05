using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class ArithmeticOperation : Expression
    {

        private Expression left;
        private Expression right;
        private string type;
        public int line;
        public int column;

        public ArithmeticOperation(Expression left, Expression right, string type, int line, int column)
        {
            this.left = left;
            this.right = right;
            this.type = type;
            this.line = line;
            this.column = column;
        }
        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = this.right.evaluate(env);
            Symbol result;
            Types resultantType = Utils.TypesTable.getType(left.type, right.type);

            if (resultantType != Types.INTEGER && (type != "+" && type != "-" && type != "*" && type != "/" && type != "%"))
                throw new Exception();

            switch (type)
            {
                case "+":
                    result = new Symbol(double.Parse(left.ToString()) + double.Parse(right.ToString()), left.type, null, this.line, this.column);
                    return result;
                case "-":
                    result = new Symbol(double.Parse(left.ToString()) - double.Parse(right.ToString()), left.type, null, this.line, this.column);
                    return result;
                case "*":
                    result = new Symbol(double.Parse(left.ToString()) * double.Parse(right.ToString()), left.type, null, this.line, this.column);
                    return result;
                case "/":
                    result = new Symbol(double.Parse(left.ToString()) / double.Parse(right.ToString()), left.type, null, this.line, this.column);
                    return result;
                case "%":
                    result = new Symbol(double.Parse(left.ToString()) % double.Parse(right.ToString()), left.type, null, this.line, this.column);
                    return result;
            }

            return null;
        }

    }
}
