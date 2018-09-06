using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using JrtFS.Common;
using JrtFS.Filters;
using JrtFS.Models;

using Newtonsoft.Json;

namespace JrtFS.Controllers
{
    [RoutePrefix("fs")]
    public partial class FileController : ApiController
    {
        /// <summary>
        /// get file stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("getFile")]
        public HttpResponseMessage GetFile(string fileName)
        {
            var bucket = this.GetBucket();
            string path = FileConfig.MapPath(bucket, fileName);
            if (!File.Exists(path))
            {
                return this.ApiResult(ApiStatusCodes.NotExist, "文件不存在");
            }

            try
            {
                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(path, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
                response.Content = new StreamContent(responseStream);
                return response;
            }
            catch (IOException)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, "读取文件失败，请重试");
            }
        }



        [HttpPost, AuthorizedAccess]
        [Route("uploadFile")]
        public async Task<HttpResponseMessage> UploadFile()
        {
            var bucket = this.GetBucket();
            var rootPath = FileConfig.MapPath(bucket, "");

            //var filePath = FileConfig.MapPath(fileName);
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, string.Empty);
            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new CustomMultipartFormDataStreamProvider(rootPath, bucket);
                //new MultipartFormDataStreamProvider(@"C:\tmp", Int32.MaxValue)

                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                var task = await content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        return this.ApiResult(ApiStatusCodes.NormalError, "存储文件失败");
                    }


                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        return new FileDesc(info.Name, rootUrl + "/" + info.Name, info.Length / 1024);
                    });
                    ;
                    return this.ApiResult(fileInfo.ToList());
                });

                return task;
            }
            else
            {
                return this.ApiResult(ApiStatusCodes.NormalError, "表单文件格式不正确");
            }
        }



        ///// <summary>
        ///// 上传文件
        ///// </summary>
        ///// <param name="fileName">文件名</param>
        ///// <returns></returns>
        //[Route("upload")]
        //[HttpPost]
        //public HttpResponseMessage Upload([FromUri]string fileName)
        //{
        //    var task = this.Request.Content.ReadAsStreamAsync();
        //    task.Wait();
        //    Stream requestStream = task.Result;

        //    try
        //    {
        //        var filePath = FileConfig.MapPath(fileName);

        //        Stream fileStream = File.Create(filePath);
        //        requestStream.CopyTo(fileStream);
        //        fileStream.Close();
        //        requestStream.Close();
        //    }
        //    catch (IOException ex)
        //    {
        //        return this.ApiResult(FsAPiCode.NormalError, ex.Message);
        //    }

        //    return this.ApiResult(FsAPiCode.Success);
        //}


    }



}