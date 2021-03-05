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
                LinkedList<Instruction> functionAndProcedureDeclaration = instructions(root.ChildNodes[2].ChildNodes[2].ChildNodes[0].ChildNodes[0]);
                LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                execute(variableDeclaration, functionAndProcedureDeclaration, instructionsList);

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

        public void execute(LinkedList<Instruction> variables, LinkedList<Instruction> functionsAndProcedures, LinkedList<Instruction> instructions)
        {
            Interpreter.Environment global = new Interpreter.Environment(null);
            global.SetEnvName("global");

            foreach (var variable in variables)
            {
                object exec = variable.execute(global);
            }

            foreach (var funcOrProc in functionsAndProcedures) 
            {
                object exec = funcOrProc.execute(global);
            }

            foreach (var instruction in instructions)
            {
                try
                {
                    object execution = instruction.execute(global);
                    if (execution != null)
                    {
                        resultsList.Add(execution.ToString());
                    }
                }
                catch (Exception ex)
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

        public LinkedList<Instruction> caseelements(ParseTreeNode actual) 
        {
            LinkedList<Instruction> caseElementsList = new LinkedList<Instruction>();
            foreach(ParseTreeNode node in actual.ChildNodes) 
            {
                caseElementsList.AddLast(caseElement(node));
            }

            return caseElementsList;
        }

        public LinkedList<Declare> declares(ParseTreeNode actual) 
        {
            LinkedList<Declare> listaDeclares = new LinkedList<Declare>();
            foreach(ParseTreeNode node in actual.ChildNodes)
            {
                listaDeclares.AddLast(declare(node));
            }

            return listaDeclares;
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

        public Instruction caseElement(ParseTreeNode actual) 
        {
            return new Case_element(expression(actual.ChildNodes[0]), instruction(actual.ChildNodes[2]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
        }

        public Instruction instruction(ParseTreeNode actual)
        {
            string operationToken = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            switch (operationToken)
            {
                case "var":
                    if (actual.ChildNodes.Count == 5)
                    {
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), expression(actual.ChildNodes[3]), actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                    }
                    else
                    {
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), newLiteralWithDefaultValue(actual.ChildNodes[1].ChildNodes[2]), actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                    }
                case "writeln":
                    //return new Writeln(expression(actual.ChildNodes.ElementAt(2)));
                    return new Writeln(expressions(actual.ChildNodes[2]));
                case "write":
                    //return new Write(expression(actual.ChildNodes.ElementAt(2)));
                    return new Write(expressions(actual.ChildNodes[2]));
                case "instruccion_if_sup":
                    if (actual.ChildNodes[0].ChildNodes.Count == 1)
                    {
                        return new If(expression(actual.ChildNodes[0].ChildNodes[0].ChildNodes[2]), instructions(actual.ChildNodes[0].ChildNodes[0].ChildNodes[6]), null, actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                    else
                    {
                        return new If(expression(actual.ChildNodes[0].ChildNodes[0].ChildNodes[2]), instructions(actual.ChildNodes[0].ChildNodes[0].ChildNodes[6]), instructions(actual.ChildNodes[0].ChildNodes[1].ChildNodes[2]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                case "while":
                    return new While(expression(actual.ChildNodes[1]), instructions(actual.ChildNodes[4]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "repeat":
                    return new Repeat(expression(actual.ChildNodes[3]), instructions(actual.ChildNodes[1]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "for":
                    return new For(new Assign(actual.ChildNodes[1].Token.Text, expression(actual.ChildNodes[4])), 
                        new LogicOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), newLiteralWithSetedValue(actual.ChildNodes[6]), "<=", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column), 
                        new Assign(actual.ChildNodes[1].Token.Text, new ArithmeticOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), new Literal(Types.INTEGER, (object)"1"), "+", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column)),
                        instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "break":
                    return new Break("break");
                case "continue":
                    return new Continue("continue");
                case "exit":
                    return new Exit("return", expression(actual.ChildNodes[2]));
                case "function":
                    if (actual.ChildNodes.Count == 12) 
                    {
                        //function with only instructions
                        return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[5].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                    else 
                    {
                        //TODO functions with parameters
                        return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[6].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[10]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                case "procedure":
                    if (actual.ChildNodes.Count == 10)
                    {
                        //Procedure with only instructions
                        if (actual.ChildNodes[5].ChildNodes.Count == 0) 
                        {
                            return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[7]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        else 
                        {
                            //Procedure without parameters but with local vars and instructions
                            return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[5]), instructions(actual.ChildNodes[7]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        
                    }
                    else 
                    {
                        //Procedure with parameters, local variables and instructions
                        return new Procedure(actual.ChildNodes[1].Token.Text, declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[6]), instructions(actual.ChildNodes[8]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                case "functionorprocedurecall":
                    if (actual.ChildNodes[0].ChildNodes.Count == 4)
                    {
                        return new FunctionProcedureCall(actual.ChildNodes[0].ChildNodes[0].Token.Text);
                    }
                    else 
                    {
                        //TODO procedure or function call with parameters
                        return new FunctionProcedureCall(actual.ChildNodes[0].ChildNodes[0].Token.Text, expressions(actual.ChildNodes[0].ChildNodes[2]));
                    }
                case "case_statement":
                    return new Case(expression(actual.ChildNodes[0].ChildNodes[1]), caseelements(actual.ChildNodes[0].ChildNodes[3]));
                case "graficar_ts":
                    return new Graphts();
                default:
                    
                    if (actual.ChildNodes.Count == 5  || actual.ChildNodes.Count == 4) //var assignment
                    {
                        return new Assign(actual.ChildNodes[0].Token.Text, expression(actual.ChildNodes[3]));
                    }
                    else
                    {
                        //TODO array assignment
                        return null;
                    }
            }
        }

        private Declare declare(ParseTreeNode actual) 
        {
            if (actual.ChildNodes.Count == 5) 
            {
                return new Declare(actual.ChildNodes[1].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[3]), actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
            }
            else 
            {
                return new Declare(actual.ChildNodes[0].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[2]), actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
            }
            
        }

        private Function.FunctionTypes GetFunctionType(string type)
        {
            switch (type.ToLower()) 
            {
                case "integer":
                    return Function.FunctionTypes.INTEGER;
                case "string":
                    return Function.FunctionTypes.STRING;
                case "real":
                    return Function.FunctionTypes.REAL;
                case "boolean":
                    return Function.FunctionTypes.BOOLEAN;
                default:
                    return Function.FunctionTypes.VOID;
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

        public Literal newLiteralWithSetedValue(ParseTreeNode actual) 
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
                    throw new PascalError(actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column, "the obtained literal doesn´t have a valid type", "Semantic Error");
            }
        }

        public Literal newLiteralWithSetedValue(ParseTreeNode actual, string value)
        {
            switch (actual.ChildNodes[0].Token.Terminal.Name.ToLower())
            {
                case "integer":
                    return new Literal(Types.INTEGER, (object)value);
                case "string":
                    return new Literal(Types.STRING, (object)value);
                case "real":
                    return new Literal(Types.REAL, (object)value);
                case "boolean":
                    return new Literal(Types.BOOLEAN, (object)value);
                case "identifier":
                    return new Literal(Types.IDENTIFIER, (object)value);
                default:
                    throw new PascalError(actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column, "the obtained literal doesn´t have a valid type", "Semantic Error");
            }
        }

        public Expression expression(ParseTreeNode actual)
        {
            if (actual.ChildNodes.Count == 3)
            {
                if(actual.ChildNodes[0].Token != null && actual.ChildNodes[2].Token != null) 
                { 
                    if(!(actual.ChildNodes[0].Token.Text.Equals("(") && actual.ChildNodes[2].Token.Text.Equals(")"))) 
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
                            return new Literal(Types.IDENTIFIER, actual.ChildNodes[0].Token.Text);
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
                default:
                    return null;

            }
        }
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
