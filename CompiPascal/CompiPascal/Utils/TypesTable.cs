using CompiPascal.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Utils
{
    class TypesTable
    {
        public static Types[,] types = new Types[4, 4] {
            { Types.INTEGER, Types.REAL, Types.STRING, Types.ERROR },
            { Types.REAL, Types.REAL, Types.STRING, Types.ERROR },
            { Types.STRING, Types.STRING, Types.STRING, Types.STRING },
            { Types.ERROR, Types.ERROR, Types.STRING, Types.BOOLEAN}
        };

        public static Types getType(Interpreter.Type left, Interpreter.Type right)
        {
            return types[(int)left.type, (int)right.type];
        }
    }
}
