using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PredictiveText
{
    [Serializable]
    public class SearchFactory
    {
        public Algorithm SearchTree;

        public SearchFactory(SearchAlgorythms id, string folder, string file = "*.*")
        {
            switch (id)
            {
                case SearchAlgorythms.FileTrie:
                    SearchTree = new FileTrie(folder, file, 2);
                    break;
                case SearchAlgorythms.MemoryTrie:
                    SearchTree = new MemoryTrie(folder, file);
                    break;
                case SearchAlgorythms.Trie:
                    SearchTree = new FileTrie(folder, file, 3);
                    break;
            }
        }
    }
}
