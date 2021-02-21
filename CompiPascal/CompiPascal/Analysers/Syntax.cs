using Irony.Parsing;
using Irony.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using CompiPascal.Interpreter;
using CompiPascal.Utils;

namespace CompiPascal.Analysers
{
    class Syntax
    {
        public List<string> resultsList = new List<string>();
        public List<string> errorsList = new List<string>();
        public void Analyze(string input) 
        {
            
            InterpreterGrammar grammar = new InterpreterGrammar();
            LanguageData language = new LanguageData(grammar);

            foreach (var item in language.Errors)
            {
                Console.WriteLine(item);
            }

            Parser parser = new Parser(language);
            ParseTree tree = parser.Parse(input);

            ParseTreeNode root = tree.Root;
            
            Errors errors = new Errors(tree, root);

            if (!errors.HasErrors())
            {
                generateGraph(root);

                LinkedList<Instruction> variableDeclaration = instructions(root.ChildNodes[2].ChildNodes[1]);
                LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                execute(variableDeclaration, instructionsList);

                //LinkedList<Instruccion> AST = instrucciones(raiz.ChildNodes.ElementAt(0));

                //TablaDeSimbolos global = new TablaDeSimbolos();

                //foreach (Instruccion ins in AST)
                //{
                //    ins.ejecutar(global);
                //}

            }
            else 
            {
                //We print the errors in the output 
            }

        }

        public void execute(LinkedList<Instruction> variables, LinkedList<Instruction> instructions)
        {
            Interpreter.Environment global = new Interpreter.Environment(null);

            foreach(var variable in variables) 
            {
                object exec = variable.execute(global);
            }
            
            foreach (var instruction in instructions)
            {
                try 
                { 
                    object execution = instruction.execute(global);
                //resultsList.Add(execution.ToString());
                }
                catch(Exception ex) 
                {
                    errorsList.Add(ex.Message);
                }
            }
        }

        public LinkedList<Instruction> instructions(ParseTreeNode actual)
        {
            LinkedList<Instruction> listaInstrucciones = new LinkedList<Instruction>();
            foreach (ParseTreeNode nodo in actual.ChildNodes)
            {
                listaInstrucciones.AddLast(instruction(nodo));
            }
            return listaInstrucciones;
        }

        public Instruction instruction(ParseTreeNode actual)
        {
            string operationToken = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            switch (operationToken)
            {
                case "var":
                    if (actual.ChildNodes.Count == 5)
                    {
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), expression(actual.ChildNodes[3]));
                    }
                    else 
                    {
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), newLiteralWithDefaultValue(actual.ChildNodes[1].ChildNodes[2]));
                    }
                case "writeln":
                    return new Writeln(expression(actual.ChildNodes.ElementAt(2)));
                case "write":
                    return new Write(expression(actual.ChildNodes.ElementAt(2)));
                case "instruccion_if_sup":
                    if (actual.ChildNodes[0].ChildNodes.Count == 1) 
                    {
                        return new If(expression(actual.ChildNodes[0].ChildNodes[0].ChildNodes[2]), instructions(actual.ChildNodes[0].ChildNodes[0].ChildNodes[6]), null);
                    }
                    else 
                    {
                        return new If(expression(actual.ChildNodes[0].ChildNodes[0].ChildNodes[2]), instructions(actual.ChildNodes[0].ChildNodes[0].ChildNodes[6]), instructions(actual.ChildNodes[0].ChildNodes[1].ChildNodes[2]));
                    }
                case "while":
                    return new While(expression(actual.ChildNodes[1]), instructions(actual.ChildNodes[4]));
                default: 
                    if (actual.ChildNodes.Count == 5) //var assignment
                    {
                        return new Assign(actual.ChildNodes[0].Token.Text, expression(actual.ChildNodes[3]));
                    }
                    else 
                    {
                        //TODO array assignment
                        return null;
                    }
                    //case "while":
                    //    return new while(expression(actual))
                    /*case "mientras":
                        return new Mientras(expresion_logica(actual.ChildNodes.ElementAt(2)), instrucciones(actual.ChildNodes.ElementAt(5)));
                    case "numero":
                        string tokenValor = actual.ChildNodes.ElementAt(1).ToString().Split(' ')[0];
                        return new Declaracion(tokenValor, Simbolo.Tipo.NUMERO);
                    case "if":
                        if (actual.ChildNodes.Count == 7)
                        {
                            return new If(expresion_logica(actual.ChildNodes.ElementAt(2)), instrucciones(actual.ChildNodes.ElementAt(5)));
                        }
                        else
                        {
                            return new If(expresion_logica(actual.ChildNodes.ElementAt(2)), instrucciones(actual.ChildNodes.ElementAt(5)), instrucciones(actual.ChildNodes.ElementAt(9)));
                        }
                    default:
                        if (actual.ChildNodes.Count == 3)
                        {
                            tokenValor = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0];
                            return new Asignacion(tokenValor, expresion_numerica(actual));
                        }
                        else
                        {
                            tokenValor = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0];
                            return new Asignacion(tokenValor, expresion_numerica(actual.ChildNodes.ElementAt(2)));
                        }*/
            }

            return null;
        }

        public Literal newLiteralWithDefaultValue(ParseTreeNode actual) 
        {
            switch (actual.ChildNodes[0].Token.Terminal.Name.ToLower())
            {
                case "integer":
                    return new Literal(Types.INTEGER, 0);
                case "string":
                    return new Literal(Types.STRING, "");
                case "real":
                    return new Literal(Types.REAL, 0.0);
                case "boolean":
                    return new Literal(Types.BOOLEAN, false);
                default:
                    throw new PascalError(0, 0, "the obtained literal doesn´t have a valid type", "Semantic Error");
            }
        }
        public Expression expression(ParseTreeNode actual)
        {
            if (actual.ChildNodes.Count == 3)
            {
                string op = actual.ChildNodes[1].Token.Text;
                switch (op)
                {
                    case "+":
                        return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "+");
                    case "-":
                        return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "-");
                    case "*":
                        return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "*");
                    case "/":
                        return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "/");
                    case "%":
                        return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "%");
                    case ",":
                        return new StringOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ",");
                    case "=":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "=");
                    case "<>":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<>");
                    case ">":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ">");
                    case ">=":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ">=");
                    case "<":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<");
                    case "<=":
                        return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<=");
                    default:
                        return null;
                    
                }
            }
            else
            {
                switch (actual.ChildNodes[0].Token.Terminal.Name.ToLower()) 
                {
                    case "integer":
                        return new Literal(Types.INTEGER, (object)actual.ChildNodes[0].Token.Text);
                    case "string":
                        return new Literal(Types.STRING, (object)actual.ChildNodes[0].Token.Text);
                    case "real":
                        return new Literal(Types.REAL, (object)actual.ChildNodes[0].Token.Text);
                    case "boolean":
                        return new Literal(Types.BOOLEAN, (object)actual.ChildNodes[0].Token.Text);
                    case "identifier":
                        return new Literal(Types.IDENTIFIER, actual.ChildNodes[0].Token.Text);
                    default:
                        throw new PascalError(0, 0, "the obtained literal doesn´t have a valid type", "Semantic Error");
                }
            }
        }
        //public Operation expresion_cadena(ParseTreeNode actual)
        //{
        //    if (actual.ChildNodes.Count == 3)
        //    {
        //        return new Operacion(expresion_cadena(actual.ChildNodes.ElementAt(0)), expresion_cadena(actual.ChildNodes.ElementAt(2)), Operacion.Tipo_operacion.CONCATENACION);
        //    }
        //    else
        //    {
        //        String tokenValor = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0];
        //        if (tokenValor.Equals("expresion_numerica"))
        //        {
        //            return expresion_numerica(actual.ChildNodes.ElementAt(0));
        //        }
        //        else
        //        {
        //            tokenValor = actual.ChildNodes.ElementAt(0).ToString();
        //            return new Operacion(tokenValor.Remove(tokenValor.ToCharArray().Length - 8, 8), Operacion.Tipo_operacion.CADENA);
        //        }
        //    }
        //}

        public void generateGraph(ParseTreeNode raiz)
        {
            string graphDot = Grapher.getDot(raiz);
            string path = "ast.txt";
            try
            {
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(graphDot);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
