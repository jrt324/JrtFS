using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace JrtFS.Common
{
    public class CustomMultipartFormDataStreamProvider1 : MultipartFormDataStreamProvider
    {


        private string Bucket;

        public CustomMultipartFormDataStreamProvider1(string path, string bucket)
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


    }
}