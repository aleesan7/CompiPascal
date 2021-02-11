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
            var TO = ToTerm("to");
            var ELSE = ToTerm("else");
            var END = ToTerm("end");
            var FOR = ToTerm("for");
            var IF = ToTerm("if");
            var BEGIN = ToTerm("begin");
            var PROGRAM = ToTerm("program");
            var CONST = ToTerm("const");
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
            var DOT = ToTerm(".");
            var DOUBLEDOT = ToTerm("..");
            var LEFTBRAC = ToTerm("[");
            var RIGHTBRAC = ToTerm("]");
            var LEFTPAREN = ToTerm("(");
            var RIGHTPAREN = ToTerm(")");
            var NOTEQUAL = ToTerm("<>");
            var GREATER = ToTerm(">");
            var GREATHEROREQUAL = ToTerm(">=");
            var SMALLERTHAN = ToTerm("<");
            var SMALLEROREQUAL = ToTerm("<=");

            var TRUE = ToTerm("true");
            var FALSE = ToTerm("false");
            #endregion

            #region Non Terminals
            NonTerminal program = new NonTerminal("program");
            NonTerminal program_heading = new NonTerminal("program_heading");
            NonTerminal block = new NonTerminal("block");
            NonTerminal constant_definition_part = new NonTerminal("constant_definition_part");
            NonTerminal constant_definition = new NonTerminal("constant_definition");
            NonTerminal type_definition_part = new NonTerminal("type_definition_part");
            NonTerminal variable_declaration_part = new NonTerminal("variable_declaration_part");
            NonTerminal procedure_and_function_declaration_part = new NonTerminal("procedure_and_function_declaration_part");
            NonTerminal const_values = new NonTerminal("const_values");
            NonTerminal initial = new NonTerminal("initial");
            NonTerminal instrucciones_sup = new NonTerminal("instrucciones_sup");
            NonTerminal instruccion_sup = new NonTerminal("instruccion_sup");
            NonTerminal tipo_funcion = new NonTerminal("tipo_funcion");
            NonTerminal parametros = new NonTerminal("parametros");
            NonTerminal instrucciones = new NonTerminal("instrucciones");
            NonTerminal declaracion = new NonTerminal("declaracion");
            NonTerminal declaracion_array = new NonTerminal("declaracion_array");
            NonTerminal dimensiones = new NonTerminal("dimensiones");
            NonTerminal expresion = new NonTerminal("expresion");
            NonTerminal expresiones = new NonTerminal("expresiones");
            NonTerminal instruccion = new NonTerminal("instruccion");
            NonTerminal instruccion_if_sup = new NonTerminal("instruccion_if_sup");
            NonTerminal instruccion_if = new NonTerminal("instruccion_if");
            NonTerminal instrucciones_elseif = new NonTerminal("instrucciones_elseif");
            NonTerminal instruccion_else = new NonTerminal("instruccion_else");
            NonTerminal instruccion_elseif = new NonTerminal("instruccion_elseif");
            NonTerminal tipo = new NonTerminal("tipo");
            NonTerminal lista_identificadores = new NonTerminal("lista_identificadores");
            #endregion

            #region Grammar
            program.Rule = program_heading + SEMICOLON + block + DOT;

            program_heading.Rule = PROGRAM + identifier;
                      //| PROGRAM + identifier + LEFTPAREN identifier_list RIGHTPAREN;

            //identifier_list = identifier_list comma IDENTIFIER

                        //| IDENTIFIER

            block.Rule =  constant_definition_part + type_definition_part +
                     variable_declaration_part + procedure_and_function_declaration_part +
                     instrucciones_sup;

            //initial.Rule = instrucciones_sup;

            constant_definition_part.Rule = constant_definition_part + constant_definition
                                   | constant_definition;

            constant_definition.Rule = CONST + identifier + EQUAL + const_values;

            const_values.Rule = INTEGER
                              | STRING
                              | REAL;

            instrucciones_sup.Rule = instrucciones_sup + instruccion_sup
                            | instruccion_sup;

            instruccion_sup.Rule = FUNCTION + identifier + LEFTPAREN + parametros + RIGHTPAREN + COLON + tipo_funcion + SEMICOLON + BEGIN + instrucciones + END + SEMICOLON //{:RESULT = new Function(a, b, c, d);:}
                          | FUNCTION + identifier + LEFTPAREN + RIGHTPAREN + COLON + tipo_funcion + SEMICOLON + BEGIN + instrucciones + END + SEMICOLON //{:RESULT = new Function(a, b, c);:}
                          | VAR + declaracion + SEMICOLON; //{:RESULT = a;:}
            instruccion_sup.ErrorRule = SyntaxError + SEMICOLON
                                      | SyntaxError ;

            parametros.Rule = parametros + COMMA + declaracion //{:RESULT = a; RESULT.add(b);:}
                     | declaracion;//{:RESULT = new LinkedList<>(); RESULT.add(a);:}


            dimensiones.Rule = dimensiones + LEFTBRAC + expresion + RIGHTBRAC //{:RESULT = a; RESULT.add(b);:}
                     | LEFTBRAC + expresion + RIGHTBRAC; //{:RESULT = new LinkedList<>(); RESULT.add(a);:}



            expresiones.Rule = expresiones + COMMA + expresion //{:RESULT = a; RESULT.add(b);:}
                      | expresion; //:a{:RESULT = new LinkedList<>(); RESULT.add(a);:}


            instrucciones.Rule = instrucciones + instruccion //:b{:RESULT = a; RESULT.add(b);:}
                        | instruccion;//:a{:RESULT = new LinkedList<>(); RESULT.add(a);:}


            instruccion.Rule = WRITE + LEFTPAREN + expresion + RIGHTPAREN + SEMICOLON  //{:RESULT = new Imprimir(a);:}
                     | VAR + declaracion  //{:RESULT = a;:}
                     | identifier + COLON + EQUAL + expresion + SEMICOLON //{:RESULT = new Asignacion(a, b);:}
                     | identifier + dimensiones + COLON + EQUAL + expresion + SEMICOLON //{:RESULT = new AsignacionArreglo(a, b, c);:}
                     | instruccion_if_sup  //{:RESULT = a;:}
                     | WHILE + expresion + DO + BEGIN + instrucciones + END + SEMICOLON //{:RESULT = new While(a, b);:}
                     | FOR + identifier + COLON + EQUAL + expresion + TO + expresion + BEGIN + instrucciones + END + SEMICOLON
                     | REPEAT + instrucciones + UNTIL + expresion + SEMICOLON
                     //| RFOR LEFTPAREN identifier:a EQUAL expresion: b SEMICOLON expresion: c SEMICOLON identifier: d EQUAL expresion: e RIGHTPAREN LLAVIZQ instrucciones:f LLAVDER{:RESULT = new For(new Asignacion(a, b), c, new Asignacion(d, e), f);:}
                     | identifier + LEFTPAREN + expresiones + RIGHTPAREN + SEMICOLON //{:RESULT = new LlamadaFuncion(a, b);:}
                     | identifier + LEFTPAREN + RIGHTPAREN + SEMICOLON               //{:RESULT = new LlamadaFuncion(a, new LinkedList<>());:}
                                                                                     //| RETURN + SEMICOLON             {:RESULT = new Return();:}
                     | EXIT + expresion + SEMICOLON //{:RESULT = new Return(a);:}
                     | BREAK + SEMICOLON; //{:RESULT = new Break();:}
            instruccion.ErrorRule = SyntaxError + SEMICOLON
                                  | SyntaxError;

            declaracion.Rule = identifier + COLON + tipo //{:RESULT = new Declaracion(b, a);:}
                      | declaracion_array; //{:RESULT = new DeclaracionArreglo(b, a, c);:}

            declaracion_array.Rule = identifier + COLON + ARRAY + LEFTBRAC + expresion + DOUBLEDOT + expresion + RIGHTBRAC + SEMICOLON;

            //declaracion.Rule = lista_identificadores + COLON + tipo
            //          | identifier + COLON + ARRAY + LEFTBRAC + expresion + DOUBLEDOT + expresion + RIGHTBRAC + SEMICOLON;

            //lista_identificadores.Rule = lista_identificadores + COMMA + identifier
            //                    | identifier;

            tipo.Rule = INTEGERtype  //{:RESULT = a;:}
                | REALType
                | STRINGType //:a  {:RESULT = a;:}
                | BOOLEANtype; //:a {:RESULT = a;:}


            tipo_funcion.Rule = tipo;//  {:RESULT = a;:}


            expresion.Rule =
                   //MENOS       expresion: a         {:RESULT = new Operacion(a, Operacion.Tipo_operacion.NEGATIVO);:}% prec UMENOS
                   expresion + PLUS + expresion        //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.SUMA);:}
                 | expresion + MINUS + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.RESTA);:}
                 | expresion + STAR + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.MULTIPLICACION);:}
                 | expresion + DIV + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.DIVISION);:}
                 | expresion + GREATER + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.MAYOR_QUE);:}
                 | expresion + SMALLERTHAN + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.MENOR_QUE);:}
                 | expresion + SMALLEROREQUAL + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.MENOR_IGUAL_QUE);:}
                 | expresion + GREATHEROREQUAL + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.MAYOR_IGUAL_QUE);:}
                 | expresion + NOTEQUAL + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.DIFERENTE_QUE);:}
                 | expresion + EQUAL + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.IGUAL_QUE);:}
                 | NOT + expresion                     //{:RESULT = new Operacion(a, Operacion.Tipo_operacion.NOT);:}% prec RNOT
                 | expresion + OR + expresion          //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.OR);:}
                 | expresion + AND + expresion         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.AND);:}
                                                       //| expresion:a CONCAT      expresion: b         //{:RESULT = new Operacion(a, b, Operacion.Tipo_operacion.CONCATENACION);:}
                 | LEFTPAREN + expresion + RIGHTPAREN       //{:RESULT = a;:}
                 | INTEGER                                     //{:RESULT = new Operacion(new Double(a));:}
                 | REAL                                   //{:RESULT = new Operacion(new Double(a));:}
                 | STRING                                     //{:RESULT = new Operacion(a, Operacion.Tipo_operacion.CADENA);:}
                 | TRUE                                      //{:RESULT = new Operacion(a, Operacion.Tipo_operacion.TRUE);:}
                 | FALSE                                     //{:RESULT = new Operacion(a, Operacion.Tipo_operacion.FALSE);:}
                 | identifier                                 //{:RESULT = new Operacion(a, Operacion.Tipo_operacion.identifier);:}
                 | identifier + LEFTPAREN + expresiones + RIGHTPAREN //{:RESULT = new LlamadaFuncion(a, b);:}
                 | identifier + LEFTPAREN + RIGHTPAREN               //{:RESULT = new LlamadaFuncion(a, new LinkedList<>());:}
                 | identifier + dimensiones;                  //{:RESULT = new AccesoArreglo(a, b);:}


            instruccion_if_sup.Rule = instruccion_if + instruccion_else //{:RESULT = new If(a);:}
                             //| instruccion_if + instrucciones_elseif  //{:RESULT = new If(a, b);:}
                             //| instruccion_if + instrucciones_elseif + instruccion_else //{:RESULT = new If(a, b, c);:}
                             | instruccion_if;  //{:RESULT = new If(a, b);:}


            instruccion_if.Rule = IF + LEFTPAREN + expresion + RIGHTPAREN + THEN + BEGIN + instrucciones + END; //{:RESULT = new SubIf(a, b);:}


            instrucciones_elseif.Rule = instrucciones_elseif + instruccion_elseif //:b{:RESULT = a; RESULT.add(b);:}
                               | instruccion_elseif; //:a{:RESULT = new LinkedList<>(); RESULT.add(a);:}
        

        //    instruccion_elseif.Rule = RELSEIF LEFTPAREN expresion:a RIGHTPAREN LLAVIZQ instrucciones:b LLAVDER {:RESULT = new SubIf(a, b);:}
        //;

            instruccion_else.Rule = ELSE + instrucciones; //{:RESULT = new SubIf(a);:}

            #endregion
        }
    }
}
