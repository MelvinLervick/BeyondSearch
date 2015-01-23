using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using WebPageWidget.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using WebPageWidget.ContentManagers;

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

                StoreWidgetParametersAndContent(contentParameters);
            }
            catch
            {
                InitializeWidget();
            }
        }

        public bool ReadWidgetFile(string fileName)
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

        public bool WriteWidgetFile(string fileName)
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

        static string html = string.Empty;
        public string ScanForLinks(string url)
        {
            var links = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;


            try
            {
                // Download page
                //var client = new WebClient();
                //html = client.DownloadString(url);
                ////var request = (HttpWebRequest)WebRequest.Create(url);
                ////request.Method = "GET";
                ////response = request.GetResponse();
                ////reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ////html = reader.ReadToEnd();
                //RunAsync(url).Wait();
                //////var getUrlSource = new UrlContentManager(url);
                //////getUrlSource.GetContentAsync();

                //////while (!getUrlSource.Complete)
                //////{
                //////    Thread.Sleep(100);
                //////}

                //////if (getUrlSource.ErrorMessage.Length > 0)
                //////{
                //////    ErrorMessage = getUrlSource.ErrorMessage;
                //////}
                //////html = getUrlSource.ReturnValue;

                html = url;

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
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return links.ToString();
        }
        static async Task RunAsync(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/html"));

            //client.BaseAddress = new Uri("http://s2csportdiver.devciinspsearch.com/sprtd");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/text"));

            //// New code:
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            //html = await response.Content.ReadAsStringAsync();
        }

        public string ScanForTags(string url, string parseTag)
        {
            var links = new StringBuilder();
            var html = string.Empty;

            try
            {
                html = url;
                // Download page
                //using (var client = new WebClient())
                //{
                //    html = client.DownloadString(url);
                //} 
                
                //var client = new WebClient();
                //var html = client.DownloadString(url);
                //var filename = System.IO.Path.GetTempFileName();
                //client.DownloadFile(url, filename);

                var doc = new HtmlAgilityPack.HtmlDocument();
                //doc.Load(filename);
                //var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);//.Load(url);
                var root = doc.DocumentNode;
                var aNodes = root.Descendants(parseTag).ToList();
                foreach (var node in aNodes)
                {
                    links.AppendLine(node.OuterHtml);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
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