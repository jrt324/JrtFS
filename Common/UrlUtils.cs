using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using JrtFS.Common;

namespace JrtFS.Comm
{
    public static class UrlUtils
    {

        private static int? DefaultPort { get; set; }


        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        public static string ResolveUrl(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
                return originalUrl;

            // *** Absolute path - just return
            if (IsAbsolutePath(originalUrl))
                return originalUrl;

            // *** We don't start with the '~' -> we don't process the Url
            if (!originalUrl.StartsWith("~"))
                return originalUrl;

            // *** Fix up path for ~ root app dir directory
            // VirtualPathUtility blows up if there is a 
            // query string, so we have to account for this.
            int queryStringStartIndex = originalUrl.IndexOf('?');
            if (queryStringStartIndex != -1)
            {
                string queryString = originalUrl.Substring(queryStringStartIndex);
                string baseUrl = originalUrl.Substring(0, queryStringStartIndex);

                return string.Concat(
                    VirtualPathUtility.ToAbsolute(baseUrl),
                    queryString);
            }
            else
            {
                return VirtualPathUtility.ToAbsolute(originalUrl);
            }

        }

        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            return ResolveServerUrl(serverUrl, forceHttps, null);
        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="serverUrl">Any Url, either App relative or fully qualified</param>
        /// <param name="forceHttps">if true forces the url to use https</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public static string ResolveServerUrl(string serverUrl, bool forceHttps, int? port)
        {
            if (string.IsNullOrEmpty(serverUrl))
                return serverUrl;

            if (port == null)
            {
                if (DefaultPort == null)
                {
                    var confgDefaultPort = ConfigurationManager.AppSettings["siteDefaultPort"];
                    if (!confgDefaultPort.IsNull())
                    {
                        DefaultPort = int.Parse(confgDefaultPort);
                    }
                    else
                    {
                        DefaultPort = 0;
                    }
                }

                if (DefaultPort != null && DefaultPort.Value != 0)
                {
                    port = DefaultPort;
                }
            }

            // *** Is it already an absolute Url?
            if (IsAbsolutePath(serverUrl))
                return serverUrl;

            string newServerUrl = ResolveUrl(serverUrl);
            Uri result = new Uri(HttpContext.Current.Request.Url, newServerUrl);
            if (port != null && port > 0)
            {
                UriBuilder builder = new UriBuilder(result) { Port = port.Value };
                result = builder.Uri;
            }

            if (!forceHttps)
                return result.ToString();
            else
                return ForceUriToHttps(result).ToString();

        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        /// 
        /// It work like Page.ResolveUrl, but adds these to the beginning.
        /// This method is useful for generating Urls for AJAX methods
        /// </summary>
        /// <param name="serverUrl">Any Url, either App relative or fully qualified</param>
        /// <returns></returns>
        public static string ResolveServerUrl(string serverUrl)
        {
            return ResolveServerUrl(serverUrl, false);
        }

        /// <summary>
        /// Forces the Uri to use https
        /// </summary>
        private static Uri ForceUriToHttps(Uri uri)
        {
            // ** Re-write Url using builder.
            var builder = new UriBuilder(uri) { Scheme = Uri.UriSchemeHttps };
            return builder.Uri;
        }

        public static bool IsAbsolutePath(string originalUrl)
        {
            // *** Absolute path - just return
            int indexOfSlashes = originalUrl.IndexOf("://", StringComparison.Ordinal);
            int indexOfQuestionMarks = originalUrl.IndexOf("?", StringComparison.Ordinal);

            if (indexOfSlashes > -1 &&
                 (indexOfQuestionMarks < 0 ||
                  (indexOfQuestionMarks > -1 && indexOfQuestionMarks > indexOfSlashes)
                  )
                )
                return true;

            return false;
        }

        /// <summary>
        /// 移除url中的query string的某个键
        /// </summary>
        /// <remarks>
        /// RemoveQueryStringByKey("https://www.baidu.com/sclient=psy-ab&q=cookie", "q");
        /// return https://www.baidu.com/sclient=psy-ab
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveQueryStringByKey(string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }

        /// <summary>
        /// 移除HttpRequest请求的Url中的query string的某个键
        /// </summary>
        /// <param name="request"></param>
        public static void RemoveRequestQueryStringByKey(this HttpRequest request, string key)
        {
            PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).
                GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            // make collection editable
            isreadonly.SetValue(request.QueryString, false, null);
            // remove
            request.QueryString.Remove(key);
        }

        //public static Uri AddQuery(this Uri uri, string name, string value)
        //{
        //    // this actually returns HttpValueCollection : NameValueCollection
        //    // which uses unicode compliant encoding on ToString()
        //    var query = HttpUtility.ParseQueryString(uri.Query);

        //    query.Add(name, value);

        //    var uriBuilder = new UriBuilder(uri)
        //    {
        //        Query = query.ToString()
        //    };

        //    return uriBuilder.Uri;
        //}

        public static string AddQueryString(string baseUrl, string name, string value)
        {
            var endChar = baseUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? (baseUrl.EndsWith("&") ? "" : "&") : "?";
            var url = "{0}{1}{2}={3}".FormatWith(baseUrl, endChar, name, HttpUtility.UrlEncode(value));
            return url;
        }

        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var ub = new UriBuilder(uri);

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);
            httpValueCollection.Add(name, value);

            // this code block is taken from httpValueCollection.ToString() method
            // and modified so it encodes strings with HttpUtility.UrlEncode
            if (httpValueCollection.Count == 0)
                ub.Query = String.Empty;
            else
            {
                var sb = new StringBuilder();

                for (int i = 0; i < httpValueCollection.Count; i++)
                {
                    string text = httpValueCollection.GetKey(i);
                    {
                        text = HttpUtility.UrlEncode(text);

                        string val = (text != null) ? (text + "=") : string.Empty;
                        string[] vals = httpValueCollection.GetValues(i);

                        if (sb.Length > 0)
                            sb.Append('&');

                        if (vals == null || vals.Length == 0)
                            sb.Append(val);
                        else
                        {
                            if (vals.Length == 1)
                            {
                                sb.Append(val);
                                sb.Append(HttpUtility.UrlEncode(vals[0]));
                            }
                            else
                            {
                                for (int j = 0; j < vals.Length; j++)
                                {
                                    if (j > 0)
                                        sb.Append('&');

                                    sb.Append(val);
                                    sb.Append(HttpUtility.UrlEncode(vals[j]));
                                }
                            }
                        }
                    }
                }

                ub.Query = sb.ToString();
            }

            return ub.Uri;
        }

        public static string UrlCombine(this string url1, string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }

            if (url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }

        public static string PathCombin(this string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2.TrimStart('/', '\\'));
        }

        /// <summary>
        /// 返回url中QueryString参数的值
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQueryString(string url, string key)
        {
            var queryString = url.Contains("://") ? new Uri(url).Query : url;
            var nameValues = HttpUtility.ParseQueryString(queryString);
            return nameValues[key];
        }

        //public static string Combine(params string[] parts)
        //{
        //    if (parts == null || parts.Length == 0) return string.Empty;

        //    var urlBuilder = new StringBuilder();
        //    foreach (var part in parts)
        //    {
        //        var tempUrl = TryCreateRelativeOrAbsolute(part);
        //        urlBuilder.Append(tempUrl);
        //    }
        //    return VirtualPathUtility.RemoveTrailingSlash(urlBuilder.ToString());
        //}

        //private static string TryCreateRelativeOrAbsolute(string s)
        //{
        //    System.Uri uri;
        //    System.Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri);
        //    string tempUrl = VirtualPathUtility.AppendTrailingSlash(uri.ToString());
        //    return tempUrl;
        //}

    }
}
