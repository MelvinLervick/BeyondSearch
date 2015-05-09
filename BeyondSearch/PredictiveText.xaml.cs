using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using BeyondSearch.Common;
using BeyondSearch.Common.BeyondSearchFileReader;
using BeyondSearch.Filters;
using Microsoft.Win32;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for PredictiveText.xaml
    /// </summary>
    public partial class PredictiveText : Window
    {
        private const string Term = "Term";

        public ObservableCollection<FilteredKeyword> Keywords;

        public PredictiveText()
        {
            InitializeComponent();
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
                DefaultExt = ".tsv",
                Filter = "Index documents All files (*.*)|*.*|(.tsv)|*.tsv",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxKeywordFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxKeywordFile.Text = dlg.SafeFileName;

                var sw = new Stopwatch();
                sw.Start();

                if (TextBoxKeywordFile.Text.Contains(".tsv"))
                {
                    var reader = new BeyondSearchFileReader();
                    var terms =
                        reader.ReadTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text, TextBoxKeywordFile.Text), RecordFormat.Tsv)
                            .ToList();

                    StoreTermsReadToObservableCollection(Keywords, terms);
                }
                else
                {
                    var reader = new BeyondSearchFileReader();
                    var terms =
                        reader.ReadTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text,
                            TextBoxKeywordFile.Text), RecordFormat.TermOnly).ToList();

                    StoreTermsReadToObservableCollection(Keywords, terms);
                }

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void StoreTermsReadToObservableCollection(
            ObservableCollection<FilteredKeyword> collection,
            IEnumerable<FilteredKeyword> terms)
        {
            collection.Clear();
            foreach (var filteredKeyword in terms)
            {
                collection.AddFilteredKeywordListItem(false, filteredKeyword.Keyword, category: filteredKeyword.Category,
                    bit: filteredKeyword.CategoryBit);
            }
        }

        private void Menu_SaveWordsClick(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                DefaultExt = "txt",
                CheckPathExists = true,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFile.ShowDialog() ?? false)
            {
                // If the file name is not an empty string open it for saving.
                if (saveFile.FileName != "")
                {
                    using (var tw = new System.IO.StreamWriter(saveFile.FileName))
                    {
                        tw.WriteLine(Term);
                        foreach (var keyword in Keywords)
                        {
                            if (!string.IsNullOrWhiteSpace(keyword.Keyword))
                            {
                                tw.WriteLine(keyword.Keyword);
                            }
                        }

                        tw.Close();
                    }
                }
                else
                {
                    TextBoxFilterFile.Text = "Invalid filename";
                }
            }
        }
    }
}
