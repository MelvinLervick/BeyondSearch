using System;
using WebPageWidget.Common;

namespace WebPageWidget
{
    [Serializable]
    public class WidgetParameters
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public WidgetType Type { get; set; }
        public bool Locked { get; set; }
        public bool Encrypted { get; set; }
        public string Key { get; set; }

        public WidgetParameters()
        {
            Name = "Enter Widget's Name";
            Author = "Enter Author's Name";
            Version = "0";
            Type = WidgetType.Site;
            Locked = false;
            Encrypted = false;
            Key = string.Empty;
        }
    }
}