using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class ArrayAssign : Instruction
    {
        private string id;
        private Expression value;
        private LinkedList<Expression> indexes;

        public ArrayAssign(string id, Expression value, LinkedList<Expression> indexes)
        {
            this.id = id;
            this.value = value;
            this.indexes = indexes;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            LinkedList<int> indexesValues = new LinkedList<int>();
            foreach(Expression expression in this.indexes)
            {
                Symbol valueSymbol = expression.evaluate(env);

                if (valueSymbol.type.type != Types.INTEGER)
                {
                    throw new CompiPascal.Utils.PascalError(valueSymbol.line, valueSymbol.column, "The values to define an array index, must be integer", "Semantic");
                }
                indexesValues.AddLast(int.Parse(valueSymbol.value.ToString()));
            }

            env.AssignArrayValue(this.id, this.value.evaluate(env), indexesValues);
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            string arrayAssignContent = string.Empty;
            string indexesContent = string.Empty;

            foreach(Expression expr in this.indexes) 
            {
                indexesContent += expr.evaluateTranslate(env) + ",";
            }

            indexesContent = indexesContent.Substring(0, indexesContent.Length - 1);

            arrayAssignContent += this.id + "[" + indexesContent + "]" + " := " + this.value.evaluateTranslate(env) + ";" + System.Environment.NewLine;

            return arrayAssignContent;
        }
    }
}
