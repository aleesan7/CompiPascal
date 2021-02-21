using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Writeln : Instruction
    {

        private Expression content;

        public Writeln(Expression content)
        {
            this.content = content;
        }

        //public Object ejecutar(TablaDeSimbolos ts)
        //{
        //    String impresion = content.ejecutar(ts).ToString();
        //    System.Diagnostics.Debug.WriteLine(impresion);
        //    return null;
        //}

        public override object execute(Environment env)
        {
            Symbol toWrite = this.content.evaluate(env);
            System.Diagnostics.Debug.WriteLine(toWrite.value.ToString());
            return toWrite.value;
        }
    }
}
