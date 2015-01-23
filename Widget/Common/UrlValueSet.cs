using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace WebPageWidget.Common
{
    public class UrlValueSet : IEnumerable<string>
    {
        protected NameValueCollection Collection = new NameValueCollection();

        /// <summary>
        /// Add a parameter to the set
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <param name="urlEncode">If set to true, value will be URL encoded.</param>
        /// <param name="isRequiredParameter">If set to true, the parameter will be added even if the value is null or whitespace.</param>
        public void Add(string name, object value, bool urlEncode = true, bool isRequiredParameter = false)
        {
            Add(name, value.ToString(), urlEncode, isRequiredParameter);
        }

        /// <summary>
        /// Add a parameter to the set
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <param name="urlEncode">If set to true, value will be URL encoded.</param>
        /// <param name="isRequiredParameter">If set to true, the parameter will be added even if the value is null or whitespace.</param>
        public virtual void Add(string name, string value, bool urlEncode = true, bool isRequiredParameter = false)
        {
            if (!String.IsNullOrWhiteSpace(name) &&
                (isRequiredParameter || !String.IsNullOrWhiteSpace(value)))
            {
                value = value ?? String.Empty;
                Collection.Add(name, urlEncode ? HttpUtility.UrlEncode(value) : value);
            }
        }

        /// <summary>
        /// Checks whether a parameter already exists in the set
        /// </summary>
        /// <param name="name">The name of the parameter to find</param>
        /// <returns>True if the parameter is found, otherwise false</returns>
        public bool Contains(string name)
        {
            return Collection[name] != null;
        }

        /// <summary>
        /// Concatenated all parmaters in the set with the '&' delimiter
        /// </summary>
        /// <returns>A string containing all parameters concatenated together</returns>
        public override string ToString()
        {
            return String.Join("&", this);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return (from string key in Collection from value in Collection.GetValues(key) select key + '=' + value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
