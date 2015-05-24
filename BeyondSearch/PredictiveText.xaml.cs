using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeyondSearch.Common;
using BeyondSearch.Common.BeyondSearchFileReader;
using BeyondSearch.Filters;
using Microsoft.Win32;
using PredictiveText;
using WordSearch;
using WordSearch.Trie;
using Path = System.Windows.Shapes.Path;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for PredictiveText.xaml
    /// </summary>
    public partial class PredictiveText : Window
    {
        private const string Term = "Term";
        private const int MinSearchTextLength = 1;

        private readonly ITrie<WordPositionOLD> searchTrie;
        private long searchTrieWordCount;

        private SearchFactory searchTree;

        public PredictiveText()
        {
            InitializeComponent();
            searchTrie = new SuffixTrie<WordPositionOLD>(MinSearchTextLength);
        }

        private void Menu_FileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_LoadWordsClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Index documents All files (*.*)|*.*|(.txt)|*.txt",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxPTFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxPTFile.Text = dlg.SafeFileName;

                var sw = new Stopwatch();
                sw.Start();

                //LoadAllFiles();
                searchTree = new SearchFactory(SearchAlgorythms.FileTrie, TextBoxPTFolder.Text, TextBoxPTFile.Text);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void LoadAllFiles()
        {
            searchTrieWordCount = 0;
            var path = TextBoxPTFolder.Text;
            if (!Directory.Exists(path)) return;

            var file = System.IO.Path.Combine(TextBoxPTFolder.Text, TextBoxPTFile.Text);
            var fileInfo = new FileInfo(file);

            LoadFile(file);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            
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

        private IEnumerable<Tuple<WordPositionOLD, string>> GetWords(string file)
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
                                var wordPosition = new WordPositionOLD(position, file);
                                yield return new Tuple<WordPositionOLD, string>(wordPosition, word.ToString().ToLower());
                                word.Clear();
                                searchTrieWordCount++;
                            }
                        }
                    }
                }
            }
        }

        private void TextBoxSearchFor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = TextBoxSearchFor.Text;

            if (string.IsNullOrEmpty(text) || text.Length < MinSearchTextLength) return;

            var result = searchTree.SearchTree.Retrieve(text).ToArray();

            ListBoxWordsFound.Items.Clear();
            foreach (var wordPosition in result)
            {
                ListBoxWordsFound.Items.Add(wordPosition);
            }

        }

        private void ListBoxWordsFound_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ListBoxWordsFound.SelectedItem as WordPosition;
            if (item == null) return;

            using (var file = File.Open(item.FileName, FileMode.Open))
            {
                const int bufferSize = 200;
                var position = Math.Max(item.CharPosition - bufferSize / 2, 0);

                file.Seek(position, SeekOrigin.Begin);
                var buffer = new byte[bufferSize];
                file.Read(buffer, 0, bufferSize);

                var line = Encoding.ASCII.GetString(buffer);
                TextBlockSelected.Text = line;

                var searchText = TextBlockSelected.Text;
                var index = TextBlockSelected.Text.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase);
                if (index < 0) return;

                //TextBlockSelected.Text = String..Select(index, searchText.Length);
                //TextBlockSelected.SelectionBackColor = Color.Yellow;
                //TextBlockSelected.DeselectAll();
            }
        }
    }
}
