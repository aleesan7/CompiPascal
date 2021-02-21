using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Utils
{
    class PascalError : Exception
    {
        private int line, column;
        private string message;
        private string type;

        public PascalError(int line, int column, string message, string type)
        {
            this.line = line;
            this.column = column;
            this.message = message;
            this.type = type;
        }

        public override string ToString()
        {
            return "An error was found: " + this.type + " - in the line: " + this.line + " - in the column: " + this.column + " - message: " + this.message;
        }
    }
}
