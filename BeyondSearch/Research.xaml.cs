using System;
using System.Collections.Generic;
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
    public Research()
    {
      InitializeComponent();
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

    private void AddFilter_Click( object sender, RoutedEventArgs e )
    {
      if (TextBoxStringToAdd.Text.Length > 0)
      {
        ListBoxFilters.Items.Add( TextBoxStringToAdd.Text );
      }
    }

    private void FilterType_Checked( object sender, RoutedEventArgs e )
    {
      var button = sender as RadioButton;

      //if ( button != null && button.Content.ToString() == "Exact" )
    }

    private void Filter_Click( object sender, RoutedEventArgs e )
    {
      if ( ListBoxKeywords.Items.Count > 0 )
      {
        foreach ( var item in ListBoxKeywords.Items )
        {
          ListBoxFilteredKeywords.Items.Add( item );
        }
      }
    }
  }
}
