using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVL
{
    class Program
    {
        static void Main(string[] args)
        {
            AVL_Tree<int> tree = null;
            tree = new AVL_Tree<int>(Comparer<int>.Default);
           
            
            tree.Add(17);
            tree.Add(1);
            tree.Add(5);
            tree.Add(10);
            tree.Add(12);
            tree.Add(105);
            tree.Add(97);
            tree.Add(45);
            tree.Add(38);
            tree.Add(17);

            Console.WriteLine("Remove:");
            tree.Remove(97);

            tree.Remove(1);
            tree.Remove(5);
            tree.Remove(17);
            tree.Remove(10);
            tree.Remove(12);

            Console.ReadLine();
        }
    }
}
