using System.Collections.Generic;
using System;

public struct BinaryTree<TKey, TValue> where TKey : IComparable
{
    private struct TreeNode
    {
        public TKey key;
        public TValue value;

        public TreeNode(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    private List<TreeNode> tree;

    public int Count { get => tree.Count; }

    public void GetIndex(int index, out TKey key, out TValue value)
    {
        key = tree[index].key;
        value = tree[index].value;
    }

    public void Initialise()
    {
        tree = new List<TreeNode>();
    }

    public void Add(TKey key, TValue value)
    {
        tree.Add(new TreeNode());

        // propagate upwards
        int currentIndex = tree.Count - 1;
        
        while(0 < currentIndex)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (key.CompareTo(tree[parentIndex].key) < 0)
            {
                // current is smaller than parent
                tree[currentIndex] = tree[parentIndex];
                currentIndex = parentIndex;
            }
            else
                break;
        }

        tree[currentIndex] = new TreeNode(key, value);
    }

    public bool Pop(out TKey key, out TValue value)
    {
        if (tree.Count == 0)
        {
            key = default;
            value = default;
            return false;
        }
        else
        {
            key = tree[0].key;
            value = tree[0].value;
        }

        // remove last item
        TreeNode currentNode = tree[tree.Count - 1];
        tree.RemoveAt(tree.Count - 1);
        if (tree.Count == 0)
            return true;

        // propagate downwards
        int currentIndex = 0;
        while(true)
        {
            int leftChildIndex = 2 * currentIndex + 1;
            int rightChildIndex = 2 * currentIndex + 2;
            int smallestKeyIndex;
            if (rightChildIndex < tree.Count)
            {
                // find the smallest key among children                
                if (tree[leftChildIndex].key.CompareTo(tree[rightChildIndex].key) < 0)
                    smallestKeyIndex = leftChildIndex;
                else
                    smallestKeyIndex = rightChildIndex;
            }
            else if (leftChildIndex < tree.Count)
                smallestKeyIndex = leftChildIndex;
            else
                break;

            if (tree[smallestKeyIndex].key.CompareTo(currentNode.key) < 0)
            {
                // 1 child is smaller than current
                tree[currentIndex] = tree[smallestKeyIndex];
                currentIndex = smallestKeyIndex;
            }
            else
                break;
        }

        tree[currentIndex] = currentNode;
        return true;
    }
}
