using Newtonsoft.Json;

namespace WebPageWidget
{
    public class Widget : IWidget
    {
        public WidgetParameters Parameters { get; set; }
        public string CssContent { get; set; }
        public string HtmlContent { get; set; }
        public Widget(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                ExtractWidgetContent(content);
            }
            else
            {
                InitializeWidget();
            }
        }
        public void ExtractWidgetContent(string content)
        {
            try
            {
                var contentParameters = JsonConvert.DeserializeObject<WidgetContent>(content);

                Parameters.Name = contentParameters.Parameters.Name;
                Parameters.Author = contentParameters.Parameters.Author;
                Parameters.Version = contentParameters.Parameters.Version;

                CssContent = contentParameters.CssContent;
                HtmlContent = contentParameters.HtmlContent;
            }
            catch
            {
                InitializeWidget();
            }
        }

        private void InitializeWidget()
        {
            Parameters = new WidgetParameters();
            CssContent = string.Empty;
            HtmlContent = string.Empty;
        }
    }
}