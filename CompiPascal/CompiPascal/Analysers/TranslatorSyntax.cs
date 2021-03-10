using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CompiPascal.Analysers
{
    class TranslatorSyntax
    {
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
                //LinkedList<Instruction> instructionsList = instructions(root.ChildNodes[2].ChildNodes[4]);
                //execute(variableDeclaration, functionAndProcedureDeclaration, instructionsList);

          

            }
            else
            {
                //We print the errors in the output 
            }

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
