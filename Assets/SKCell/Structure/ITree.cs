
namespace SKCell
{
    public interface ITree<T>
    {
        string Name
        {
            get; set;
        }

        ITreeNode<T> HeadNode
        {
            get;
        }

        /// <summary>
        /// Construct a tree given a head node. Call this after every change in nodes to keep the tree up to date.
        /// </summary>
        void Construct();

        void SetHeadNode(ITreeNode<T> node);

        /// <summary>
        /// Get a node in the tree. Call Construct() before calling this method.
        /// </summary>
        /// <returns></returns>
        ITreeNode<T> GetNode(string name);

        /// <summary>
        /// Delete a node in the tree. Call Construct() before calling this method.
        /// </summary>
        /// <returns></returns>
        ITreeNode<T> DeleteNode(string name);
    }
}