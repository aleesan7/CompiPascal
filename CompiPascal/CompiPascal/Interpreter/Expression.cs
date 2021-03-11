using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    abstract class Expression
    {
        public abstract Symbol evaluate(Environment env);

        public abstract string evaluateTranslate(Environment env);
    }
}
