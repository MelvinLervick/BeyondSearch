using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebPageWidget.Common;

namespace WebPageWidget.ContentManagers
{
    class UrlContentManager : WebClient
    {
        private static string returnValue;
        //private static HttpClient client;
        private static bool complete;
        private static string endpointUrl;
        private static string errorMessage;

        public bool Complete { get { return complete; } }
        public string ReturnValue { get { return returnValue; } }
        public string ErrorMessage { get { return errorMessage; } }

        public UrlContentManager(string url)
        {
            //client = new HttpClient();
            endpointUrl = url;
        }
        public void GetContentAsync()
        {
            complete = false;
            errorMessage = string.Empty;

            base.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/html"));

            RunAsync();
        }

        private async void RunAsync()
        {
            try
            {
                returnValue = await GetUrlResponseData();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            complete = true;
        }

        private async Task<string> GetUrlResponseData()
        {
            var url = new UrlBuilder(endpointUrl);

            returnValue = string.Empty;

            //var cancellationTokenSource = new CancellationTokenSource(1000); //timeout
            var response = await base.GetAsync(url.ToString());

            // Check that response was successful or throw exception
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadAsStringAsync());
        }
    }
}
