using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JrtFS.Controllers;

namespace JrtFS.Common
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        internal static class MultipartFormDataStreamProviderHelper
        {
            public static bool IsFileContent(HttpContent parent, HttpContentHeaders headers)
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent");
                }
                if (headers == null)
                {
                    throw new ArgumentNullException("headers");
                }
                ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
                if (contentDisposition == null)
                {
                    //Resources.MultipartFormDataStreamProviderNoContentDisposition
                    throw new InvalidOperationException("Content-Disposition没有文件");
                }
                return !string.IsNullOrEmpty(contentDisposition.FileName);
            }

            public static string UnquoteToken(string token)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return token;
                }
                if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
                {
                    return token.Substring(1, token.Length - 2);
                }
                return token;
            }

            public static async Task ReadFormDataAsync(Collection<HttpContent> contents, NameValueCollection formData, CancellationToken cancellationToken)
            {
                foreach (HttpContent content in contents)
                {
                    ContentDispositionHeaderValue contentDisposition = content.Headers.ContentDisposition;
                    if (string.IsNullOrEmpty(contentDisposition.FileName))
                    {
                        string formFieldName = UnquoteToken(contentDisposition.Name) ?? string.Empty;
                        cancellationToken.ThrowIfCancellationRequested();
                        string formFieldValue = await content.ReadAsStringAsync();
                        formData.Add(formFieldName, formFieldValue);
                    }
                }
            }
        }


        private string Bucket;

        public CustomMultipartFormDataStreamProvider(string path, string bucket)
            : base(path)
        {
            this.Bucket = bucket;
        }


        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";

            //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
            return name.Replace("\"", string.Empty);
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (MultipartFormDataStreamProviderHelper.IsFileContent(parent, headers))
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent");
                }
                if (headers == null)
                {
                    throw new ArgumentNullException("headers");
                }
                string path;
                try
                {
                    string localFileName = this.GetLocalFileName(headers);
                    path = Path.Combine(this.RootPath, localFileName);
                    //path = Path.Combine(this.RootPath, Path.GetFileName(localFileName));

                    var dir= Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                catch (Exception innerException)
                {
                    throw new InvalidOperationException("无效的文件名", innerException);
                }

                MultipartFileData item = new MultipartFileData(headers, path);
                this.FileData.Add(item);
                return File.Create(path, this.BufferSize, FileOptions.Asynchronous);
            }
            return new MemoryStream();
        }
    }

}