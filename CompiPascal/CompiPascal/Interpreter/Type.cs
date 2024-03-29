﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    public enum Types
    {
        INTEGER = 0,
        REAL = 1,
        STRING = 2,
        BOOLEAN = 3,
        IDENTIFIER = 4,
        ERROR = 5,
        TYPE = 6
    }
    class Type
    {
        public Types type;
        public string auxType;

        public Type(Types type, string auxType)
        {
            this.type = type;
            this.auxType = auxType;
        }
    }
}
