using System.Linq;
using System.Collections.Generic;

namespace SKCell
{
    public class TreeStructure<T> : ITree<T>
    {
        private string name;
        private ITreeNode<T> headNode;
        private ITreeNode<T>[] nodeList;

        public string Name { get => name; set => name = value; }

        public ITreeNode<T> HeadNode => headNode;

        public TreeStructure() { }
        public TreeStructure(string name)
        {
            this.name = name;
        }

        public TreeStructure(string name, ITreeNode<T> headNode)
        {
            this.name = name;
            this.headNode = headNode;
        }

        public void SetHeadNode(ITreeNode<T> node)
        {
            if (node.Parent != null)
            {
                SKUtils.EditorLogError("Tree.SetHeadNode: node with parent cannot be set head node.");
                return;
            }

            headNode = node;
        }

        public void Construct()
        {
            if (headNode == null)
                return;

            nodeList = headNode.GetAllChildRecursive();
        }

        /// <summary>
        /// Get a node in the tree by its name. Call Construct() before calling this method.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITreeNode<T> GetNode(string name)
        {
            return nodeList.First(x => x.Name.Equals(name));
        }

        /// <summary>
        /// Delete a node in the tree by its name. Call Construct() before calling this method.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITreeNode<T> DeleteNode(string name)
        {
            ITreeNode<T> node = GetNode(name);
            node.DetachFromParent();

            return node;
        }
    }
}
