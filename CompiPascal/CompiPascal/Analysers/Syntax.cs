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
        public List<string> translateResultsList = new List<string>();
        public List<CompiPascal.Utils.PascalError> errorsList = new List<CompiPascal.Utils.PascalError>();
        public string strResult = "";
        public bool syntacticErrorsFound = false;
        public Interpreter.Environment globalEnv;

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

                LinkedList<Instruction> constantDefinition = instructions(root.ChildNodes[2].ChildNodes[0]);
                LinkedList<Instruction> variableDeclaration = instructions(root.ChildNodes[2].ChildNodes[1]);
                LinkedList<Instruction> functionAndProcedureDeclaration = instructions(root.ChildNodes[2].ChildNodes[2].ChildNodes[0].ChildNodes[0]);
                LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                execute(constantDefinition, variableDeclaration, functionAndProcedureDeclaration, instructionsList);


            }
            else
            {
                //We print the errors in the output 
                syntacticErrorsFound = true;
            }

        }

        public void AnalyzeTranslator(string input)
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

                LinkedList<Instruction> constantDefinition = instructions(root.ChildNodes[2].ChildNodes[0]);
                LinkedList<Instruction> variableDeclaration = instructions(root.ChildNodes[2].ChildNodes[1]);
                LinkedList<Instruction> functionAndProcedureDeclaration = instructions(root.ChildNodes[2].ChildNodes[2].ChildNodes[0].ChildNodes[0]);
                LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                executeTranslator(variableDeclaration, functionAndProcedureDeclaration, instructionsList);


            }
            else
            {
                //We print the errors in the output 
            }

        }


        public void execute(LinkedList<Instruction> constants, LinkedList<Instruction> variables, LinkedList<Instruction> functionsAndProcedures, LinkedList<Instruction> instructions)
        {
            Interpreter.Environment global = new Interpreter.Environment(null);
            global.SetEnvName("global");

            this.globalEnv = global;

            foreach(var constant in constants) 
            {
                constant.execute(global);
            }

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
                }
                catch (CompiPascal.Utils.PascalError ex)
                {
                    errorsList.Add(ex);
                }
            }
        }

        public void executeTranslator(LinkedList<Instruction> variables, LinkedList<Instruction> functionsAndProcedures, LinkedList<Instruction> instructions)
        {
            Interpreter.Environment global = new Interpreter.Environment(null);
            string execution = string.Empty;
            global.SetEnvName("global");

            foreach (var variable in variables)
            {
                execution += variable.executeTranslate(global);
            }

            execution += System.Environment.NewLine;

            foreach (var funcOrProc in functionsAndProcedures)
            {
                execution += funcOrProc.executeTranslate(global);
            }

            if (global.GetCodes().Count > 0) 
            {
                foreach(string code in global.GetCodes()) 
                {
                    execution += code + System.Environment.NewLine;
                }
            }

            this.translateResultsList.Add("begin");
            execution += "begin" + System.Environment.NewLine;

            foreach (var instruction in instructions)
            {
                try
                {
                    execution = execution  + instruction.executeTranslate(global);
                }
                catch (CompiPascal.Utils.PascalError ex)
                {
                    errorsList.Add(ex);
                }
            }

            this.translateResultsList.Add("end.");
            execution += "end." + System.Environment.NewLine;

            this.strResult = execution;
        }

        public LinkedList<Instruction> instructions(ParseTreeNode actual)
        {
            LinkedList<Instruction> listaInstrucciones = new LinkedList<Instruction>();
            foreach (ParseTreeNode nodo in actual.ChildNodes)
            {
                if (actual.Term.Name.ToString().Equals("proc_or_func_declaration")) 
                {
                    if (nodo.ChildNodes[0].Term.Name.ToString().Equals("procedure")) 
                    {
                        //If we have nested procedures or functions, then we need to create nested procs or funcs
                        if(nodo.ChildNodes[6].Term.Name.ToString().Equals("proc_or_func_declaration") 
                            || nodo.ChildNodes[7].Term.Name.ToString().Equals("proc_or_func_declaration")) 
                        {
                            listaInstrucciones.AddLast(specialInstruction(nodo));
                        }
                        else 
                        {
                            listaInstrucciones.AddLast(instruction(nodo));
                        }
                    }
                    else 
                    {
                        if (nodo.ChildNodes[0].Term.Name.ToString().Equals("function")) //Validate that we don´t have a child node for nested functions
                        {
                            listaInstrucciones.AddLast(instruction(nodo));
                        }
                    }
                }
                else 
                {
                    listaInstrucciones.AddLast(instruction(nodo));
                }

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

        public LinkedList<Interpreter.Range> ranges(ParseTreeNode actual) 
        {
            LinkedList<Interpreter.Range> rangesList = new LinkedList<Interpreter.Range>();
            foreach(ParseTreeNode node in actual.ChildNodes) 
            {
                rangesList.AddLast(range(node));
            }

            return rangesList;
        }

        public Instruction caseElement(ParseTreeNode actual) 
        {
            if(actual.ChildNodes.Count > 5) 
            {
                return new Case_element(expression(actual.ChildNodes[0]), instructions(actual.ChildNodes[3]), expression(actual.ChildNodes[0]), false, actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
            }
            else 
            {
                return new Case_element(new Literal(Types.BOOLEAN, true), instructions(actual.ChildNodes[2]), new Literal(Types.BOOLEAN, true), true, actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
            }
            
        }

        public Interpreter.Range range (ParseTreeNode actual) 
        {
            return new Interpreter.Range(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]));
        }

        public LinkedList<Procedure> procedures(ParseTreeNode actual)
        {
            LinkedList<Procedure> proceduresList = new LinkedList<Procedure>();
            foreach (ParseTreeNode node in actual.ChildNodes)
            {
                proceduresList.AddLast(procedure(node));
            }

            return proceduresList;
        }

        public Procedure procedure(ParseTreeNode actual)
        {
            //proc with instructions
            //proc with instructions + local vars
            //proc with instructions + local vars + parameters
            //proc with instructions + local vars + parameters + nested procs

            if (actual.ChildNodes.Count == 12)
            {
                // proc with all: parameters, local vars, nested procs and instructions
                if (actual.ChildNodes[6].ChildNodes.Count > 0 && actual.ChildNodes[3].ChildNodes.Count > 0 && actual.ChildNodes[7].ChildNodes[0].ChildNodes.Count> 0)
                {
                    return new Procedure(actual.ChildNodes[1].Token.Text, declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[6]), instructions(actual.ChildNodes[9]), procedures(actual.ChildNodes[7]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                }
            }
            else
            {
                //Proc without parameters but with local vars and instructions and nested proc
                if(actual.ChildNodes[5].ChildNodes.Count != 0 && actual.ChildNodes[6].ChildNodes.Count>0) 
                {
                    return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[5]), instructions(actual.ChildNodes[8]), procedures(actual.ChildNodes[6]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                }
                else 
                {
                    //proc without parameters, but with local vars and instructions, NO nested procs
                    if(actual.ChildNodes[5].ChildNodes.Count != 0 && actual.ChildNodes[6].ChildNodes.Count == 0) 
                    {
                        return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[5]), instructions(actual.ChildNodes[8]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                }
            }

            ////return new Procedure(actual.ChildNodes[0].Token.Text, )

            return null;
        }

        private Instruction specialInstruction(ParseTreeNode actual)
        {
            string operationToken = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            switch (operationToken) 
            {
                case "procedure":
                    if (actual.ChildNodes.Count == 11) //11 child nodes procedure
                    {
                        if(actual.ChildNodes[6].ChildNodes.Count > 0) 
                        {
                            return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[5]), instructions(actual.ChildNodes[8]), procedures(actual.ChildNodes[6]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        else 
                        {
                            return new Procedure(actual.ChildNodes[1].Token.Text, instructions(actual.ChildNodes[5]), instructions(actual.ChildNodes[8]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        
                    }
                    else 
                    {
                        //12 child nodes procedure
                        return new Procedure(actual.ChildNodes[1].Token.Text, declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[6]), instructions(actual.ChildNodes[9]), procedures(actual.ChildNodes[7]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                default:
                    return null;
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
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), expression(actual.ChildNodes[3]), new Interpreter.Type(GetVarType(actual.ChildNodes[1].ChildNodes[2].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                    }
                    else
                    {
                        if(actual.ChildNodes.Count == 3) 
                        {
                            if(actual.ChildNodes[1].ChildNodes.Count == 1) 
                            {
                                string type = actual.ChildNodes[1].ChildNodes[0].ChildNodes[7].ChildNodes[0].Token.Text.ToString();


                                return new ArrayDeclare(actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text.ToString(), new Interpreter.Type(GetVarType(type), null), ranges(actual.ChildNodes[1].ChildNodes[0].ChildNodes[4]), actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].ChildNodes[0].Span.Location.Column);
                            }
                            else 
                            {
                                return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), newLiteralWithDefaultValue(actual.ChildNodes[1].ChildNodes[2]), new Interpreter.Type(GetVarType(actual.ChildNodes[1].ChildNodes[2].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
                            }
                        }
                        else 
                        {
                            return null;
                        }
                    }
                case "const":
                    if (actual.ChildNodes.Count == 5)
                    {
                        return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), expression(actual.ChildNodes[3]), new Interpreter.Type(GetVarType(actual.ChildNodes[1].ChildNodes[2].ChildNodes[0].Token.Text), null), true, actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
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
                                return new Declare(actual.ChildNodes[1].ChildNodes[0].Token.Text.ToString(), newLiteralWithDefaultValue(actual.ChildNodes[1].ChildNodes[2]), new Interpreter.Type(GetVarType(actual.ChildNodes[1].ChildNodes[2].ChildNodes[0].Token.Text), null), true, actual.ChildNodes[1].ChildNodes[0].Span.Location.Line, actual.ChildNodes[1].ChildNodes[0].Span.Location.Column);
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
                    if (actual.ChildNodes[5].Term.Name.ToString().Equals("downto")) 
                    {
                        return new For(new Assign(actual.ChildNodes[1].Token.Text, expression(actual.ChildNodes[4])),
                                    new LogicOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), expression(actual.ChildNodes[6]) /*(newLiteralWithSetedValue(actual.ChildNodes[6])*/, ">=", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column),
                                    new Assign(actual.ChildNodes[1].Token.Text, new ArithmeticOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), new Literal(Types.INTEGER, (object)"1"), "-", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column)),
                                    instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                    else 
                    {
                        return new For(new Assign(actual.ChildNodes[1].Token.Text, expression(actual.ChildNodes[4])),
                                    new LogicOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), expression(actual.ChildNodes[6]) /*(newLiteralWithSetedValue(actual.ChildNodes[6])*/, "<=", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column),
                                    new Assign(actual.ChildNodes[1].Token.Text, new ArithmeticOperation(new Literal(Types.IDENTIFIER, actual.ChildNodes[1].Token.Text), new Literal(Types.INTEGER, (object)"1"), "+", actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column)),
                                    instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                    }
                case "break":
                    return new Break("break");
                case "continue":
                    return new Continue("continue");
                case "exit":
                    return new Exit("return", expression(actual.ChildNodes[2]));
                case "function":
                    if (actual.ChildNodes.Count == 12) 
                    {
                        
                        if(actual.ChildNodes[7].ChildNodes.Count> 0) 
                        {
                            //function with local vars and instructions
                            return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[5].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), instructions(actual.ChildNodes[7]), instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        else 
                        {
                            //function with only instructions
                            return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[5].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), instructions(actual.ChildNodes[9]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
                        
                    }
                    else 
                    {
                        
                        if (actual.ChildNodes[8].ChildNodes.Count > 0) 
                        {
                            if(actual.ChildNodes[8].ChildNodes.Count>0 && actual.ChildNodes[3].ChildNodes.Count == 0) 
                            {
                                //function with local vars and instructions but no parameters
                                return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[6].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), instructions(actual.ChildNodes[8]), instructions(actual.ChildNodes[10]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                            }
                            else 
                            {
                                //function with local vars and parameters
                                return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[6].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), instructions(actual.ChildNodes[8]), declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[10]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                            }
                            
                        }
                        else 
                        {
                            //function with parameters and instructions
                            return new Function(actual.ChildNodes[1].Token.Text, GetFunctionType(actual.ChildNodes[6].ChildNodes[0].ChildNodes[0].Token.Text.ToString()), declares(actual.ChildNodes[3]), instructions(actual.ChildNodes[10]), actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                        }
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
                        return new ArrayAssign(actual.ChildNodes[0].Token.Text, expression(actual.ChildNodes[6]), expressions(actual.ChildNodes[2]));
                    }
            }
        }

        private Declare declare(ParseTreeNode actual) 
        {
            if (actual.ChildNodes.Count == 5) 
            {
                return new Declare(actual.ChildNodes[1].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[3]), new Interpreter.Type(GetVarType(actual.ChildNodes[3].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
            }
            else 
            {
                if(actual.ChildNodes.Count == 4 && actual.ChildNodes[0].Token.Text.ToString().ToLower().Equals("var")) 
                {
                    return new Declare(actual.ChildNodes[1].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[3]), new Interpreter.Type(GetVarType(actual.ChildNodes[3].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
                }
                else 
                {
                    if(actual.ChildNodes.Count == 4) 
                    {
                        return new Declare(actual.ChildNodes[0].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[2]), new Interpreter.Type(GetVarType(actual.ChildNodes[2].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
                    }
                    else 
                    {
                       
                       return new Declare(actual.ChildNodes[0].Token.Text, newLiteralWithDefaultValue(actual.ChildNodes[2]), new Interpreter.Type(GetVarType(actual.ChildNodes[2].ChildNodes[0].Token.Text), null), false, actual.ChildNodes[1].Span.Location.Line, actual.ChildNodes[1].Span.Location.Column);
                        
                    }
                }
               
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
                if (actual.ChildNodes.Count == 2) 
                {
                    //NOT
                    return ArithmeticOrLogicOperation("NOT", actual);
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
        }

        public Expression ArithmeticOrLogicOperation(string op, ParseTreeNode actual) 
        {
            switch (op.ToLower())
            {
                case "+":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "+", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "-":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "-", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "*":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "*", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "/":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "/", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "div":
                    return new ArithmeticOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "div", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
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
                case "and":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "&&", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "or":
                    return new LogicOperation(expression(actual.ChildNodes[0]), expression(actual.ChildNodes[2]), "||", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
                case "not":
                    return new LogicOperation(expression(actual.ChildNodes[1]), null, "!", actual.ChildNodes[0].Span.Location.Line, actual.ChildNodes[0].Span.Location.Column);
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
