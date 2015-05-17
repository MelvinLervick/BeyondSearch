using System.Collections.Generic;
using System.Linq;

namespace WordSearch.PatriciaTrie
{
    public class PatriciaSuffixTrie<TValue> : ITrie<TValue>
    {
        private readonly int minQueryLength;
        private readonly PatriciaTrie<TValue> innerTrie;

        public PatriciaSuffixTrie(int minQueryLength)
            : this(minQueryLength, new PatriciaTrie<TValue>())
        {
            
        }

        internal PatriciaSuffixTrie(int minQueryLength, PatriciaTrie<TValue> innerTrie)
        {
            this.minQueryLength = minQueryLength;
            this.innerTrie = innerTrie;
        }

        protected int MinQueryLength
        {
            get { return minQueryLength; }
        }

        public IEnumerable<TValue> Retrieve(string query)
        {
            return
                innerTrie
                    .Retrieve(query)
                    .Distinct();
        }

        public void Add(string key, TValue value)
        {
            var allSuffixes = GetAllSuffixes(MinQueryLength, key);
            foreach (var currentSuffix in allSuffixes)
            {
                innerTrie.Add(currentSuffix, value);
            }
        }

        private static IEnumerable<StringPartition> GetAllSuffixes(int minSuffixLength, string word)
        {
            for (var i = word.Length - minSuffixLength; i >= 0; i--)
            {
                yield return new StringPartition(word, i);
            }
        }
    }
}