using System;

namespace WebPageWidget
{
    [Serializable]
    public class WidgetParameters
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }

        public WidgetParameters()
        {
        }
    }
}