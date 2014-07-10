using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private readonly KeywordFilter filter = new KeywordFilter();

        public List<FilteredKeyword> Keywords;
        public List<FilteredKeyword> Filters;

        public Research()
        {
            InitializeComponent();
            InitializeKeywordList();
            InitializeFilterList();
            DisplaySelectedFilter();
            ListBoxKeywords.ToolTip = ListBoxKeywords.Items.Count.ToString();
            ListBoxFilters.ToolTip = ListBoxFilters.Items.Count.ToString();
        }

        private void InitializeKeywordList()
        {
            Keywords = new List<FilteredKeyword>
            {
                new FilteredKeyword {Keyword = "hotels with pools"},
                new FilteredKeyword {Keyword = "hotels in south chicago red light"},
                new FilteredKeyword {Keyword = "stores that sell adult toys"},
                new FilteredKeyword {Keyword = "adult toys"},
                new FilteredKeyword {Keyword = "adult only restaurants"},

                new FilteredKeyword {Keyword = "animal shelter dog"},
                new FilteredKeyword {Keyword = "animal shelter dogs"},
                new FilteredKeyword {Keyword = "animal shelter cat"},
                new FilteredKeyword {Keyword = "animal shelter cats"},
                new FilteredKeyword {Keyword = "park zebra"},

                new FilteredKeyword {Keyword = "park zebras"},
                new FilteredKeyword {Keyword = "zoo animal zebra"},
                new FilteredKeyword {Keyword = "zoo animal zebras"},
                new FilteredKeyword {Keyword = "clothes young girls"},
                new FilteredKeyword {Keyword = "young girls"},

                new FilteredKeyword {Keyword = "zebra"},
                new FilteredKeyword {Keyword = "cat"},
                new FilteredKeyword {Keyword = "dog"},
                new FilteredKeyword {Keyword = "red light"},
                new FilteredKeyword {Keyword = "red lights"}
            };

            ListBoxKeywords.ItemsSource = Keywords;

            //ListBoxKeywords.Items.Add("hotels with pools");
            //ListBoxKeywords.Items.Add("hotels in south chicago red light");
            //ListBoxKeywords.Items.Add("stores that sell adult toys");
            //ListBoxKeywords.Items.Add("adult toys");
            //ListBoxKeywords.Items.Add("adult only restaurants");

            //ListBoxKeywords.Items.Add("animal shelter dog");
            //ListBoxKeywords.Items.Add("animal shelter dogs");
            //ListBoxKeywords.Items.Add("animal shelter cat");
            //ListBoxKeywords.Items.Add("animal shelter cats");
            //ListBoxKeywords.Items.Add("park zebra");

            //ListBoxKeywords.Items.Add("park zebras");
            //ListBoxKeywords.Items.Add("zoo animal zebra");
            //ListBoxKeywords.Items.Add("zoo animal zebras");
            //ListBoxKeywords.Items.Add("clothes young girls");
            //ListBoxKeywords.Items.Add("young girls");

            //ListBoxKeywords.Items.Add("zebra");
            //ListBoxKeywords.Items.Add("cat");
            //ListBoxKeywords.Items.Add("dog");
            //ListBoxKeywords.Items.Add("red light");
            //ListBoxKeywords.Items.Add("red lights");
        }

        private void InitializeFilterList()
        {
            Filters = new List<FilteredKeyword>
            {
                new FilteredKeyword {Keyword = "adult toys"},
                new FilteredKeyword {Keyword = "zebra"},
                new FilteredKeyword {Keyword = "young girls"},
                new FilteredKeyword {Keyword = "red light"},
                new FilteredKeyword {Keyword = "cat"},

                new FilteredKeyword {Keyword = "dog"}
            };

            ListBoxFilters.ItemsSource = Filters;

            //ListBoxFilters.Items.Add("adult toys");
            //ListBoxFilters.Items.Add("zebra");
            //ListBoxFilters.Items.Add("young girls");
            //ListBoxFilters.Items.Add("red light");
            //ListBoxFilters.Items.Add("cat");

            //ListBoxFilters.Items.Add("dog");
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
                ListBoxKeywords.Items.Add(" " + TextBoxStringToAdd.Text + " ");
                ListBoxKeywords.ToolTip = ListBoxKeywords.Items.Count.ToString();
            }
        }

        private void ClearKeyword_Click(object sender, RoutedEventArgs e)
        {
            ListBoxKeywords.Items.Clear();
            ListBoxKeywords.ToolTip = string.Empty;

            TextBoxKeywordFolder.Text = string.Empty;
            TextBoxKeywordFile.Text = string.Empty;
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxStringToAdd.Text.Length > 0)
            {
                ListBoxFilters.Items.Add(TextBoxStringToAdd.Text);
                ListBoxFilters.ToolTip = ListBoxFilters.Items.Count.ToString();
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ListBoxFilters.Items.Clear();
            ListBoxFilters.ToolTip = string.Empty;

            TextBoxFilterFolder.Text = string.Empty;
            TextBoxFilterFile.Text = string.Empty;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            ListBoxFilteredKeywords.Items.Clear();

            SetSelectedFilters(sw);
            ListBoxFilteredKeywords.ToolTip = ListBoxFilteredKeywords.Items.Count.ToString();
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

        private void ContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Contains(keywords) : filter.Contains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void StrictContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.StrictContains(keywords) : filter.StrictContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ContainsSansSpaceAndNumberMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.ContainsSansSpaceAndNumber(keywords)
                    : filter.ContainsSansSpaceAndNumber1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ExactMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Exact(keywords) : filter.Exact1(keywords);
                sw.Stop();

                ListBoxFilteredKeywords.Items.Clear();
                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void FuzzyContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.FuzzyContains(keywords)
                    : filter.FuzzyContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void LucenePorterStemFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<FilteredKeyword>().Select(x => x.Keyword).ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.LucenePorterStemContains(keywords)
                    : filter.LucenePorterStemContains1(keywords);
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
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

        private void Menu_FilesKeywordsClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|*.cfs|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxKeywordFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxKeywordFile.Text = dlg.SafeFileName;
            }

            if (TextBoxKeywordFile.Text.Length > 0)
            {
                if (TextBoxKeywordFile.Text.Contains(".tsv"))
                {
                    var reader = new TsvProhibitedKeywordFileReader();
                    var terms =
                        reader.ReadKeywords(System.IO.Path.Combine(TextBoxKeywordFolder.Text, TextBoxKeywordFile.Text))
                            .ToList();
                    ListBoxKeywords.Items.Clear();
                    foreach (var term in terms)
                    {
                        ListBoxKeywords.Items.Add(term);
                    }
                }
                else
                {
                    var reader = new FilterTermFileReader();
                    var terms =
                        reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text,
                            TextBoxKeywordFile.Text)).ToList();
                    ListBoxKeywords.Items.Clear();
                    foreach (var term in terms)
                    {
                        ListBoxKeywords.Items.Add(term);
                    }
                }
                ListBoxKeywords.ToolTip = ListBoxKeywords.Items.Count.ToString();
            }
        }

        private void MoveFilteredKeywords_Click(object sender, RoutedEventArgs e)
        {
            ListBoxKeywords.Items.Clear();
            foreach (var item in ListBoxFilteredKeywords.Items)
            {
                ListBoxKeywords.Items.Add(item);
            }
            ListBoxFilteredKeywords.Items.Clear();
            ListBoxKeywords.ToolTip = ListBoxKeywords.Items.Count.ToString();
            ListBoxFilteredKeywords.ToolTip = ListBoxFilteredKeywords.Items.Count.ToString();
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
                        tw.WriteLine(Term);
                        foreach (var item in ListBoxFilters.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ToString()))
                            {
                                tw.WriteLine(item.ToString());
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
                        foreach (var item in ListBoxKeywords.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ToString()))
                            {
                                tw.WriteLine(item.ToString());
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
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|All files (*.*)|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;
            }

            if (TextBoxFilterFile.Text.Length > 0)
            {
                var reader = new FilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();
                ListBoxFilters.Items.Clear();
                foreach (var term in terms)
                {
                    ListBoxFilters.Items.Add(term);
                }

                ListBoxFilters.ToolTip = ListBoxFilters.Items.Count.ToString();
            }
        }

        private void Menu_FilesFilterCategoryClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|All files (*.*)|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;
            }

            if (TextBoxFilterFile.Text.Length > 0)
            {
                var reader = new CategorizedFilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();
                Filters =
                    terms.Select(
                        x => new FilteredKeyword {Category = x.Category, CategoryBit = x.CategoryBit, Keyword = x.Term}).ToList();
                
                ListBoxFilters.ItemsSource = Filters;
                ListBoxFilters.ToolTip = Filters.Count.ToString();
            }
        }

        private void Menu_FilesFilterTsvClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".tsv", Filter = "Index documents (.tsv)|*.tsv|All files (*.*)|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;
            }

            if (TextBoxFilterFile.Text.Length > 0)
            {
                var reader = new TsvProhibitedKeywordFileReader();
                var terms =
                    reader.ReadKeywords(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();
                ListBoxFilters.Items.Clear();
                foreach (var term in terms)
                {
                    ListBoxFilters.Items.Add(term);
                }

                ListBoxFilters.ToolTip = ListBoxFilters.Items.Count.ToString();
            }
        }
    }
}
