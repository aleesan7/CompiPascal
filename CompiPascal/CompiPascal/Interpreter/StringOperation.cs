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
        public int line;
        public int column;

        public StringOperation(Expression left, Expression right, string operationType, int line, int column)
        {
            this.left = left;
            this.right = right;
            this.operationType = operationType;
            this.line = line;
            this.column = column;
        }

        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = this.right.evaluate(env);
            Symbol result;
            Type type = new Type(Types.STRING, null);

            Types resultantType = TypesTable.getType(left.type, right.type);
            if (resultantType == Types.ERROR)
                throw new PascalError(this.line, this.column, "Incorrect data type", "Semantic");

            switch (operationType)
            {
                case ",":
                    result = new Symbol(left.ToString() + right.ToString(), type, null, this.line, this.column);
                    return result;
            }
            return null;
        }

        public override string evaluateTranslate(Environment env)
        {
            throw new NotImplementedException();
        }
    }
}
