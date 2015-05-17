using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WordSearch.Trie;

namespace WordSearch.PatriciaTrie
{
    [DebuggerDisplay("'{key}'")]
    public class PatriciaTrieNode<TValue> : TrieNodeBase<TValue>
    {
        private Dictionary<char, PatriciaTrieNode<TValue>> children;
        private StringPartition localKey;
        private Queue<TValue> qValues;

        protected PatriciaTrieNode(StringPartition key, TValue value)
            : this(key, new Queue<TValue>(new[] {value}), new Dictionary<char, PatriciaTrieNode<TValue>>())
        {
        }

        protected PatriciaTrieNode(StringPartition key, Queue<TValue> values,
            Dictionary<char, PatriciaTrieNode<TValue>> children)
        {
            this.qValues = values;
            this.localKey = key;
            this.children = children;
        }

        protected override int KeyLength
        {
            get { return localKey.Length; }
        }

        protected override IEnumerable<TValue> Values()
        {
            return qValues;
        }

        protected override IEnumerable<TrieNodeBase<TValue>> Children()
        {
            return children.Values;
        }


        protected override void AddValue(TValue value)
        {
            qValues.Enqueue(value);
        }

        internal virtual void Add(StringPartition keyRest, TValue value)
        {
            var zipResult = localKey.ZipWith(keyRest);

            switch (zipResult.MatchKind)
            {
                case MatchKind.ExactMatch:
                    AddValue(value);
                    break;

                case MatchKind.IsContained:
                    GetOrCreateChild(zipResult.OtherRest, value);
                    break;

                case MatchKind.Contains:
                    SplitOne(zipResult, value);
                    break;

                case MatchKind.Partial:
                    SplitTwo(zipResult, value);
                    break;
            }
        }


        private void SplitOne(ZipResult zipResult, TValue value)
        {
            var leftChild = new PatriciaTrieNode<TValue>(zipResult.ThisRest, qValues, children);

            children = new Dictionary<char, PatriciaTrieNode<TValue>>();
            qValues = new Queue<TValue>();
            AddValue(value);
            localKey = zipResult.CommonHead;

            children.Add(zipResult.ThisRest[0], leftChild);
        }

        private void SplitTwo(ZipResult zipResult, TValue value)
        {
            var leftChild = new PatriciaTrieNode<TValue>(zipResult.ThisRest, qValues, children);
            var rightChild = new PatriciaTrieNode<TValue>(zipResult.OtherRest, value);

            children = new Dictionary<char, PatriciaTrieNode<TValue>>();
            qValues = new Queue<TValue>();
            localKey = zipResult.CommonHead;

            var leftKey = zipResult.ThisRest[0];
            children.Add(leftKey, leftChild);
            var rightKey = zipResult.OtherRest[0];
            children.Add(rightKey, rightChild);
        }

        protected void GetOrCreateChild(StringPartition key, TValue value)
        {
            PatriciaTrieNode<TValue> child;
            if (!children.TryGetValue(key[0], out child))
            {
                child = new PatriciaTrieNode<TValue>(key, value);
                children.Add(key[0], child);
            }
            else
            {
                child.Add(key, value);
            }
        }

        protected override TrieNodeBase<TValue> GetOrCreateChild(char key)
        {
            throw new NotSupportedException("Use alternative signature instead.");
        }

        protected override TrieNodeBase<TValue> GetChildOrNull(string query, int position)
        {
            if (query == null) throw new ArgumentNullException("query");
            PatriciaTrieNode<TValue> child;
            if (children.TryGetValue(query[position], out child))
            {
                var queryPartition = new StringPartition(query, position, child.localKey.Length);
                if (child.localKey.StartsWith(queryPartition))
                {
                    return child;
                }
            }
            return null;
        }

        public string Traversal()
        {
            var result = new StringBuilder();
            result.Append(localKey);

            string subtreeResult = string.Join(" ; ", children.Values.Select(node => node.Traversal()).ToArray());
            if (subtreeResult.Length != 0)
            {
                result.Append("[");
                result.Append(subtreeResult);
                result.Append("]");
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return 
                string.Format(
                    "Key: {0}, Values: {1} Children:{2}, ", 
                    localKey, 
                    Values().Count(),
                    String.Join(";", children.Keys));
        }
    }
}