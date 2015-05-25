using System;
using System.Collections.Generic;

namespace WordSearch.Trie
{
    [Serializable]
    public class TrieNode<TValue> : TrieNodeBase<TValue>
    {
        private readonly Dictionary<char, TrieNode<TValue>> children;
        private readonly Queue<TValue> values;

        protected TrieNode()
        {
            children = new Dictionary<char, TrieNode<TValue>>();
            values = new Queue<TValue>();
        }

        protected override int KeyLength
        {
            get { return 1; }
        }

        protected override IEnumerable<TrieNodeBase<TValue>> Children()
        {
            return children.Values;
        }

        protected override IEnumerable<TValue> Values()
        {
            return values;
        }

        protected override TrieNodeBase<TValue> GetOrCreateChild(char key)
        {
            TrieNode<TValue> result;

            if (!children.TryGetValue(key, out result))
            {
                result = new TrieNode<TValue>();
                children.Add(key, result);
            }
            return result;
        }

        protected override TrieNodeBase<TValue> GetChildOrNull(string query, int position)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            TrieNode<TValue> childNode;
            return
                children.TryGetValue(query[position], out childNode)
                    ? childNode
                    : null;
        }

        protected override void AddValue(TValue value)
        {
            values.Enqueue(value);
        }
    }
}