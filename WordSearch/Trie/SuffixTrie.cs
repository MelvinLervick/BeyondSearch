using System;
using System.Collections.Generic;
using System.Linq;
using WordSearch.PatriciaTrie;

namespace WordSearch.Trie
{
    [Serializable]
    public class SuffixTrie<T> : ITrie<T>
    {
        private readonly Trie<T> innerTrie;
        private readonly int minSuffixLength;

        public SuffixTrie(int minSuffixLength)
            : this(new Trie<T>(), minSuffixLength)
        {
        }

        private SuffixTrie(Trie<T> innerTrie, int minSuffixLength)
        {
            this.innerTrie = innerTrie;
            this.minSuffixLength = minSuffixLength;
        }

        public IEnumerable<T> Retrieve(string query)
        {
            return
                innerTrie
                    .Retrieve(query)
                    .Distinct();
        }

        public void Add(string key, T value)
        {
            foreach (var suffix in GetAllSuffixes(minSuffixLength, key))
            {
                innerTrie.Add(suffix, value);
            }
        }

        private static IEnumerable<string> GetAllSuffixes(int minSuffixLength, string word)
        {
            for (var i = word.Length - minSuffixLength; i >= 0; i--)
            {
                var partition = new StringPartition(word, i);
                yield return partition.ToString();
            }
        }
    }
}