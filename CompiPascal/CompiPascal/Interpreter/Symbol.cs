using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.Interpreter
{
    class Symbol
    {
        public object value;
        public string id;
        public Type type;
        public bool constant = false;
        public int line;
        public int column;
        private LinkedList<int> dimentions;

        public Symbol(object value, Type type, string id, int line, int column)
        {
            this.id = id;
            this.value = value;
            this.type = type;
            this.line = line;
            this.column = column;
            this.dimentions = null;
        }

        //Array constructor
        public Symbol(Type type, string id, LinkedList<int> dimentions, int line, int column)
        {
            this.id = id;
            ArrayNode node = new ArrayNode();
            node.initializeNode(dimentions.Count, 1, dimentions, type.type);
            this.value = node;
            this.type = type;
            this.line = line;
            this.column = column;
            this.dimentions = dimentions;
        }

        public void SetValue(object val, LinkedList<int> indexes)
        {
            //if (this.valor instanceof NodoArreglo){
                if (this.dimentions.Count == indexes.Count)
                {
                    ArrayNode arr = (ArrayNode)this.value;
                    arr.SetValue(indexes.Count, 1, indexes, val, id);
                }
                else
                {
                    throw new CompiPascal.Utils.PascalError(this.line, this.column, "The amount of provided indexes doesn´t "
                            + "macth with the amount of dimentions of the array " + this.id + ", a value cannot be assigned.", 
                            "Semantic");
                }
            //}else
            //{
            //    System.out.println("La variable " + id + " no es un arreglo, por lo "
            //            + "que no puede asignársele un valor de esta manera.");
            //}
        }

        public object GetValue(string id, LinkedList<int> indexes)
        {
            //if (this.valor instanceof NodoArreglo){
                if (this.dimentions.Count == indexes.Count)
                {
                    ArrayNode arr = (ArrayNode)this.value;
                    
                    return arr.GetValue(indexes.Count, 1, indexes, id);
                }
                else
                {
                    throw new CompiPascal.Utils.PascalError(this.line, this.column, "The amount of provided indexes doesn´t "
                            + "macth with the amount of dimentions of the array " + this.id + ", a value cannot be assigned.",
                            "Semantic");

                }
            //}else
            //{
            //    System.out.println("La variable " + id + " no es un arreglo, por lo "
            //            + "que no se puede accesar de esta manera.");
            //}
          
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

    }
}
