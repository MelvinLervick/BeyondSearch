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
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class Widget : Window
    {
        public Widget()
        {
            InitializeComponent();
        }

        private void Menu_FileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_FileNewOnClick(object sender, RoutedEventArgs e)
        {
            TextBoxWidgetName.IsReadOnly = false;
            TextBoxWidgetName.Focus();
        }

        private void Menu_FileOpenOnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Menu_FileSaveOnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Menu_FileCloseOnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            ButtonCreate.IsEnabled = false;
        }

        private void TextBoxWidgetName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Check to see if widget with specified name already exists
            // If not
            ButtonCreate.IsEnabled = true;
            // Else Display message "Widget with the specified name already exists."
        }
    }
}
