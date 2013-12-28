using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeyondSearch
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly object dummyNode = null;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Menu_FileExitClick( object sender, RoutedEventArgs e )
		{
			Application app = Application.Current;
			app.Shutdown();
		}

    private void Menu_ResearchClick( object sender, RoutedEventArgs e )
    {
      var researchPage = new Research();
      researchPage.Show();
    }

    public string SelectedImagePath
		{
			get;
			set;
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			foreach (string s in Directory.GetLogicalDrives())
			{
				var item = new TreeViewItem {Header = s, Tag = s, FontWeight = FontWeights.Normal};
			    item.Items.Add( dummyNode );
				item.Expanded += new RoutedEventHandler( folder_Expanded );
				FoldersItem.Items.Add( item );
			}
		}

		void folder_Expanded( object sender, RoutedEventArgs e )
		{
			var item = (TreeViewItem)sender;
			if (item.Items.Count == 1 && item.Items[0] == dummyNode)
			{
				item.Items.Clear();
				try
				{
					foreach (string s in Directory.GetDirectories( item.Tag.ToString() ))
					{
						var subitem = new TreeViewItem
						{
						    Header = s.Substring(s.LastIndexOf("\\") + 1),
						    Tag = s,
						    FontWeight = FontWeights.Normal
						};
					    subitem.Items.Add( dummyNode );
						subitem.Expanded += new RoutedEventHandler( folder_Expanded );
						item.Items.Add( subitem );
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private void foldersItem_SelectedItemChanged( object sender, RoutedPropertyChangedEventArgs<object> e )
		{
			var tree = (TreeView)sender;
			var temp = ((TreeViewItem)tree.SelectedItem);

			if (temp == null)
				return;
			SelectedImagePath = "";
		    string temp2 = "";
			while (true)
			{
				string temp1 = temp.Header.ToString();
				if (temp1.Contains( @"\" ))
				{
					temp2 = "";
				}
				SelectedImagePath = temp1 + temp2 + SelectedImagePath;
				if (temp.Parent.GetType() == typeof( TreeView ))
				{
					break;
				}
				temp = ((TreeViewItem)temp.Parent);
				temp2 = @"\";
			}
			//show user selected path
			//MessageBox.Show( SelectedImagePath );
            if (this.BuildDirectoryAndFileList(SelectedImagePath))
            {
                FolderList.ItemsSource = this.SelectedFolderDirectoryNamesAndFileNames;
            }
		}

	    public List<FolderItem> SelectedFolderDirectoryNamesAndFileNames { get; set; }
        private bool BuildDirectoryAndFileList(string path)
        {
            DirectoryInfo filesAndDirectories = new DirectoryInfo(path);
            this.SelectedFolderDirectoryNamesAndFileNames = new List<FolderItem>();
            
            this.AddDirectoryNamesToSelectedFolderItems(filesAndDirectories);

            this.AddFileNamesToSelectedFolderItems(filesAndDirectories);

            return true;
        }

	    private void AddFileNamesToSelectedFolderItems(DirectoryInfo filesAndDirectories)
	    {
	        FileSystemInfo[] files = filesAndDirectories.GetFiles();
	        foreach (var file in files)
	        {
	            this.SelectedFolderDirectoryNamesAndFileNames.Add(new FolderItem(file.Name, file.Extension.Replace(".", "")));
	        }
	    }

	    private void AddDirectoryNamesToSelectedFolderItems(DirectoryInfo filesAndDirectories)
	    {
	        DirectoryInfo[] directories = filesAndDirectories.GetDirectories();
	        foreach (var directory in directories)
	        {
	            this.SelectedFolderDirectoryNamesAndFileNames.Add(new FolderItem(directory.Name));
	        }
	    }
	}
}
