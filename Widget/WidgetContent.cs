using System;
using System.Collections.Generic;

namespace WebPageWidget
{
    [Serializable]
    public class WidgetContent
    {
        public WidgetParameters Parameters;
        public Dictionary<string, string> Placeholders; 
        public string StyleContent;
        public string HtmlContent;
        public string ScriptContent;
        public string ConfigContent;
    }
}
