using System;
using System.Windows.Media.Imaging;

namespace BeyondSearch
{
    public class FolderItem
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public BitmapImage Image { get; set; }

        public FolderItem(string name)
        {
            Name = name;
            Extension = string.Empty;

            Image = this.GetImageBitmap(Extension);
        }

        public FolderItem(string name, string extension)
        {
            Name = name;
            Extension = extension;

            Image = this.GetImageBitmap(extension);
        }

        private BitmapImage GetImageBitmap(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Folder.png"));
            }

            return new BitmapImage(new Uri("pack://application:,,,/Resources/Folder.png"));
        }
    }
}
