using System.Collections.Generic;

namespace WordSearch
{
    /// <summary>
    /// Data Structure Interface
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ITrie<TValue>
    {
        IEnumerable<TValue> Retrieve(string query);
        void Add(string key, TValue value);
    }
}