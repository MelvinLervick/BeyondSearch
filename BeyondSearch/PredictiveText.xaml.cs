using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PredictiveText;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for PredictiveText.xaml
    /// </summary>
    public partial class PredictiveText : Window
    {
        private const string Term = "Term";

        private SearchFactory searchTree;

        public PredictiveText()
        {
            InitializeComponent();
        }

        private void Menu_FileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_LoadWordsFromFileClick(object sender, RoutedEventArgs e)
        {
            bool? result;
            var dlg = OpenFileDialog(out result);

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxPTFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxPTFile.Text = dlg.SafeFileName;

                var sw = new Stopwatch();
                sw.Start();

                searchTree = new SearchFactory(SearchAlgorythms.FileTrie, TextBoxPTFolder.Text, TextBoxPTFile.Text);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private static OpenFileDialog OpenFileDialog(out bool? result)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Index documents All files (.txt)|*.txt",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            result = dlg.ShowDialog();
            return dlg;
        }

        private void TextBoxSearchFor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = TextBoxSearchFor.Text;

            if (string.IsNullOrEmpty(text) || text.Length < searchTree.SearchTree.MinSearchTextLength) return;

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

        private void Menu_LoadWordsFromFilesClick(object sender, RoutedEventArgs e)
        {
            bool? result;
            var dlg = OpenFileDialog(out result);

            if (result == true)
            {
                // Get the selected folder/file name and display in a TextBox 
                TextBoxPTFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxPTFile.Text = dlg.SafeFileName;

                var sw = new Stopwatch();
                sw.Start();

                searchTree = new SearchFactory(SearchAlgorythms.FileTrie, TextBoxPTFolder.Text);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }
    }
}
