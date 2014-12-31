using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using WebPageWidget;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class Widget : Window
    {
        private WebWidget workWidget;
        public Widget()
        {
            workWidget = new WebWidget();
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
            TextBoxWidgetFolder.Text = string.Empty;
            TextBoxWidgetFile.Text = string.Empty;
        }

        private void Menu_FileOpenOnClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "Widget Files (.json)|*.json|All files (*.*)|*.*",
                CheckPathExists = true,
                CheckFileExists = true
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxWidgetFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxWidgetFile.Text = dlg.SafeFileName;

                // Read Selected File
                // http://james.newtonking.com/json/help/index.html
                //
                if ( !string.IsNullOrWhiteSpace( TextBoxWidgetFolder.Text ) ||
                     !string.IsNullOrWhiteSpace( TextBoxWidgetFile.Text ) )
                {
                    if ( workWidget.ReadWidgetFile( System.IO.Path.Combine( TextBoxWidgetFolder.Text,
                            TextBoxWidgetFile.Text ) ) )
                    {
                        TextBoxWidgetName.Text = workWidget.Parameters.Name;
                        TextBoxParameterName.Text = workWidget.Parameters.Name;
                        TextBoxParameterAuthor.Text = workWidget.Parameters.Author;
                        TextBoxParameterVersion.Text = workWidget.Parameters.Version;
                        TextBoxParameterType.Text = workWidget.Parameters.Type.ToString();
                        CheckBoxParameterLocked.IsChecked = workWidget.Parameters.Locked;
                        CheckBoxParameterEncrypted.IsChecked = workWidget.Parameters.Encrypted;
                        TextBoxParameterKey.Text = workWidget.Parameters.Key;

                        TextBoxCodeSnippet.Text = workWidget.HtmlContent;
                        TextBoxStyling.Text = workWidget.StyleContent;
                    }
                    else
                    {
                        TextBoxErrorMessage.Text = workWidget.ErrorMessage;
                    }
                }
            }
        }

        private void Menu_FileSaveOnClick(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog
            {
                InitialDirectory = TextBoxWidgetFolder.Text,
                FileName = TextBoxWidgetFile.Text,
                DefaultExt = "json",
                CheckPathExists = true,
                Filter = "Widget Files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (saveFile.ShowDialog() ?? false)
            {
                // If the file name is not an empty string open it for saving.
                if (saveFile.FileName != "")
                {
                    using (var tw = new System.IO.StreamWriter(saveFile.FileName))
                    {
                        // http://james.newtonking.com/json/help/index.html

                        //List<data> _data = new List<data>();
                        //_data.Add(new data()
                        //{
                        //    Id = 1,
                        //    SSN = 2,
                        //    Message = "A Message"
                        //});
                        //string json = JsonConvert.SerializeObject(_data.ToArray());

                        ////write string to file
                        //System.IO.File.WriteAllText(@"D:\path.txt", json);
                        
                        
                        //tw.WriteLine(Term);
                        //foreach (var keyword in Keywords)
                        //{
                        //    if (!string.IsNullOrWhiteSpace(keyword.Keyword))
                        //    {
                        //        tw.WriteLine(keyword.Keyword);
                        //    }
                        //}

                        tw.Close();
                    }
                }
                else
                {
                    TextBoxWidgetFile.Text = "Invalid Widget File Name";
                }
            }
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
