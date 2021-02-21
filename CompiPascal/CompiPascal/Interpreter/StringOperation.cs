using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class StringOperation : Expression
    {
        private Expression left;
        private Expression right;
        private string operationType;
        public StringOperation(Expression left, Expression right, string operationType)
        {
            this.left = left;
            this.right = right;
            this.operationType = operationType;
        }

        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = this.right.evaluate(env);
            Symbol result;
            Type type = new Type(Types.STRING, null);

            Types resultantType = TypesTable.getType(left.type, right.type);
            if (resultantType == Types.ERROR)
                throw new PascalError(0, 0, "Incorrect data type", "Semantic");

            switch (operationType)
            {
                case ",":
                    result = new Symbol(left.ToString() + right.ToString(), type, null);
                    return result;
            }
            return null;
        }
    }
}
