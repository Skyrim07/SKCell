using System.Collections.Generic;

namespace SKCell
{
    /// <summary>
    /// Interface for a tree node.
    /// </summary>
    public interface ITreeNode<T>
    {
        string Name
        {
            get; set;
        }
        int ChildCount
        {
            get;
        }

        ITreeNode<T> Parent
        {
            get; set;
        }

        T GetValue();

        void SetParent(ITreeNode<T> parent);

        void DetachFromParent();

        bool HasChild(int index);

        bool HasChild(string name);

        ITreeNode<T> GetChild(int index);

        ITreeNode<T> GetChild(string name);

        ITreeNode<T>[] GetAllChildRecursive();

        int GetChildIndex(ITreeNode<T> child);

        ITreeNode<T>[] GetAllChild();

        void GetAllChildNonAlloc(ref List<ITreeNode<T>> results);

        void AddChild(ITreeNode<T> child);

        void AddChild(int index, ITreeNode<T> child);

        void RemoveChild(int index);

        void RemoveChild(string name);

        void ClearChild();

        string ToString();
    }
}
