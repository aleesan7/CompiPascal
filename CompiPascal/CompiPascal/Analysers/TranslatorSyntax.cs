using CompiPascal.Interpreter;
using CompiPascal.Utils;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CompiPascal.Analysers
{
    class TranslatorSyntax
    {
        public List<string> resultsList = new List<string>();
        public List<CompiPascal.Utils.PascalError> errorsList = new List<CompiPascal.Utils.PascalError>();
        public void Analyze(string input)
        {

            TranslatorGrammar grammar = new TranslatorGrammar();
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

                //LinkedList<Instruction> variableDeclaration = instructions(root.ChildNodes[2].ChildNodes[1]);
                //LinkedList<Instruction> functionAndProcedureDeclaration = instructions(root.ChildNodes[2].ChildNodes[2].ChildNodes[0].ChildNodes[0]);
                LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                execute(null, null, instructionsList);

          

            }
            else
            {
                //We print the errors in the output 
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

        public LinkedList<Expression> expressions(ParseTreeNode actual)
        {
            LinkedList<Expression> expressionList = new LinkedList<Expression>();
            foreach (ParseTreeNode node in actual.ChildNodes)
            {
                expressionList.AddLast(expression(node));
            }

            return expressionList;
        }

        public void execute(LinkedList<Instruction> variables, LinkedList<Instruction> functionsAndProcedures, LinkedList<Instruction> instructions)
        {
            Interpreter.Environment global = new Interpreter.Environment(null);
            global.SetEnvName("global");

            //foreach (var variable in variables)
            //{
            //    object exec = variable.executeTranslate(global);
            //}

            //foreach (var funcOrProc in functionsAndProcedures)
            //{
            //    object exec = funcOrProc.executeTranslate(global);
            //}

            foreach (var instruction in instructions)
            {
                try
                {
                    object execution = instruction.executeTranslate(global);
                }
                catch (CompiPascal.Utils.PascalError ex)
                {
                    errorsList.Add(ex);
                }
            }
        }

        public Instruction instruction(ParseTreeNode actual)
        {
            string operationToken = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            switch (operationToken)
            {
                case "var":
                    if (actual.ChildNodes.Count == 5)
                    {
                        //return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), expression(actual.ChildNodes[3]), actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                        return null;
                    }
                    else
                    {
                        if (actual.ChildNodes.Count == 3)
                        {
                            if (actual.ChildNodes[1].ChildNodes.Count == 1)
                            {
                                string type = actual.ChildNodes[1].ChildNodes[0].ChildNodes[7].ChildNodes[0].Token.Text.ToString();


                                return new ArrayDeclare(actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text.ToString(), new Interpreter.Type(GetVarType(type), null), ranges(actual.ChildNodes[1].ChildNodes[0].ChildNodes[4]), actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Span.Location.Column);
                            }
                            else
                            {
                                //return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), newLiteralWithDefaultValue(actual.ChildNodes[1].ChildNodes[2]), actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                case "writeln":
                    //return new Writeln(expression(actual.ChildNodes.ElementAt(2)));
                    return new Writeln(expressions(actual.ChildNodes[2]));
                case "write":
                    //return new Write(expression(actual.ChildNodes.ElementAt(2)));
                    return new Write(expressions(actual.ChildNodes[2]));
                
                default:
                    return null;
            }
        }

        public Expression expression(ParseTreeNode actual)
        {
            if (actual.ChildNodes.Count == 3)
            {
                if (actual.ChildNodes[0].Token != null && actual.ChildNodes[2].Token != null)
                {
                    if (!(actual.ChildNodes[0].Token.Text.Equals("(") && actual.ChildNodes[2].Token.Text.Equals(")")))
                    {
                        string op = actual.ChildNodes[1].Token.Text;
                        return ArithmeticOrLogicOperation(op, actual);
                    }
                    else
                    {
                        return expression(actual.ChildNodes[1]);
                    }
                }
                else
                {
                    string op = actual.ChildNodes[1].Token.Text;
                    return ArithmeticOrLogicOperation(op, actual);
                }
            }
            else
            {
                if (!actual.ChildNodes[0].Term.Name.ToString().Equals("functionOrProcedureCall"))
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
                            if (actual.ChildNodes.Count == 1)  // Simple var expression
                            {
                                return new Literal(Types.IDENTIFIER, actual.ChildNodes[0].Token.Text);
                            }
                            else
                            {
                                //Array var expression
                                return new ArrayAccess(actual.ChildNodes[0].Token.Text, expressions(actual.ChildNodes[2]));
                            }

                        default:
                            throw new PascalError(actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column, "the obtained literal doesn´t have a valid type", "Semantic Error");
                    }
                }
                else
                {
                    if (actual.ChildNodes[0].ChildNodes[2].ChildNodes.Count > 0)
                    {
                        return new FunctionCallExpression(actual.ChildNodes[0].ChildNodes[0].Token.Text, expressions(actual.ChildNodes[0].ChildNodes[2]));
                    }
                    else
                    {
                        return new FunctionCallExpression(actual.ChildNodes[0].ChildNodes[0].Token.Text);
                    }
                }
            }
        }

        public Expression ArithmeticOrLogicOperation(string op, ParseTreeNode actual)
        {
            switch (op)
            {
                case "+":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "+", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "-":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "-", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "*":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "*", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "/":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "/", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "%":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "%", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case ",":
                    return new StringOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ",", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "=":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "=", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "<>":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<>", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case ">":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ">", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case ">=":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), ">=", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "<":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "<=":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "<=", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "AND":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "&&", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "OR":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "||", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                default:
                    return null;

            }
        }

        private Types GetVarType(string type)
        {
            switch (type.ToLower())
            {
                case "integer":
                    return Types.INTEGER;
                case "string":
                    return Types.STRING;
                case "real":
                    return Types.REAL;
                case "boolean":
                    return Types.BOOLEAN;
                default:
                    return Types.ERROR;
            }
        }

        public Literal newLiteralWithDefaultValue(ParseTreeNode actual)
        {
            switch (actual.ChildNodes[0].Token.Terminal.Name.ToLower())
            {
                case "integer":
                    return new Literal(Types.INTEGER, (object)0);
                case "string":
                    return new Literal(Types.STRING, (object)"");
                case "real":
                    return new Literal(Types.REAL, (object)0.0);
                case "boolean":
                    return new Literal(Types.BOOLEAN, (object)false);
                default:
                    throw new PascalError(actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column, "the obtained literal doesn´t have a valid type", "Semantic Error");
            }
        }

        public LinkedList<Interpreter.Range> ranges(ParseTreeNode actual)
        {
            LinkedList<Interpreter.Range> rangesList = new LinkedList<Interpreter.Range>();
            foreach (ParseTreeNode node in actual.ChildNodes)
            {
                rangesList.AddLast(range(node));
            }

            return rangesList;
        }

        public Interpreter.Range range(ParseTreeNode actual)
        {
            return new Interpreter.Range(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]));
        }

        public void generateGraph(ParseTreeNode raiz)
        {
            string graphDot = Grapher.getDot(raiz);
            string path = "astTranslator.txt";
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
