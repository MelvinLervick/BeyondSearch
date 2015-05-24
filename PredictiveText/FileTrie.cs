using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WordSearch;
using WordSearch.Trie;

namespace PredictiveText
{
    public class FileTrie : Algorithm
    {
        private readonly ITrie<WordPosition> searchTrie;

        private readonly int minSearchTextLength;
        private long searchTrieWordCount;

        public override string Name
        {
            get { return "FileTrie"; }
        }

        public override int MinSearchTextLength
        {
            get { return minSearchTextLength; }
        }

        public override long SearchTrieWordCount
        {
            get { return searchTrieWordCount; }
        }

        public FileTrie(string folder, string file, int textLength = 1)
        {
            minSearchTextLength = 1;

            searchTrie = new SuffixTrie<WordPosition>(minSearchTextLength);
            
            LoadFiles(folder, file);
        }

        public override IEnumerable<object> Retrieve(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < MinSearchTextLength) return new List<string>();

            return searchTrie.Retrieve(searchTerm).ToArray();

            //return result.Select(wordPosition => wordPosition).ToList<object>();
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
                var wordPosition = word.Item1;
                searchTrie.Add(text, wordPosition);
            }
        }

        private IEnumerable<Tuple<WordPosition, string>> GetWords(string file)
        {
            using (Stream stream = File.Open(file, FileMode.Open))
            {
                var word = new StringBuilder();
                while (true)
                {
                    var position = stream.Position;
                    int data = (char)stream.ReadByte();
                    {
                        if (data > byte.MaxValue) break;
                        var ch = (Char)data;
                        if (char.IsLetter(ch))
                        {
                            word.Append(ch);
                        }
                        else
                        {
                            if (word.Length != 0)
                            {
                                var wordPosition = new WordPosition(position, file);
                                yield return new Tuple<WordPosition, string>(wordPosition, word.ToString().ToLower());
                                word.Clear();
                                searchTrieWordCount++;
                            }
                        }
                    }
                }
            }
        }
    }
}