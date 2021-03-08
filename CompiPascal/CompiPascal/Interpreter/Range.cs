using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Range
    {
        private Expression expSup;
        private Expression expInf;

        public Range(Expression expInf, Expression expSup)
        {
            this.expInf = expInf;
            this.expSup = expSup;
        }

        public Expression GetInfExpression()
        {
            return this.expInf;
        }

        public Expression GetSupExpression() 
        {
            return this.expSup;
        }
        
    }
}
