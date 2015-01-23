namespace WebPageWidget.Common
{
    public class UrlBuilder : UrlValueSet
    {
        readonly bool hasQuery;

        /// <summary>
        /// The base URL which this object was initalized with.
        /// </summary>
        /// <remarks>
        /// BaseUrl may differ from the initial URL if the initial URL was suffixed with '?'
        /// </remarks>
        public string BaseUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Initialize the class with a base URL.
        /// </summary>
        /// <param name="baseUrl">A fully-formed base URL to build parameters upon</param>
        /// <remarks>
        /// If baseUrl ends with '?', it will be trimmed.
        /// </remarks>
        public UrlBuilder(string baseUrl)
        {
            int queryIndex = baseUrl.IndexOf('?');
            bool trimQuery = queryIndex == baseUrl.Length - 1;

            hasQuery = !trimQuery && queryIndex > -1;
            BaseUrl = trimQuery ? baseUrl.Remove(queryIndex) : baseUrl;
        }

        public string GetParameters()
        {
            return base.ToString();
        }

        /// <summary>
        /// Returns a fully-formed URL string from the BaseUrl and parameter set
        /// </summary>
        /// <returns>A fully-formed URL string</returns>
        public override string ToString()
        {
            return BaseUrl + (hasQuery ? '&' : '?') + base.ToString();
        }
    }
}
