﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    abstract class Instruction
    {
        public abstract object execute(Environment env);

        public abstract string executeTranslate(Environment env);
    }
}
