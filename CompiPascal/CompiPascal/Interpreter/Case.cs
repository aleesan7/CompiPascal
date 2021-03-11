using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Case : Instruction
    {
        Expression expr;
        LinkedList<Instruction> caseElements;

        public Case(Expression expr, LinkedList<Instruction> caseElements)
        {
            this.expr = expr;
            this.caseElements = caseElements;
            this.results = new LinkedList<string>();
        }

        public override object execute(Environment env)
        {
            foreach(Instruction caseElement in this.caseElements) 
            {
                if (caseElement.GetType().Name.ToString().ToLower().Equals("case_element"))
                {
                    Case_element tempCaseElement = (Case_element)caseElement;
                    Expression tempExpr = tempCaseElement.GetCondition();
                    LogicOperation newExpr = new LogicOperation(this.expr, tempExpr, "=", 0, 0);

                    tempCaseElement.SetCondition(newExpr);

                    tempCaseElement.execute(env);

                    if (tempCaseElement.results.Count > 0)
                    {
                        foreach (string result in tempCaseElement.results)
                        {
                            this.results.AddLast(result);
                        }
                        tempCaseElement.results.Clear();
                    }
                }
            }

            return null;
        }

        public override string executeTranslate(Environment env)
        {
            string caseContent = string.Empty;
            string caseElements = string.Empty;

            if (this.caseElements.Count > 0)
            {
                foreach (Case_element case_element in this.caseElements)
                {
                    caseElements += case_element.executeTranslate(env) + System.Environment.NewLine;
                }

            }

            caseContent += "case " + this.expr.evaluateTranslate(env) + " of" + System.Environment.NewLine;
            caseContent += "\t" + caseElements;
            caseContent += "end;" + System.Environment.NewLine;

            return caseContent;
        }
    }
}
