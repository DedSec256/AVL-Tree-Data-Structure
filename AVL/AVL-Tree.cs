using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVL
{
    class AVL_Tree<T>
    {
        Node root;
        Comparer<T> comparerT;

        class Node
        {
            public T    key;
            public Node Left;
            public Node Rigth;
            public Node Last;
            public int count;

            public Node(T value, Node Last = null)
            {
                this.key    = value;
                this.Left   = null;
                this.Rigth  = null;
                this.Last   = Last;
                this.count  = 1;
            }
            
        }

        public AVL_Tree(Comparer<T> comp)
        {
            this.comparerT = comp;
        }

        public void Add(T element)
        {
            if(this.root == null)
            {
                this.root = new Node(element);
                return;
            }

            Node currentNode = this.root;
            Node lastNode    = null;
            bool rotateLeft  = false;    /* По какую сторону от родительской
                                          * вершины находится добавляемая вершина
                                          */

            while(currentNode!=null)
            {
                lastNode = currentNode;

                if (comparerT.Compare(element, currentNode.key) < 0)
                {
                    currentNode = currentNode.Left;
                    rotateLeft = true;
                }
                else if (comparerT.Compare(element, currentNode.key) > 0)
                {
                    currentNode = currentNode.Rigth;
                    rotateLeft = false;
                }

                else
                {
                    currentNode.count += 1;
                    return;
                }

            }

            currentNode      = new Node(element);
            currentNode.Last = lastNode;

            switch(rotateLeft) /*
                                *  Необходимо, чтобы правильно назначить эту вершину
                                *  родителю
                                */
            {
                case true:
                    lastNode.Left = currentNode;
                    break;

                case false:
                    lastNode.Rigth = currentNode;
                    break;
            }
            FixHeight(currentNode); // Определяем, нет ли разбалансировки по высоте

            #region Рисуем дерево в консоли
            Console.WriteLine("------------------------------------------------------------------------------------");
            Print(this.root, 10);
            Console.WriteLine("------------------------------------------------------------------------------------");
            #endregion
        }

        private void FixHeight(Node added, bool add = true)
        {
            Node currentNode = added;

            int height = Convert.ToInt32(add);

            while (currentNode.Last != null)
            {
                /*
                 * Сравниваем левую ветвь с правой 
                 * за счёт рекурсивной функции FindHeight
                 */
                int tempHeight = 0;

                Node leftOrRight;

                if (comparerT.Compare(currentNode.key, currentNode.Last.key) < 0)
                {
                    tempHeight = this.FindHeight(currentNode.Last.Rigth);
                    leftOrRight = currentNode.Last.Rigth;
                }

                else
                {
                    tempHeight = this.FindHeight(currentNode.Last.Left);
                    leftOrRight = currentNode.Last.Left; 
                }

                if (Math.Abs(height - tempHeight) >=2)
                {
                    if(add == true)Balance(currentNode.Last, added);
                    else
                    {
                        Balance(currentNode.Last, leftOrRight.Left == null ? 
                                leftOrRight.Rigth : leftOrRight.Left);
                    }
                    break;
                }

                height = 1 + Math.Max(height, tempHeight);
                currentNode = currentNode.Last;
            }
        }

        private int FindHeight(Node node)
        {
            if (node == null) return 0;

            int sum = 1 + Math.Max(FindHeight(node.Left),FindHeight(node.Rigth));
            return sum;
        }

        private void RotateLeft(Node a, Node b)
        {
            if (a.Last == null) // Если меняемая вершина - корень
            {
                this.root = b;
                b.Last = null;
            }
            else
            {
                bool leftA = comparerT.Compare(a.key, a.Last.key) < 0 ? true : false;
                switch (leftA) //Если меняемая вершина слева/справа
                {
                    case true:
                        a.Last.Left = b;
                        b.Last = a.Last;
                        break;

                    case false:
                        a.Last.Rigth = b;
                        b.Last = a.Last;
                        break;
                }
            }

            a.Left = b.Rigth;
            if (a.Left != null) a.Left.Last = a;
            b.Rigth = a;
            a.Last = b;
        }

        private void RotateRight(Node a, Node b)
        {
            if (a.Last == null)
            {
                this.root = b;
                b.Last = null;
            }
            else
            {
                bool leftA = comparerT.Compare(a.key, a.Last.key) < 0 ? true : false;
                switch (leftA)
                {
                    case true:
                        a.Last.Left = b;
                        b.Last = a.Last;
                        break;

                    case false:
                        a.Last.Rigth = b;
                        b.Last = a.Last;
                        break;
                }
            }

            a.Rigth = b.Left;
            if(a.Rigth != null) a.Rigth.Last = a;
            b.Left = a;
            a.Last = b;
        }

        private void Balance(Node a, Node added)
        {
            /*
             * Определяем, с какой стороны от родителя 
             * находится добавляемая вершина, чтобы сделать поворот в нужную сторону
             * если left = true - поворачиваем влево, и наоборот
             */
            bool left = comparerT.Compare(added.key, a.key) < 0 ? true : false;
            /*
             * Определяем, какой тип поворота - составной (уголком), или нет
             */
            bool line = CheckType(left, a,added);
            /*
             * Если добавляемая вершина - слева, а поворот НЕ составной
             */
            if ( line == true && left == true)
            {
                RotateLeft(a, a.Left);
            }
            /*
             * Если добавляемая вершина - справа, а поворот НЕ составной
             */
            else if (line == true && left == false)
            {
                RotateRight(a, a.Rigth);
            }
            /*
             * Если добавляемая вершина - слева, а поворот составной
             */
            else if (line == false && left == true)
            {
                RotateRight(a.Left, a.Left.Rigth);
                RotateLeft(a, a.Left);
            }
            /*
             * Если добавляемая вершина - справа, а поворот составной
             */
            else if (line == false && left == false)
            {
                RotateLeft(a.Rigth, a.Rigth.Left);
                RotateRight(a, a.Rigth);
            }

        }

        private bool CheckType(bool left, Node a, Node added)
        {
            switch(left)
            {
                case true:
                    if (comparerT.Compare(added.key,a.key) < 0 &&
                        comparerT.Compare(added.key,a.Left.key) < 0)
                        return true;
                    return false;

                case false:
                    if (comparerT.Compare(added.key, a.key) > 0 &&
                        comparerT.Compare(added.key, a.Rigth.key) > 0)
                        return true;
                    return false;

            }

            return false;
        }

        private void Print(Node root, uint k)
        {
            if (root != null)
            {
                Print(root.Rigth, k + 3);
               
                for (uint i = 0; i < k; i++)
                {
                    Console.Write("  ");
                }
                Console.WriteLine(root.key + "\n");
                Print(root.Left, k + 3);
            }

        }

        private Node FindElement(T element)
        {
            Node currentNode = this.root;

            while (currentNode != null)
            {
                if (comparerT.Compare(element, currentNode.key) < 0)
                {
                    currentNode = currentNode.Left;
                }
                else if (comparerT.Compare(element, currentNode.key) > 0)
                {
                    currentNode = currentNode.Rigth;
                }

                else
                {
                    return currentNode;
                }

            }

            throw new ArgumentException("Элемент не найден!");

        }

       private void NodeToZero(Node deleteNode)
       {
            FixHeight(deleteNode, false);
            if (comparerT.Compare(deleteNode.key, deleteNode.Last.key) <= 0)
            {
                deleteNode.Last.Left = null;
            }
            else
            {
                deleteNode.Last.Rigth = null;
            }
            deleteNode = null;
       }

       public void Remove(T element)
        {
            Node deleteNode = FindElement(element);
            /*
             * Если таких элементов несколько
             */
            if (deleteNode.count > 1)
            {
                deleteNode.count--;
                return;
            }
            /*
             * Если остался лишь корень
             */
            if (comparerT.Compare(this.root.key, element) == 0 
                && this.root.Left == null && this.root.Left == null)
            {
                this.root = null;
                return;
            }
            /*
             * Если удалямая вершина - лист
             */
            if (deleteNode.Left == null && deleteNode.Rigth == null)
            {
                NodeToZero(deleteNode);
            }
            /*
             * Если удаляемая вершина в середине
             */
            else
            {
                Node temp = null;
                if (deleteNode.Left != null)
                {
                    temp = deleteNode.Left;
                    while (temp.Rigth != null)
                    {
                        temp = temp.Rigth;
                    }
                }
                else
                {
                    temp = deleteNode.Rigth;
                    while (temp.Left != null)
                    {
                        temp = temp.Left;
                    }
                }

                    deleteNode.key = temp.key;
                    NodeToZero(temp);
            }

            #region Рисуем дерево в консоли
            Console.WriteLine("------------------------------------------------------------------------------------");
            Print(this.root, 10);
            Console.WriteLine("------------------------------------------------------------------------------------");
            #endregion
        }
    }
}