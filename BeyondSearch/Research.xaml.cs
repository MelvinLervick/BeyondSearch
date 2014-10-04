using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BeyondSearch.Common;
using BeyondSearch.Common.BeyondSearchFileReader;
using BeyondSearch.Common.CategorizedFilterReader;
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
        private const string Dictionary = "Dictionary";
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
        public ObservableCollection<FilteredKeyword> FilteredKeywords;
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
            FilteredKeywords = new ObservableCollection<FilteredKeyword>();

            InitializeKeywordList();
            InitializeFilterList();
            DisplaySelectedFilter();

            ListBoxKeywords.ItemsSource = Keywords;
            ListBoxKeywords.Items.SortDescriptions.Add(new SortDescription("Keyword", ListSortDirection.Ascending));
            ListBoxKeywords.Items.IsLiveSorting = true;
            ListBoxFilters.ItemsSource = Filters;
            ListBoxFilters.Items.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
            ListBoxFilters.Items.SortDescriptions.Add(new SortDescription("Keyword", ListSortDirection.Ascending));
            ListBoxFilters.Items.IsLiveSorting = true;
            ListBoxUnFilteredKeywords.ItemsSource = UnFilteredKeywords;
            ListBoxFilteredKeywords.ItemsSource = FilteredKeywords;
        }

        private void InitializeKeywordList()
        {
            Keywords.AddFilteredKeywordListItem(false, "hotels with pools");
            Keywords.AddFilteredKeywordListItem(false, "hotels in south chicago red light");
            Keywords.AddFilteredKeywordListItem(false, "stores that sell adult toys");
            Keywords.AddFilteredKeywordListItem(false, "adult toys");
            Keywords.AddFilteredKeywordListItem(false, "adult only restaurants");

            Keywords.AddFilteredKeywordListItem(false, "animal shelter dog");
            Keywords.AddFilteredKeywordListItem(false, "animal shelter dogs");
            Keywords.AddFilteredKeywordListItem(false, "animal shelter cat");
            Keywords.AddFilteredKeywordListItem(false, "animal shelter cats");
            Keywords.AddFilteredKeywordListItem(false, "park zebra");

            Keywords.AddFilteredKeywordListItem(false, "park zebras");
            Keywords.AddFilteredKeywordListItem(false, "zoo animal zebra");
            Keywords.AddFilteredKeywordListItem(false, "zoo animal zebras");
            Keywords.AddFilteredKeywordListItem(false, "clothes young girls");
            Keywords.AddFilteredKeywordListItem(false, "young girls");

            Keywords.AddFilteredKeywordListItem(false, "zebra");
            Keywords.AddFilteredKeywordListItem(false, "cat");
            Keywords.AddFilteredKeywordListItem(false, "dog");
            Keywords.AddFilteredKeywordListItem(false, "red light");
            Keywords.AddFilteredKeywordListItem(false, "red lights");
        }

        private void InitializeFilterList()
        {
            Filters.AddFilteredKeywordListItem(false,  "adult toys" );
            Filters.AddFilteredKeywordListItem(false,  "zebra" );
            Filters.AddFilteredKeywordListItem(false,  "young girls" );
            Filters.AddFilteredKeywordListItem(false,  "red light" );
            Filters.AddFilteredKeywordListItem(false,  "cat" );

            Filters.AddFilteredKeywordListItem(false,  "dog" );
        }

        private void DisplaySelectedFilter()
        {
            if (MenuItemDictionary.IsChecked)
            {
                LabelSelectedFilter.Content = Dictionary;
                return;
            }
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

            FilteredKeywords.Clear();
            UnFilteredKeywords.Clear();
            UpdateFilterCategoryBits();
            SetSelectedFilters(sw);
        }

        private void SetSelectedFilters(Stopwatch sw)
        {
            if (MenuItemExact.IsChecked) ExactMatchFilter(sw);
            if (MenuItemFuzzy.IsChecked) FuzzyContainsMatchFilter(sw);
            if (MenuItemSansSpaceOrNumber.IsChecked) ContainsSansSpaceAndNumberMatchFilter(sw);
            if (MenuItemStrictContains.IsChecked) StrictContainsMatchFilter(sw);
            if (MenuItemContains.IsChecked) ContainsMatchFilter(sw);
            if (MenuItemDictionary.IsChecked) DictionaryMatchFilter(sw);
            if (MenuItemLucenePorterStem.IsChecked) LucenePorterStemFilter(sw);
        }

        private void MoveUnFilteredKeywords_Click(object sender, RoutedEventArgs e)
        {
            if (Keywords != null && Keywords.Count > 0) Keywords.Clear();

            foreach (var keyword in UnFilteredKeywords)
            {
                Keywords.AddFilteredKeywordListItem(false,  keyword.Keyword, category: keyword.Category, bit: keyword.CategoryBit );
            }

            UnFilteredKeywords.Clear();
        }

        private void MarkSelectedFilter_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as MenuItem;
            item.IsChecked = true;

            if (item.Name == "MenuItemDictionary")
            {
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemContains")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemStrictContains")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemSansSpaceOrNumber")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemExact")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemFuzzy")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemLucenePorterStem.IsChecked = false;
            }
            if (item.Name == "MenuItemLucenePorterStem")
            {
                MenuItemDictionary.IsChecked = false;
                MenuItemContains.IsChecked = false;
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
            }

            DisplaySelectedFilter();
        }

        private void ComboBoxCategory_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = Categories;
            comboBox.DisplayMemberPath = Category;
            comboBox.SelectedValuePath = Category;
        }

        #region Filters

        private void DictionaryMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists(filter.Dictionary(keywords));
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ContainsMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.Contains( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void StrictContainsMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.StrictContains( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ContainsSansSpaceAndNumberMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.ContainsSansSpaceAndNumber( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ExactMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.Exact( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void FuzzyContainsMatchFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.FuzzyContains( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void LucenePorterStemFilter(Stopwatch sw)
        {
            filter.FillFilterList(Filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                var keywords = Keywords.Select(x => x.Keyword).ToList();

                sw.Start();
                SetGoodAndBadDisplayLists( filter.LucenePorterStemContains( keywords ) );
                sw.Stop();

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        #endregion

        #region ToolTips

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

        private void ListBoxFilteredKeywords_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxFilteredKeywords.ToolTip = FilteredKeywords.Count.ToString();
        }

        #endregion

        #region File Read/Write

        private void Menu_FilesKeywordsClick(object sender, RoutedEventArgs e)
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
                        UpdateFilterCategoryBits();
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

        private void UpdateFilterCategoryBits()
        {
            foreach ( var keyword in Filters )
            {
                keyword.CategoryBit =
                    ( Categories.Where( cat => cat.Category == keyword.Category ).Select( cat => cat.CategoryBit ) ).First();
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

                var sw = new Stopwatch();
                sw.Start();

                var reader = new BeyondSearchFileReader();
                var terms =
                    reader.ReadTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text), RecordFormat.TermOnly)
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
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

                var sw = new Stopwatch();
                sw.Start();

                var reader = new BeyondSearchFileReader();
                var terms =
                    reader.ReadTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text), RecordFormat.CategorizedTerm)
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
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

                var sw = new Stopwatch();
                sw.Start();

                var reader = new BeyondSearchFileReader();
                var terms =
                    reader.ReadTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text), RecordFormat.Tsv)
                        .ToList();

                StoreTermsReadToObservableCollection(Filters, terms);

                sw.Stop();
                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        #endregion

        #region private methods not referenced directly by XAML UI

        private void AddWordToCollection(ObservableCollection<FilteredKeyword> collection, string word)
        {
            if (collection == null) collection = new ObservableCollection<FilteredKeyword>();
            collection.AddFilteredKeywordListItem(true, word);
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
                collection.AddFilteredKeywordListItem(false, filteredKeyword.Keyword, category: filteredKeyword.Category,
                    bit: filteredKeyword.CategoryBit);
            }
        }

        private void SetGoodAndBadDisplayLists(GoodOrBadKeywords filteredItems)
        {
            foreach (var filteredItem in filteredItems.GoodKeywords)
            {
                UnFilteredKeywords.AddFilteredKeywordListItem(false, filteredItem);
            }
            foreach (var filteredItem in filteredItems.BadKeywords)
            {
                FilteredKeywords.AddFilteredKeywordListItem(false, filteredItem);
            }
        }

        #endregion

        private FilteredKeyword filteredKeyword;
        private void KeywordsListContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ContextMenu) sender;
            var item = contextMenu.PlacementTarget as ListBox;
            filteredKeyword = item.SelectedItem as FilteredKeyword;
        }

        private void KeywordsListMenuItem_OnClick( object sender, RoutedEventArgs e )
        {
            if(filteredKeyword != null) Keywords.Remove( filteredKeyword );
        }

        private string filterText;
        private void FiltersListContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ContextMenu)sender;
            var panel = contextMenu.PlacementTarget as StackPanel;
            var dock = panel.Children[0] as DockPanel;
            var val = dock.Children[0] as TextBox;
            filterText = val.Text;
        }

        private void FiltersListMenuItem_OnClicksListMenuItem_OnClick( object sender, RoutedEventArgs e )
        {
            if ( !string.IsNullOrWhiteSpace( filterText ) )
            {
                var myFilteredKeyword = Filters.First( f => f.Keyword == filterText );
                Filters.Remove(myFilteredKeyword);
            }
        }
    }
}
