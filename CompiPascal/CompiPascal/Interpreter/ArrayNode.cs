using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompiPascal.Interpreter
{
    class ArrayNode
    {
       
        private LinkedList<ArrayNode> nextCells;

        private object value;
   
        public ArrayNode()
        {
            this.nextCells = new LinkedList<ArrayNode>();
            this.value = null;
        }

        public void initializeNode(int dimentionsAmount, int actualDimention, LinkedList<int> dimentionsSizes, Types type)
        {
            if (actualDimention > dimentionsAmount)
            {
                return;
            }
            for (int i = 0; i < dimentionsSizes.ElementAt(actualDimention - 1); i++)
            {
                ArrayNode arr = new ArrayNode();

                switch (type) 
                {
                    case Types.INTEGER:
                        arr.value = 0;
                        break;
                    case Types.STRING:
                        arr.value = "";
                        break;
                    case Types.REAL:
                        arr.value = 0.0;
                        break;
                    case Types.BOOLEAN:
                        arr.value = false;
                        break;
                }

                nextCells.AddLast(arr);
                arr.initializeNode(dimentionsAmount, actualDimention + 1, dimentionsSizes, type);
            }
        }

        public void SetValue(int indexesAmount, int actualIndex, LinkedList<int> indexes, Object val, String id)
        {
            int valIndiceActual = indexes.ElementAt(actualIndex - 1);
            if (valIndiceActual < nextCells.Count && valIndiceActual >= 0)
            {
                ArrayNode arr = nextCells.ElementAt(valIndiceActual);
                if (actualIndex == indexesAmount)
                {
                    arr.value = val;
                }
                else
                {
                    arr.SetValue(indexesAmount, actualIndex + 1, indexes, val, id);
                }
            }
            else
            {
                //System.err.println("La asignación al arreglo " + id + " no puede "
                //        + "realizarse porque uno o más de los indexes exceden "
                //        + "los límites del arreglo.");
            }
        }

        public object GetValue(int indexesAmount, int actualIndex, LinkedList<int> indexes, String id)
        {
            int valIndiceActual = indexes.ElementAt(actualIndex - 1);
            if (valIndiceActual < nextCells.Count && valIndiceActual >= 0)
            {
                ArrayNode arr = nextCells.ElementAt(valIndiceActual);
                if (actualIndex == indexesAmount)
                {
                    return arr.value;
                }
                else
                {
                    return arr.GetValue(indexesAmount, actualIndex + 1, indexes, id);
                }
            }
            else
            {
                //System.err.println("El acceso al arreglo " + id + " no puede "
                //        + "realizarse porque uno o más de los indexes exceden "
                //        + "los límites del arreglo.");
            }
            return null;
        }
    }
}
