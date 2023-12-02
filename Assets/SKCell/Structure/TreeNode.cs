using System.Linq;
using System.Collections.Generic;

namespace SKCell
{
    /// <summary>
    /// Tree node structure class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : ITreeNode<T>
    {
        private string name;
        private ITreeNode<T> parent;
        private List<ITreeNode<T>> children = new List<ITreeNode<T>>();

        private T value;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int ChildCount => children.Count;

        public ITreeNode<T> Parent
        {
            get
            {
                return parent;
            }
            set
            {
                this.parent = value;
            }
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        public TreeNode() { }

        public TreeNode(string name, T value)
        {
            this.name = name;
            this.value = value;
        }

        public TreeNode(string name, T value, ITreeNode<T> parent)
        {
            this.name = name;
            this.value = value;
            SetParent(parent);
        }

        /// <summary>
        /// Add child at the last index.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ITreeNode<T> child)
        {
            children.Add(child);
            child.Parent = this;
        }
        public void AddChild(int index, ITreeNode<T> child)
        {
            children.Insert(index, child);
            child.Parent = this;
        }

        public void ClearChild()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Parent = null;
            }
            children.Clear();
        }

        public ITreeNode<T>[] GetAllChild()
        {
            return children.ToArray();
        }

        public void GetAllChildNonAlloc(ref List<ITreeNode<T>> results)
        {
            results = children;
        }

        public ITreeNode<T>[] GetAllChildRecursive()
        {
            List<ITreeNode<T>> list = new List<ITreeNode<T>>();
            GetAllChildRecursiveHelper(list, this);
            return list.ToArray();
        }
        private void GetAllChildRecursiveHelper(List<ITreeNode<T>> list, ITreeNode<T> node)
        {
            list.Add(node);

            ITreeNode<T>[] childs = node.GetAllChild();
            for (int i = 0; i < childs.Length; i++)
            {
                GetAllChildRecursiveHelper(list, childs[i]);
            }
        }

        public ITreeNode<T> GetChild(int index)
        {
            if (index >= children.Count)
            {
                SKUtils.EditorLogError($"TreeNode.GetChild: index out of bounds!  index = {index}");
            }
            return children[index];
        }

        public int GetChildIndex(ITreeNode<T> child)
        {
            return children.IndexOf(child);
        }
        
        public T GetValue()
        {
            return value;
        }

        public ITreeNode<T> GetChild(string name)
        {
            return children.First(x => x.Name.Equals(name));
        }

        public bool HasChild(int index)
        {
            return children.Count > index;
        }

        public bool HasChild(string name)
        {
            return children.Any(x => x.Name.Equals(name));
        }

        public void RemoveChild(int index)
        {
            ITreeNode<T> node = children[index];
            node.Parent = null;
            children.Remove(node);
        }

        public void RemoveChild(string name)
        {
            ITreeNode<T> node = GetChild(name);
            node.Parent = null;
            children.Remove(node);
        }

        public void SetParent(ITreeNode<T> parent)
        {
            if(Parent != null)
                 Parent.RemoveChild(Parent.GetChildIndex(this));
            parent.AddChild(this);
        }

        public void DetachFromParent()
        {
            if (Parent != null)
                Parent.RemoveChild(Parent.GetChildIndex(this));
            Parent = null;
        }
    }
}
