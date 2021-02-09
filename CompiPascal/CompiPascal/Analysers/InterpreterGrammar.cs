using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace CompiPascal.Analysers
{
    class InterpreterGrammar : Grammar
    {
        public InterpreterGrammar() : base(caseSensitive: false)
        {
            #region ER
            CommentTerminal lineComment = new CommentTerminal("lineComment", "//", "\n", "\r\n"); //si viene una nueva linea se termina de reconocer el comentario.
            CommentTerminal blockComment = new CommentTerminal("blockComment", "(*", "*)");
            CommentTerminal multiLineComment = new CommentTerminal("multiLineComment", "{", "}");

            IdentifierTerminal identifier = new IdentifierTerminal("identifier");
            StringLiteral STRING = new StringLiteral("string", "\"");
            var INTEGER = new NumberLiteral("integer");
            var REAL = new RegexBasedTerminal("real", "[0-9]+'.'[0-9]+");

            #endregion

            #region Terminals
            var ARRAY = ToTerm("array");
            var INTEGERtype = ToTerm("integer");
            var STRINGType = ToTerm("string");
            var REALType = ToTerm("real");
            var BOOLEANtype = ToTerm("boolean");

            var AND = ToTerm("AND");
            var NOT = ToTerm("NOT");
            var OR = ToTerm("OR");
            var MINUS = ToTerm("-");
            var STAR = ToTerm("*");
            var PLUS = ToTerm("+");
            var DIV = ToTerm("/");
            var MOD = ToTerm("%");

            var FUNCTION = ToTerm("function");
            var PROCEDURE = ToTerm("procedure");

            var DO = ToTerm("do");
            var ELSE = ToTerm("else");
            var END = ToTerm("end");
            var FOR = ToTerm("for");
            var IF = ToTerm("if");
            var BEGIN = ToTerm("begin");
            var PROGRAM = ToTerm("program");
            var REPEAT = ToTerm("repeat");
            var THEN = ToTerm("then");
            var TYPE = ToTerm("type");
            var UNTIL = ToTerm("until");
            var VAR = ToTerm("var");
            var WHILE = ToTerm("while");
            var CASE = ToTerm("case");
            var EXIT = ToTerm("exit");
            var WRITE = ToTerm("write");
            var WRITELN = ToTerm("writeln");
            var GRAPHTS = ToTerm("graficar_ts");
            var BREAK = ToTerm("break");
            var CONTINUE = ToTerm("continue");

            var SEMICOLON = ToTerm(";");
            var EQUAL = ToTerm("=");
            var COLON = ToTerm(":");
            var COMMA = ToTerm(",");
            var DOUBLEDOT = ToTerm("..");
            var LEFTBRAC = ToTerm("[");
            var RIGHTBRAC = ToTerm("]");
            var LEFTPAREN = ToTerm("(");
            var RIGHTPAREN = ToTerm(")");
            var NOTEQUAL = ToTerm("<>");
            var GREATER = ToTerm(">");
            var SMALLERTHAN = ToTerm("<");

            var TRUE = ToTerm("true");
            var FALSE = ToTerm("false");
            #endregion
        }
    }
}
