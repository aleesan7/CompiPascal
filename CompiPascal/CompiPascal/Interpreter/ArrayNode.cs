using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CompiPascal.Interpreter
{
    class ArrayNode
    {
        /**
     * Lista de celdas vecinas en el arreglo.
     */
        private LinkedList<ArrayNode> nextCells;
        /**
         * Valor que alberga esta celda del arreglo.
         */
        private object value;
        /**
         * Constructor para crear una celda de arreglo vacía
         */
        public ArrayNode()
        {
            this.nextCells = new LinkedList<ArrayNode>();
            this.value = null;
        }
        /**
         * Método que inicializa todas las celdas de un arreglo, esta inicialización
         * se propaga a lo largo de todas las celdas del arreglo según lo que se le 
         * indica en sus parámetros.
         * @param dimentionsAmount Cantidad de dimensiones del arreglo
         * @param actualDimention Dimensión que se está analizando en la propagación actual
         * @param dimentionsSizes Lista que contienen los tamaños de todas las dimensiones del arreglo
         */
        public void initializeNode(int dimentionsAmount, int actualDimention, LinkedList<int> dimentionsSizes)
        {
            if (actualDimention > dimentionsAmount)
            {
                return;
            }
            for (int i = 0; i < dimentionsSizes.ElementAt(actualDimention - 1); i++)
            {
                ArrayNode arr = new ArrayNode();
                nextCells.AddLast(arr);
                arr.initializeNode(dimentionsAmount, actualDimention + 1, dimentionsSizes);
            }
        }
        /**
         * Método que configura cierto value en una celda específica del arreglo
         * @param indexesAmount Cantidad de indexes que se reciben para el acceso al arreglo
         * @param actualIndex Indice que se está analizando en la propagación actual
         * @param indexes Lista de los indexes con los que se accederá al arreglo para asignar el value
         * @param val Valor que se le quiere asignar a cierta celda del arreglo
         * @param id Identificador del arreglo
         */
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
        /**
         * Método que recoge cierto value en una celda específica del arreglo y 
         * devuelve nulo cuando no lo encuentra
         * @param indexesAmount Cantidad de indexes que se reciben para el acceso al arreglo
         * @param actualIndex Indice que se está analizando en la propagación actual
         * @param indexes Lista de los indexes con los que se accederá al arreglo para asignar el value
         * @param id Identificador del arreglo
         * @return El value almacenado por la celda específica o null en caso no lo encuentre
         */
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
