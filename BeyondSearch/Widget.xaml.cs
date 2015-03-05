using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BeyondSearch.IWidget;
using HtmlAgilityPack;
using Microsoft.Win32;
using mshtml;
using WebPageWidget;
using WebPageWidget.Common;

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

        private void Menu_ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_FileNewOnClick(object sender, RoutedEventArgs e)
        {
            TextBoxWidgetName.IsReadOnly = false;
            TextBoxWidgetName.Focus();
            TextBoxWidgetFolder.Text = string.Empty;
            TextBoxWidgetFile.Text = string.Empty;

            workWidget = new WebWidget();
            UpdateWidgetDisplayFields();
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
                if (!string.IsNullOrWhiteSpace(TextBoxWidgetFolder.Text) ||
                     !string.IsNullOrWhiteSpace(TextBoxWidgetFile.Text))
                {
                    if (workWidget.ReadWidgetFile(System.IO.Path.Combine(TextBoxWidgetFolder.Text,
                            TextBoxWidgetFile.Text)))
                    {
                        UpdateWidgetDisplayFields();
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
                InitialDirectory = string.IsNullOrWhiteSpace(TextBoxWidgetFolder.Text) ? ConfigurationManager.AppSettings["WidgetTestFolder"] : TextBoxWidgetFolder.Text,
                FileName = string.IsNullOrWhiteSpace(TextBoxWidgetFile.Text) ? TextBoxWidgetName.Text : TextBoxWidgetFile.Text,
                DefaultExt = "json",
                CheckPathExists = true,
                Filter = "Widget Files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (saveFile.ShowDialog() ?? false)
            {
                // If the file name is not an empty string open it for saving.
                if (saveFile.FileName != "")
                {
                    // http://james.newtonking.com/json/help/index.html
                    StoreWidgetDisplayFields();
                    if (!workWidget.WriteWidgetFile(saveFile.FileName))
                    {
                        TextBoxErrorMessage.Text = string.Format("Unable to save widget: {0}", workWidget.ErrorMessage);
                    }
                }
                else
                {
                    TextBoxErrorMessage.Text = "Invalid Widget File Name";
                }
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            ButtonCreate.IsEnabled = false;
            workWidget.Parameters.Name = TextBoxWidgetName.Text;
            UpdateWidgetDisplayFields();
        }

        private void TextBoxWidgetName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Check to see if widget with specified name already exists
            // If not
            ButtonCreate.IsEnabled = true;
            // Else Display message "Widget with the specified name already exists."
        }

        private void Menu_DisplayWidgetOnClick(object sender, RoutedEventArgs e)
        {
            var testFolder = ConfigurationManager.AppSettings["WidgetTestFolder"];
            var fileName = System.IO.Path.Combine(testFolder, TextBoxParameterName.Text + ".html");

            StoreWidgetDisplayFields();
            var testWidget = workWidget.CreateWidget();

            string html = testWidget;

            while (html.Contains("<widget"))
            {
                var poss = html.IndexOf("<widget", 0, System.StringComparison.Ordinal);
                var pose = html.IndexOf(">", poss, System.StringComparison.Ordinal);
                var widget = html.Substring(poss, pose - poss + 1);
                var posNs = widget.IndexOf("name=\"", 0, System.StringComparison.Ordinal) + 6;
                var posNe = widget.IndexOf("\"", posNs, System.StringComparison.Ordinal);
                var widgetName = widget.Substring(posNs, posNe - posNs);

                var widgetFileName = System.IO.Path.Combine(testFolder, widgetName + ".json");
                var dWidget = new WebWidget();
                if (dWidget.ReadWidgetFile(widgetFileName))
                {
                    var wHtml = dWidget.CreateWidget();
                    html = html.Replace(widget, wHtml);
                }
            }

            using (var w = new StreamWriter(fileName))
            {
                w.Write(html);
                w.Flush();
            }

            ExampleBrowser.Navigate(string.Format("file:///{0}", fileName));

        }

        internal bool GetSource = false;
        private void GetSource_Click(object sender, RoutedEventArgs e)
        {
            GetSource = true;
            ExampleBrowser.Navigate(new Uri(TextBoxUrl.Text));

            var testWidget = new WidgetServiceClient();
            TextBoxErrorMessage.Text = testWidget.GetData("Calling Test WidgetService");

            //TextBoxLinks.Text = string.IsNullOrWhiteSpace(TextBoxTag.Text)
            //        ? workWidget.ScanForLinks(ExampleBrowser.Document.ToString())
            //        : workWidget.ScanForTags(ExampleBrowser.Document.ToString(), TextBoxTag.Text);

            //if (!string.IsNullOrWhiteSpace(workWidget.ErrorMessage))
            //{
            //    TextBoxErrorMessage.Text = workWidget.ErrorMessage;
            //    workWidget.ErrorMessage = string.Empty;
            //}
            //else
            //{
            //    TextBoxErrorMessage.Text = string.Empty;
            //}
        }

        #region Local Methods

        private void UpdateWidgetDisplayFields()
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

        private void StoreWidgetDisplayFields()
        {
            workWidget.Parameters.Name = TextBoxWidgetName.Text;
            workWidget.Parameters.Name = TextBoxParameterName.Text;
            workWidget.Parameters.Author = TextBoxParameterAuthor.Text;
            workWidget.Parameters.Version = TextBoxParameterVersion.Text;
            workWidget.Parameters.Type = (WidgetType)Enum.Parse(typeof(WidgetType), TextBoxParameterType.Text, true);
            workWidget.Parameters.Locked = CheckBoxParameterLocked.IsChecked ?? false;
            workWidget.Parameters.Encrypted = CheckBoxParameterEncrypted.IsChecked ?? false;
            workWidget.Parameters.Key = TextBoxParameterKey.Text;

            workWidget.HtmlContent = TextBoxCodeSnippet.Text;
            workWidget.StyleContent = TextBoxStyling.Text;
        }

        #endregion Local Methods

        private void ExampleBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (GetSource)
            {
                GetSource = false;
                var doc = ExampleBrowser.Document as mshtml.HTMLDocument;

                TextBoxLinks.Text = string.IsNullOrWhiteSpace(TextBoxTag.Text)
                        ? workWidget.ScanForLinks(doc.documentElement.outerHTML)
                        : workWidget.ScanForTags(doc.documentElement.outerHTML, TextBoxTag.Text);

                if (!string.IsNullOrWhiteSpace(workWidget.ErrorMessage))
                {
                    TextBoxErrorMessage.Text = workWidget.ErrorMessage;
                    workWidget.ErrorMessage = string.Empty;
                }
                else
                {
                    TextBoxErrorMessage.Text = string.Empty;
                }
            }
        }
    }
}
