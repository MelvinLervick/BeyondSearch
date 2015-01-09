using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using WebPageWidget.Common;

namespace WebPageWidget
{
    public class WebWidget : IWebWidget
    {
        public WidgetParameters Parameters { get; set; }
        public Dictionary<string, string> Placeholders { get; set; }
        public string StyleContent { get; set; }
        public string HtmlContent { get; set; }
        public string ScriptContent { get; set; }
        public string ConfigContent { get; set; }

        public string ErrorMessage;

        public WebWidget()
        {
            InitializeWidget();
        }

        public WebWidget(string content)
        {
            InitializeWidget();

            if (!string.IsNullOrWhiteSpace(content))
            {
                ExtractWidgetContent(content);
            }
        }
        public void ExtractWidgetContent(string content)
        {
            try
            {
                var contentParameters = JsonConvert.DeserializeObject<WidgetContent>(content);

                StoreWidgetParametersAndContent( contentParameters );
            }
            catch
            {
                InitializeWidget();
            }
        }

        public bool ReadWidgetFile( string fileName )
        {
            try
            {
                using (var r = new StreamReader(fileName))
                {
                    var json = r.ReadToEnd();
                    var contentParameters = JsonConvert.DeserializeObject<WidgetContent>(json);

                    StoreWidgetParametersAndContent(contentParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        public bool WriteWidgetFile( string fileName )
        {
            try
            {
                var widget = new WidgetContent
                {
                    Parameters = Parameters,
                    Placeholders = Placeholders,
                    ConfigContent = ConfigContent,
                    HtmlContent = HtmlContent,
                    ScriptContent = ScriptContent,
                    StyleContent = StyleContent
                };
                var ser = JsonConvert.SerializeObject(widget);

                using (var w = new StreamWriter(fileName))
                {
                    w.Write(ser);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public string CreateWidget()
        {
            return StyleContent + HtmlContent;
        }

        public string ScanLinks(string url)
        {
            // Download page
            var client = new WebClient();
            var html = client.DownloadString(url);
            var links = new StringBuilder();

            // Scan links on this page
            HtmlTag tag;
            var parse = new HtmlParser(html);
            while (parse.ParseNext("a", out tag))
            {
                // See if this anchor links to us
                string value;
                if (tag.Attributes.TryGetValue("href", out value))
                {
                    // value contains URL referenced by this link
                    links.AppendLine(value);
                }
            }

            return links.ToString();
        }

        private void InitializeWidget()
        {
            Parameters = new WidgetParameters();
            Placeholders = new Dictionary<string, string>();
            StyleContent = string.Empty;
            HtmlContent = string.Empty;
            ScriptContent = string.Empty;
            ConfigContent = string.Empty;
        }

        private void StoreWidgetParametersAndContent(WidgetContent contentParameters)
        {
            Parameters.Name = contentParameters.Parameters.Name;
            Parameters.Author = contentParameters.Parameters.Author;
            Parameters.Version = contentParameters.Parameters.Version;
            Parameters.Type = contentParameters.Parameters.Type;
            Parameters.Locked = contentParameters.Parameters.Locked;
            Parameters.Encrypted = contentParameters.Parameters.Encrypted;
            Parameters.Key = contentParameters.Parameters.Key;

            StyleContent = contentParameters.StyleContent;
            HtmlContent = contentParameters.HtmlContent;
            ScriptContent = contentParameters.ScriptContent;
            ConfigContent = contentParameters.ConfigContent;
        }
    }
}