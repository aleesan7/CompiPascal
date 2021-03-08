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
                }
            }

            return null;
        }
    }
}
