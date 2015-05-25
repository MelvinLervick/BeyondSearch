using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WordSearch;
using WordSearch.Trie;

namespace PredictiveText
{
    [Serializable]
    public class MemoryTrie : Algorithm
    {
        private readonly ITrie<MemoryPosition> searchTrie;

        private readonly int minSearchTextLength;
        private long searchTrieWordCount;

        public override string Name
        {
            get { return "MemoryTrie"; }
        }

        public override int MinSearchTextLength
        {
            get { return minSearchTextLength; }
        }

        public override long SearchTrieWordCount
        {
            get { return searchTrieWordCount; }
        }

        public MemoryTrie(string folder, string file, int textLength = 1)
        {
            minSearchTextLength = 1;

            searchTrie = new SuffixTrie<MemoryPosition>(minSearchTextLength);
            
            LoadFiles(folder, file);
        }

        public override IEnumerable<object> Retrieve(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < MinSearchTextLength) return new List<string>();

            return searchTrie.Retrieve(searchTerm).ToArray();
        }

        private void LoadFiles(string searchFolder, string searchFile)
        {
            searchTrieWordCount = 0;
            
            if (!Directory.Exists(searchFolder)) return;

            if (searchFile == "*.*")
            {
                var files = Directory.GetFiles(searchFolder, "*.txt", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    LoadFile(file);
                }
            }
            else
            {
                var file = System.IO.Path.Combine(searchFolder, searchFile);

                LoadFile(file);
            }
        }

        private void LoadFile(string fileName)
        {
            var words = GetWords(fileName).ToArray();
            foreach (var word in words)
            {
                var text = word.Item2;
                var memoryPosition = word.Item1;
                searchTrie.Add(text, memoryPosition);
            }
        }

        private IEnumerable<Tuple<MemoryPosition, string>> GetWords(string file)
        {
            using (var stream = new StreamReader(file))
            {
                long lineNumber = 0;
                while (true)
                {
                    if (stream.EndOfStream) break;

                    lineNumber++;
                    var line = stream.ReadLine();
                    var lineFields = line.Split('\t');

                    var word = lineFields[0];
                    if (word.Length != 0)
                    {
                        var memoryPosition = new MemoryPosition(lineNumber, line);
                        yield return new Tuple<MemoryPosition, string>(memoryPosition, word.ToLower());
                        searchTrieWordCount++;
                    }
                }
            }
        }
    }
}