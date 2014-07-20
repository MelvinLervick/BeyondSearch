using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BeyondSearch.Common;
using BeyondSearch.Common.CategorizedFilterReader;
using BeyondSearch.Common.FilterFileReader;
using BeyondSearch.Common.TsvFileReader;
using BeyondSearch.Filters;
using Microsoft.Win32;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for Research.xaml
    /// </summary>
    public partial class Research : Window
    {
        private const string ContainsMatch = "Contains Match";
        private const string StrictContainsMatch = "Strict Contains Match";
        private const string ContainsSansSpaceAndNumberMatch = "Sans Space & Number Match";
        private const string ExactMatch = "Exact Match";
        private const string FuzzyContainsMatch = "Fuzzy Match";
        private const string LucenePorterStemMatch = "Lucene Porter Stem";
        private const string Term = "Term";
        private const string CategoryBit = "CategoryBit";
        private const string Category = "Category";
        private const string SaveFileFilter = "{0}\t{1}\t{2}";
        private readonly KeywordFilter filter = new KeywordFilter();

        public ObservableCollection<FilteredKeyword> Keywords;
        public ObservableCollection<FilteredKeyword> Filters;
        public ObservableCollection<FilteredKeyword> UnFilteredKeywords;
        public List<KeywordCategory> Categories = new List<KeywordCategory>
        {
            new KeywordCategory {Category = "0", CategoryBit = 0},
            new KeywordCategory {Category = "1", CategoryBit = 1},
            new KeywordCategory {Category = "2", CategoryBit = 2},
            new KeywordCategory {Category = "3", CategoryBit = 4},
            new KeywordCategory {Category = "4", CategoryBit = 8},
            new KeywordCategory {Category = "5", CategoryBit = 16},
            new KeywordCategory {Category = "6", CategoryBit = 32},
            new KeywordCategory {Category = "7", CategoryBit = 64}
        }; 

        public Research()
        {
            InitializeComponent();
            Keywords = new ObservableCollection<FilteredKeyword>();
            Filters = new ObservableCollection<FilteredKeyword>();
            UnFilteredKeywords = new ObservableCollection<FilteredKeyword>();

            InitializeKeywordList();
            InitializeFilterList();
            DisplaySelectedFilter();

            ListBoxKeywords.ItemsSource = Keywords;
            ListBoxFilters.ItemsSource = Filters;
            ListBoxUnFilteredKeywords.ItemsSource = UnFilteredKeywords;
        }

        private void InitializeKeywordList()
        {
            Keywords.AddFilteredKeywordListItem("hotels with pools");
            Keywords.AddFilteredKeywordListItem("hotels in south chicago red light");
            Keywords.AddFilteredKeywordListItem("stores that sell adult toys");
            Keywords.AddFilteredKeywordListItem("adult toys");
            Keywords.AddFilteredKeywordListItem("adult only restaurants");

            Keywords.AddFilteredKeywordListItem("animal shelter dog");
            Keywords.AddFilteredKeywordListItem("animal shelter dogs");
            Keywords.AddFilteredKeywordListItem("animal shelter cat");
            Keywords.AddFilteredKeywordListItem("animal shelter cats");
            Keywords.AddFilteredKeywordListItem("park zebra");

            Keywords.AddFilteredKeywordListItem("park zebras");
            Keywords.AddFilteredKeywordListItem("zoo animal zebra");
            Keywords.AddFilteredKeywordListItem("zoo animal zebras");
            Keywords.AddFilteredKeywordListItem("clothes young girls");
            Keywords.AddFilteredKeywordListItem("young girls");

            Keywords.AddFilteredKeywordListItem("zebra");
            Keywords.AddFilteredKeywordListItem("cat");
            Keywords.AddFilteredKeywordListItem("dog");
            Keywords.AddFilteredKeywordListItem("red light");
            Keywords.AddFilteredKeywordListItem("red lights");
        }

        private void InitializeFilterList()
        {
            Filters.AddFilteredKeywordListItem( "adult toys" );
            Filters.AddFilteredKeywordListItem( "zebra" );
            Filters.AddFilteredKeywordListItem( "young girls" );
            Filters.AddFilteredKeywordListItem( "red light" );
            Filters.AddFilteredKeywordListItem( "cat" );

            Filters.AddFilteredKeywordListItem( "dog" );
        }

        private void DisplaySelectedFilter()
        {
            if (MenuItemExact.IsChecked)
            {
                LabelSelectedFilter.Content = ExactMatch;
                return;
            }
            if (MenuItemFuzzy.IsChecked)
            {
                LabelSelectedFilter.Content = FuzzyContainsMatch;
                return;
            }
            if (MenuItemSansSpaceOrNumber.IsChecked)
            {
                LabelSelectedFilter.Content = ContainsSansSpaceAndNumberMatch;
                return;
            }
            if (MenuItemStrictContains.IsChecked)
            {
                LabelSelectedFilter.Content = StrictContainsMatch;
                return;
            }
            if (MenuItemLucenePorterStem.IsChecked)
            {
                LabelSelectedFilter.Content = LucenePorterStemMatch;
                return;
            }
            if (MenuItemContains.IsChecked)
            {
                LabelSelectedFilter.Content = ContainsMatch;
                return;
            }
        }

        private void Menu_FileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddKeyword_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxStringToAdd.Text.Length > 0)
            {
                AddWordToCollection(Keywords, TextBoxStringToAdd.Text);
                TextBoxStringToAdd.Text = String.Empty;
            }
        }

        private void ClearKeyword_Click(object sender, RoutedEventArgs e)
        {
            Keywords.Clear();

            TextBoxKeywordFolder.Text = string.Empty;
            TextBoxKeywordFile.Text = string.Empty;
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxStringToAdd.Text.Length > 0)
            {
                AddWordToCollection(Filters, TextBoxStringToAdd.Text);
                TextBoxStringToAdd.Text = String.Empty;
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            Filters.Clear();

            TextBoxFilterFolder.Text = string.Empty;
            TextBoxFilterFile.Text = string.Empty;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();

            UnFilteredKeywords.Clear();
            SetSelectedFilters(sw);
        }

        private void SetSelectedFilters(Stopwatch sw)
        {
            if (MenuItemList.IsChecked)
            {
                if (MenuItemExact.IsChecked) ExactMatchFilter(sw, 0);
                if (MenuItemFuzzy.IsChecked) FuzzyContainsMatchFilter(sw, 0);
                if (MenuItemSansSpaceOrNumber.IsChecked) ContainsSansSpaceAndNumberMatchFilter(sw, 0);
                if (MenuItemStrictContains.IsChecked) StrictContainsMatchFilter(sw, 0);
                if (MenuItemContains.IsChecked) ContainsMatchFilter(sw, 0);
                if (MenuItemLucenePorterStem.IsChecked) LucenePorterStemFilter(sw, 0);
            }
            else
            {
                if (MenuItemExact.IsChecked) ExactMatchFilter(sw, 1);
                if (MenuItemFuzzy.IsChecked) FuzzyContainsMatchFilter(sw, 1);
                if (MenuItemSansSpaceOrNumber.IsChecked) ContainsSansSpaceAndNumberMatchFilter(sw, 1);
                if (MenuItemStrictContains.IsChecked) StrictContainsMatchFilter(sw, 1);
                if (MenuItemContains.IsChecked) ContainsMatchFilter(sw, 1);
                if (MenuItemLucenePorterStem.IsChecked) LucenePorterStemFilter(sw, 1);
            }
        }

        #region Filters

        private void ContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Contains(keywords) : filter.Contains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void StrictContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.StrictContains(keywords) : filter.StrictContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ContainsSansSpaceAndNumberMatchFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.ContainsSansSpaceAndNumber(keywords)
                    : filter.ContainsSansSpaceAndNumber1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ExactMatchFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Exact(keywords) : filter.Exact1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void FuzzyContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.FuzzyContains(keywords)
                    : filter.FuzzyContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void LucenePorterStemFilter(Stopwatch sw, int oneOrMany)
        {
            var filters = Filters.Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.LucenePorterStemContains(keywords)
                    : filter.LucenePorterStemContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    UnFilteredKeywords.AddFilteredKeywordListItem(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        #endregion

        private void MoveFilteredKeywords_Click(object sender, RoutedEventArgs e)
        {
            if (Keywords != null && Keywords.Count > 0) Keywords.Clear();

            foreach (var keyword in UnFilteredKeywords)
            {
                Keywords.AddFilteredKeywordListItem( keyword.Keyword, keyword.Category, keyword.CategoryBit );
            }

            UnFilteredKeywords.Clear();
        }

        private void MarkSelectedFilter_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as MenuItem;
            item.IsChecked = true;

            if (item.Name == "MenuItemContains")
            {
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemStrictContains")
            {
                MenuItemContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemSansSpaceOrNumber")
            {
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemExact")
            {
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemFuzzy")
            {
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemLucenePorterStem")
            {
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
            }

            DisplaySelectedFilter();
        }

        private void ListBoxKeywords_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxKeywords.ToolTip = Keywords.Count.ToString();
        }

        private void ListBoxFilters_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxFilters.ToolTip = Filters.Count.ToString();
        }

        private void ListBoxUnFilteredKeywords_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxUnFilteredKeywords.ToolTip = UnFilteredKeywords.Count.ToString();
        }

        #region File Read/Write

        private void Menu_FilesKeywordsClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".tsv",
                Filter = "Index documents (.tsv)|*.tsv|All files (*.*)|*.*",
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

                if (TextBoxKeywordFile.Text.Contains(".tsv"))
                {
                    var reader = new TsvProhibitedKeywordFileReader();
                    var terms =
                        reader.ReadKeywords(System.IO.Path.Combine(TextBoxKeywordFolder.Text, TextBoxKeywordFile.Text))
                            .ToList();

                    StoreTermsReadToObservableCollection(Keywords, terms);
                }
                else
                {
                    var reader = new FilterTermFileReader();
                    var terms =
                        reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text,
                            TextBoxKeywordFile.Text)).ToList();

                    StoreTermsReadToObservableCollection(Keywords, terms);
                }
            }
        }

        private void Menu_SaveFiltersClick(object sender, RoutedEventArgs e)
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
                        tw.WriteLine(SaveFileFilter, Term, CategoryBit, Category);
                        foreach (var keyword in Filters)
                        {
                            if (!string.IsNullOrWhiteSpace(keyword.Keyword))
                            {
                                tw.WriteLine(SaveFileFilter, keyword.Keyword, keyword.CategoryBit, keyword.Category);
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

        private void Menu_SaveKeywordsClick(object sender, RoutedEventArgs e)
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

        private void Menu_FilesFilterTermClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Index documents (.txt)|*.txt|All files (*.*)|*.*",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;

                var reader = new FilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);
            }
        }

        private void Menu_FilesFilterCategoryClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Index documents (.txt)|*.txt|All files (*.*)|*.*",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;

                var reader = new CategorizedFilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);
            }
        }

        private void Menu_FilesFilterTsvClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".tsv",
                Filter = "Index documents (.tsv)|*.tsv|All files (*.*)|*.*",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;

                var reader = new TsvProhibitedKeywordFileReader();
                var terms =
                    reader.ReadKeywords(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);
            }
        }

        #endregion

        #region private methods not referenced directly by XAML UI

        private void AddWordToCollection(ObservableCollection<FilteredKeyword> collection, string word)
        {
            if (collection == null) collection = new ObservableCollection<FilteredKeyword>();
            collection.AddFilteredKeywordListItem(word);
        }

        private IEnumerable<string> DuplicateList(List<string> list, int noTimesToDuplicate)
        {
            var listToReturn = new List<string>();

            for (int i = 0; i < noTimesToDuplicate; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (i > 0)
                    {
                        listToReturn.Add(list[j] + i.ToString() + " " + j.ToString() + " ");
                    }
                    else
                    {
                        listToReturn.Add(list[j]);
                    }
                }
            }

            return listToReturn;
        }

        private void StoreTermsReadToObservableCollection(
            ObservableCollection<FilteredKeyword> collection,
            IEnumerable<FilteredKeyword> terms)
        {
            collection.Clear();
            foreach (var filteredKeyword in terms)
            {
                collection.AddFilteredKeywordListItem(filteredKeyword.Keyword, filteredKeyword.Category,
                    filteredKeyword.CategoryBit);
            }
        }

        #endregion

        private void FilterCategoryTextBox_OnTextChanged( object sender, TextChangedEventArgs e )
        {
            var  box = sender as TextBox;
            var keyword = box.DataContext as FilteredKeyword;

            switch (box.Text)
            {
                case "1":
                    keyword.Category = "1";
                    keyword.CategoryBit = 1;
                    break;
                case "2":
                    keyword.Category = "2";
                    keyword.CategoryBit = 2;
                    break;
                case "3":
                    keyword.Category = "3";
                    keyword.CategoryBit = 4;
                    break;
                case "4":
                    keyword.Category = "4";
                    keyword.CategoryBit = 8;
                    break;
                case "5":
                    keyword.Category = "5";
                    keyword.CategoryBit = 16;
                    break;
                case "6":
                    keyword.Category = "6";
                    keyword.CategoryBit = 32;
                    break;
                case "7":
                    keyword.Category = "7";
                    keyword.CategoryBit = 64;
                    break;
                default:
                    keyword.Category = "0";
                    keyword.CategoryBit = 0;
                    break;
            }
            keyword.Category = box.Text;
        }

        private void ComboBoxCategory_OnLoaded( object sender, RoutedEventArgs e )
        {
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = Categories;
            comboBox.DisplayMemberPath = Category;
            comboBox.SelectedValuePath = Category;
        }
    }
}
