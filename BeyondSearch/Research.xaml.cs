using System;
using System.Collections.Generic;
using System.Globalization;
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
    private bool ExactMatch = true;
    private KeywordFilter filter = new KeywordFilter();

    public Research()
    {
      InitializeComponent();
      InitializeKeywordList();
      InitializeFilterList();
    }

    private void InitializeKeywordList()
    {
      ListBoxKeywords.Items.Add( "hotels with pools" );
      ListBoxKeywords.Items.Add( "hotels without pools" );
      ListBoxKeywords.Items.Add( "hotels in south chicago" );
      ListBoxKeywords.Items.Add( "stores that sell adult toys" );
      ListBoxKeywords.Items.Add( "adult toys" );
      ListBoxKeywords.Items.Add( "restaurants with take out" );
      ListBoxKeywords.Items.Add( "adult only restaurants" );
    }

    private void InitializeFilterList()
    {
      ListBoxFilters.Items.Add( "adult toys" );
      ListBoxFilters.Items.Add( "adult" );
    }

    private void Menu_FileExitClick( object sender, RoutedEventArgs e )
    {
      this.Close();
    }

    private void AddKeyword_Click( object sender, RoutedEventArgs e )
    {
      if ( TextBoxStringToAdd.Text.Length > 0 )
      {
        ListBoxKeywords.Items.Add( TextBoxStringToAdd.Text );
      }
    }

    private void ClearKeyword_Click( object sender, RoutedEventArgs e )
    {
      ListBoxKeywords.Items.Clear();
    }

    private void AddFilter_Click( object sender, RoutedEventArgs e )
    {
      if (TextBoxStringToAdd.Text.Length > 0)
      {
        ListBoxFilters.Items.Add( TextBoxStringToAdd.Text );
      }
    }

    private void ClearFilter_Click( object sender, RoutedEventArgs e )
    {
      ListBoxFilters.Items.Clear();
    }

    private void FilterType_Checked( object sender, RoutedEventArgs e )
    {
      var button = sender as RadioButton;
      ExactMatch = false || button != null && button.Content.ToString() == "Exact";
    }

    private void Filter_Click( object sender, RoutedEventArgs e )
    {
      ListBoxFilteredKeywords.Items.Clear();

      if ( ExactMatch )
      {
        filter.FillExactFilterList( ListBoxKeywords.Items );
        if (ListBoxKeywords.Items.Count > 0)
        {
          var filteredItems = filter.Exact( ListBoxKeywords.Items );
          foreach (var filteredItem in filteredItems)
          {
            ListBoxFilteredKeywords.Items.Add( filteredItem );
          }
        }
      }
      else
      {
        filter.FillContainsFilterList( ListBoxFilters.Items );
        if (ListBoxKeywords.Items.Count > 0)
        {
          var filteredItems = filter.Contains( ListBoxKeywords.Items );
          foreach (var filteredItem in filteredItems)
          {
            ListBoxFilteredKeywords.Items.Add( filteredItem );
          }
        }
      }
    }
  }
}
