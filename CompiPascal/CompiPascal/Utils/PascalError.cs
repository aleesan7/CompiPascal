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
        private string environment;

        public PascalError(int line, int column, string message, string type)
        {
            this.line = line;
            this.column = column;
            this.message = message;
            this.type = type;
        }

        public string GetMesage() 
        {
            return this.message;
        }

        public int GetLine() 
        {
            return this.line;
        }

        public int GetColumn() 
        {
            return this.column;
        }

        public string GetType() 
        {
            return this.type;
        }

        public override string ToString()
        {
            return "An error was found: " + this.type + " - in the line: " + this.line + " - in the column: " + this.column + " - message: " + this.message;
        }
    }
}
