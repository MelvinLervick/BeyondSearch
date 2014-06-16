﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Shapes;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for Research.xaml
    /// </summary>
    public partial class Research : Window
    {
        private bool exactMatch = true;
        private KeywordFilter filter = new KeywordFilter();

        public Research()
        {
            InitializeComponent();
            InitializeKeywordList();
            InitializeFilterList();
        }

        private void InitializeKeywordList()
        {
            ListBoxKeywords.Items.Add(" hotels with pools ");
            ListBoxKeywords.Items.Add(" hotels in south chicago red light ");
            ListBoxKeywords.Items.Add(" stores that sell adult toys ");
            ListBoxKeywords.Items.Add(" adult toys ");
            ListBoxKeywords.Items.Add(" adult only restaurants ");

            ListBoxKeywords.Items.Add(" animal shelter dog ");
            ListBoxKeywords.Items.Add(" animal shelter dogs ");
            ListBoxKeywords.Items.Add(" animal shelter cat ");
            ListBoxKeywords.Items.Add(" animal shelter cats ");
            ListBoxKeywords.Items.Add(" park zebra ");

            ListBoxKeywords.Items.Add(" park zebras ");
            ListBoxKeywords.Items.Add(" zoo animal zebra ");
            ListBoxKeywords.Items.Add(" zoo animal zebras ");
            ListBoxKeywords.Items.Add(" clothes young girls ");
            ListBoxKeywords.Items.Add(" young girls ");

            ListBoxKeywords.Items.Add(" zebra ");
            ListBoxKeywords.Items.Add(" cat ");
            ListBoxKeywords.Items.Add(" dog ");
            ListBoxKeywords.Items.Add(" red light ");
            ListBoxKeywords.Items.Add(" red lights ");
        }

        private void InitializeFilterList()
        {
            ListBoxFilters.Items.Add("adult toys");
            ListBoxFilters.Items.Add("zebra");
            ListBoxFilters.Items.Add("young girls");
            ListBoxFilters.Items.Add("red light");
            ListBoxFilters.Items.Add("cat");

            ListBoxFilters.Items.Add("dog");
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
            }
        }

        private void ClearKeyword_Click(object sender, RoutedEventArgs e)
        {
            ListBoxKeywords.Items.Clear();
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxStringToAdd.Text.Length > 0)
            {
                ListBoxFilters.Items.Add(TextBoxStringToAdd.Text);
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ListBoxFilters.Items.Clear();
        }

        private void FilterType_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            exactMatch = false || button != null && button.Content.ToString() == "Exact";
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            ListBoxFilteredKeywords.Items.Clear();
            var keywords = new List<string>();
            var filters = new List<string>();

            if (exactMatch)
            {
                filters = ListBoxFilters.Items.Cast<string>().ToList();
                filter.FillExactFilterList(DuplicateList(filters, 100));
                if (ListBoxKeywords.Items.Count > 0)
                {
                    keywords = ListBoxKeywords.Items.Cast<string>().ToList();
                    sw.Start();
                    var filteredItems = filter.Exact(DuplicateList(keywords, 500));
                    sw.Stop();
                    foreach (var filteredItem in filteredItems)
                    {
                        ListBoxFilteredKeywords.Items.Add(filteredItem);
                    }
                    TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
                }
            }
            else
            {
                filters = ListBoxFilters.Items.Cast<string>().ToList();
                filter.FillContainsFilterList(DuplicateList(filters, 100));
                if (ListBoxKeywords.Items.Count > 0)
                {
                    keywords = ListBoxKeywords.Items.Cast<string>().ToList();
                    sw.Start();
                    var filteredItems = filter.Contains(DuplicateList(keywords, 500));
                    sw.Stop();
                    foreach (var filteredItem in filteredItems)
                    {
                        ListBoxFilteredKeywords.Items.Add(filteredItem);
                    }
                    TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
                }
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

        private void ButtonKeywordsFile_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".txt", Filter = "Text documents (.txt)|*.txt"};

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxKeywordFile.Text = dlg.FileName;
            }
        }

        private void ButtonFilterFile_Click( object sender, RoutedEventArgs e )
        {
        }
    }
}