﻿using CompiPascal.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class LogicOperation : Expression
    {
        private Expression left;
        private Expression right;
        private string tipoOperacion;
        public int line;
        public int column;

        public LogicOperation(Expression left, Expression right, string tipoOperacion, int line, int column)
        {
            this.left = left;
            this.right = right;
            this.tipoOperacion = tipoOperacion;
            this.line = line;
            this.column = column;
        }
        public override Symbol evaluate(Environment env)
        {
            Symbol left = this.left.evaluate(env);
            Symbol right = (this.right!=null) ? this.right.evaluate(env) : null;
            
            
            Symbol result = null;
            Type type = new Type(Types.BOOLEAN, null);

            Types resultantType = (this.right!=null) ? TypesTable.getType(left.type, right.type) : Types.BOOLEAN;
            if (resultantType == Types.ERROR)
                throw new PascalError(this.line, this.column, "Incorrect data type", "Semantic");

            switch (tipoOperacion)
            {
                case "=":
                    if(resultantType == Types.BOOLEAN) 
                    {
                        result = new Symbol(left.ToString().Equals(right.ToString()), type, null, this.line, this.column);
                    }
                    else 
                    {
                        result = new Symbol(double.Parse(left.ToString()) == double.Parse(right.ToString()), type, null, this.line, this.column);
                    }
                    return result;
                case "<>":
                    result = new Symbol(double.Parse(left.ToString()) != double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case ">":
                    result = new Symbol(double.Parse(left.ToString()) > double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case "<":
                    result = new Symbol(double.Parse(left.ToString()) < double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case ">=":
                    result = new Symbol(double.Parse(left.ToString()) >= double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case "<=":
                    result = new Symbol(double.Parse(left.ToString()) <= double.Parse(right.ToString()), type, null, this.line, this.column);
                    return result;
                case "&&":
                    if (resultantType == Types.BOOLEAN)
                    {
                        result = new Symbol(bool.Parse(left.value.ToString()) && bool.Parse(right.value.ToString()), type, null, this.line, this.column);
                    }
                    return result;
                case "||":
                    if (resultantType == Types.BOOLEAN)
                    {
                        result = new Symbol((bool)left.value || (bool)right.value, type, null, this.line, this.column);
                    }
                    return result;
                case "!":
                    result = new Symbol(!bool.Parse(left.value.ToString()), type, null, this.line, this.column);
                    return result;
            }
            return null;
        }

        public override string evaluateTranslate(Environment env)
        {
            string left = this.left.evaluateTranslate(env);
            string right = this.right.evaluateTranslate(env);

            switch (tipoOperacion)
            {
                case "=":
                    return left + " = " + right;
                case "<>":
                    return left + " <> " + right;
                case ">":
                    return left + " > " + right;
                case "<":
                    return left + " < " + right;
                case ">=":
                    return left + " >= " + right;
                case "<=":
                    return left + " <= " + right;
                case "&&":
                    return left + " && " + right;
                case "||":
                    return left + " || " + right;
            }
            return null;
        }
    }
}
