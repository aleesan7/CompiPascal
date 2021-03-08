using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Range
    {
        private Expression expSup;
        private Expression expInf;

        public Range(Expression expSup, Expression expInf)
        {
            this.expSup = expSup;
            this.expInf = expInf;
        }

        public Expression GetSupExpression() 
        {
            return this.expSup;
        }

        public Expression GetInfExpression() 
        {
            return this.expInf;
        }
    }
}
