using System;
using System.Collections.Generic;

namespace PredictiveText
{
    [Serializable]
    public abstract class Algorithm
    {
        public abstract string Name { get; }
        public abstract int MinSearchTextLength { get; }
        public abstract long SearchTrieWordCount { get; }
        public abstract IEnumerable<object> Retrieve(string searchTerm);
    }
}