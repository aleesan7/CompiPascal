using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class ArrayAccess : Expression
    {
        protected string id;
        private LinkedList<Expression> indexes;

        public ArrayAccess(string id, LinkedList<Expression> indexes)
        {
            this.indexes = indexes;
            this.id = id;
        }

        public override Symbol evaluate(Environment env)
        {
            LinkedList<int> indexesValues = new LinkedList<int>();
            foreach (Expression expr in this.indexes)
            {
                Symbol valueSymbol = expr.evaluate(env);
                //Se comprueba que cada indice para acceder al arreglo sea de tipo numerico
                if (valueSymbol.type.type != Types.INTEGER)
                {
                    throw new CompiPascal.Utils.PascalError(valueSymbol.line, valueSymbol.column, "The value of the indexes to access an array must be numeric. The index [" + valueSymbol.value.ToString() + "] is not numeric.", "Semantic");
                }
                indexesValues.AddLast(int.Parse(valueSymbol.value.ToString()));
            }
            return env.GetArrayValue(this.id, indexesValues);
        }

        public override string evaluateTranslate(Environment env)
        {
            string arrayAccessContent = string.Empty;

            string indexesContent = string.Empty;

            foreach (Expression expr in this.indexes)
            {
                indexesContent += expr.evaluateTranslate(env) + ",";
            }

            indexesContent = indexesContent.Substring(0, indexesContent.Length - 1);

            arrayAccessContent += this.id + "[" + indexesContent + "]";

            return arrayAccessContent;
        }
    }
}