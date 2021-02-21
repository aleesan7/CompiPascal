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

        public ArithmeticOperation(Expression left, Expression right, string type)
        {
            this.left = left;
            this.right = right;
            this.type = type;
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
                    result = new Symbol(double.Parse(left.ToString()) + double.Parse(right.ToString()), left.type, null);
                    return result;
                case "-":
                    result = new Symbol(double.Parse(left.ToString()) - double.Parse(right.ToString()), left.type, null);
                    return result;
                case "*":
                    result = new Symbol(double.Parse(left.ToString()) * double.Parse(right.ToString()), left.type, null);
                    return result;
                case "/":
                    result = new Symbol(double.Parse(left.ToString()) / double.Parse(right.ToString()), left.type, null);
                    return result;
                case "%":
                    result = new Symbol(double.Parse(left.ToString()) % double.Parse(right.ToString()), left.type, null);
                    return result;
            }

            return null;
        }

    }
}
