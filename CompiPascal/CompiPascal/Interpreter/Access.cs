﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Access : Expression
    {
        private string id;

        public Access(string id) 
        {
            this.id = id;
        }

        public override Symbol evaluate(Environment env)
        {
            Symbol variable = env.ObtainVariable(this.id);
            if (variable == null) 
            {
                throw new Exception("The variable '" + id + "' doesn´t exist.");
            }
            else 
            {
                return variable;
            }
        }
    }
}