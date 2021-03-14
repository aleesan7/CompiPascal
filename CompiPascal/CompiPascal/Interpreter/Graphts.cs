using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Graphts : Instruction
    {
        public string content=string.Empty;

        public Graphts() 
        {
        }

        public override object execute(Environment env)
        {
            Environment actual = env;
            

            //while (actual != null)
            //{
                this.content += "<html>\n <body> <h2>Compi Pascal Symbols Table</h2> <table style=\"width:100%\" border=\"1\"> <tr><th>Name</th><th>Object Type</th><th>Type</th><th>Environment</th><th>Line</th><th>Column</th></tr> \n";

                Dictionary<string, Symbol> variables = actual.GetVariables();
                Dictionary<string, Function> functions = actual.GetFunctions();
                Dictionary<string, Procedure> procedures = actual.GetProcedures();

                foreach (KeyValuePair<string, Symbol> variable in variables)
                {
                    this.content += "<tr>" +
                        "<td>" + variable.Key +
                        "</td>" +
                        "<td>" + "VARIABLE" +
                        "</td>" +
                        "<td>" + variable.Value.type.type.ToString() +
                        "</td>" +
                        "<td>" + actual.GetEnvName() +
                        "</td>" +
                        "<td>" + variable.Value.line.ToString() +
                        "</td>" +
                        "<td>" + variable.Value.column.ToString() +
                        "</td>" +
                        "</tr>";
                }

                foreach (KeyValuePair<string, Function> function in functions)
                {
                    this.content += "<tr>" +
                        "<td>" + function.Key +
                        "</td>" +
                        "<td>" + "FUNCTION" +
                        "</td>" +
                        "<td>" + function.Value.GetFunctionType().ToString() +
                        "</td>" +
                        "<td>" + actual.GetEnvName() +
                        "</td>" +
                        "<td>" + function.Value.line.ToString() +
                        "</td>" +
                        "<td>" + function.Value.column.ToString() +
                        "</td>" +
                        "</tr>";
                }

                foreach (KeyValuePair<string, Procedure> procedure in procedures)
                {
                    this.content += "<tr>" +
                        "<td>" + procedure.Key +
                        "</td>" +
                        "<td>" + "PROCEDURE" +
                        "</td>" +
                        "<td>" + "VOID" +
                        "</td>" +
                        "<td>" + actual.GetEnvName() +
                        "</td>" +
                        "<td>" + procedure.Value.line.ToString() +
                        "</td>" +
                        "<td>" + procedure.Value.column.ToString() +
                        "</td>" +
                        "</tr>";
                }

                this.content += "</table> </body> </html>";

                //actual = actual.GetParent();
            //}

            using (StreamWriter outputFile = new StreamWriter("SymbolsTable" + "_" + actual.GetEnvName() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".html"))
            {
                outputFile.WriteLine(this.content);
            }

            return null;
        }

        public override string executeTranslate(Environment env)
        {
            return "graficar_ts();";
        }
    }
}
