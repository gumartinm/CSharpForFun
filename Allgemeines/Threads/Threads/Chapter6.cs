using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Threads
{
    public class Chapter6
    {
        /// <summary>
        /// 
        /// Chapter 6.
        /// Dynamic Task Parallelism
        /// 
        /// Taken from http://msdn.microsoft.com/en-us/library/ff963551.aspx
        /// 
        /// </summary>

        public void FillTree<T>(int level, Tree<T> node, Func<T> function)
        {
            node.Data = function();
            node.Level = level;

            level = level - 1;
            if (level == 0)
            {
                return;
            }

            node.Left = new Tree<T>();
            FillTree(level, node.Left, function);
            node.Right = new Tree<T>();
            FillTree(level, node.Right, function);
        }

        public void SequentialWalk<T>(Tree<T> tree, Action<T, int> action)
        {
            if (tree == null)
            {
                return;
            }

            action(tree.Data, tree.Level);
            this.SequentialWalk(tree.Left, action);
            this.SequentialWalk(tree.Right, action);
        }

        public void ParallelWalk<T>(Tree<T> tree, Action<T, int> action)
        {
            if (tree == null)
            {
                return;
            }

            var t1 = Task.Factory.StartNew(() => action(tree.Data, tree.Level));
            var t2 = Task.Factory.StartNew(() => ParallelWalk(tree.Left, action));
            var t3 = Task.Factory.StartNew(() => ParallelWalk(tree.Right, action)); 

            Task.WaitAll(t1, t2, t3);
        }

        public class Tree<T>
        {
            public T       Data  { get; set; }
            public int     Level { get; set; }
            public Tree<T> Left  { get; set; }
            public Tree<T> Right { get; set; }
        }
    }
}

