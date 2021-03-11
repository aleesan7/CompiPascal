using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
     class ArrayDeclare :  Instruction
    {
        public string id;
        public Type type;
        public int line;
        public int column;
        private LinkedList<Range> dimentions;

        public ArrayDeclare(string id, Type type, LinkedList<Range> dimentions, int line, int column)
        {
            this.id = id;
            this.type = type;
            this.dimentions = dimentions;
            this.line = line;
            this.column = column;
        }

        public override object execute(Environment env)
        {
            LinkedList<int> dimentionsSizes = new LinkedList<int>();
            int size = 0;
            foreach (Range range in this.dimentions)
            {
                Expression sup = range.GetSupExpression();
                Expression inf = range.GetInfExpression();

                Symbol supVal = sup.evaluate(env);
                Symbol infVal = inf.evaluate(env);

                if (supVal.type.type != Types.INTEGER || infVal.type.type != Types.INTEGER)
                {
                    throw new CompiPascal.Utils.PascalError(this.line, this.column, "The values to define an array index, must be integer", "Semantic");
                }

                if (int.Parse(infVal.value.ToString()) < 0)
                {
                    size = int.Parse(supVal.value.ToString()) + (int.Parse(infVal.value.ToString()) * -1);
                }
                else 
                {
                    size = int.Parse(infVal.value.ToString()) + int.Parse(supVal.value.ToString()) - 1;
                }

                dimentionsSizes.AddLast(size);
            }

            env.declareVariable(this.id, new Symbol(this.type, this.id, dimentionsSizes, this.line, this.column));
            return null;
        }

        public override string executeTranslate(Environment env)
        {
            string ArrayDeclareContent = string.Empty;
            string dimentions = string.Empty;

            foreach(Range range in this.dimentions) 
            {
                dimentions += range.GetInfExpression().evaluateTranslate(env) + ".." + range.GetSupExpression().evaluateTranslate(env) + " , " ;
            }

            dimentions = dimentions.Substring(0, dimentions.Length - 3);

            ArrayDeclareContent += "var " + this.id + " : " + "array" + "[" + dimentions + "] of " + this.type.type.ToString().ToLower() + ";"; 

            return ArrayDeclareContent;
        }
    }

}
