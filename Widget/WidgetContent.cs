using System;

namespace WebPageWidget
{
    [Serializable]
    public class WidgetContent
    {
        public WidgetParameters Parameters;
        public string CssContent;
        public string HtmlContent;
    }
}
