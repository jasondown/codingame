// https://www.codingame.com/training/medium/telephone-numbers

using System;
using System.Collections.Generic;

namespace TelephoneNumbers_Csharp
{
    class Solution
    {
        static void Main()
        {
            var trie = new Trie();

            var totalNums = int.Parse(Console.ReadLine());
            for (var i = 0; i < totalNums; i++)
            {
                var telephone = Console.ReadLine();
                trie.Insert(telephone);
            }

            // The number of elements (referencing a number) stored in the structure.
            Console.WriteLine(trie.Count);
        }
    }

    class Node
    {
        public char Value { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }

        public Node(char value, int depth, Node parent)
        {
            Value = value;
            Children = new List<Node>();
            Depth = depth;
            Parent = parent;
        }

        public bool IsLeaf() => Children.Count == 0;

        public Node FindChildNode(char c)
        {
            foreach (var child in Children)
            {
                if (child.Value == c)
                {
                    return child;
                }
            }

            return null;
        }

        public void DeleteChildNode(char c)
        {
            for (var i = 0; i < Children.Count; i++)
                if (Children[i].Value == c)
                {
                    Children.RemoveAt(i);
                }
        }
    }

    class Trie
    {
        private readonly Node _root;
        public int Count { get; private set; }

        public Trie()
        {
            _root = new Node('^', 0, null);
        }

        public Node Prefix(string s)
        {
            var currentNode = _root;
            var result = currentNode;

            foreach (var c in s)
            {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null)
                {
                    break;
                }

                result = currentNode;
            }

            return result;
        }

        public bool Search(string s)
        {
            var prefix = Prefix(s);
            return prefix.Depth == s.Length && prefix.FindChildNode('$') != null;
        }

        public void InsertRange(List<string> items)
        {
            foreach (var t in items)
            {
                Insert(t);
            }
        }

        public void Insert(string s)
        {
            var commonPrefix = Prefix(s);
            var current = commonPrefix;

            for (var i = current.Depth; i < s.Length; i++)
            {
                var newNode = new Node(s[i], current.Depth + 1, current);
                current.Children.Add(newNode);
                current = newNode;
                Count++;
            }

            current.Children.Add(new Node('$', current.Depth + 1, current));
        }

        public void Delete(string s)
        {
            if (!Search(s)) return;

            var node = Prefix(s).FindChildNode('$');

            while (node.IsLeaf())
            {
                var parent = node.Parent;
                parent.DeleteChildNode(node.Value);
                node = parent;
                Count--;
            }
        }

    }
}