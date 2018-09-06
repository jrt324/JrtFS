using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using JrtFS.Common;
using Newtonsoft.Json;

namespace JrtFS.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static HttpResponseMessage ApiResult(this ApiController controller, ApiStatusCodes code, string message)
        {
            var result = new ApiResult(code, message);
            if (controller.Request != null)
            {
                return controller.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(result));//接收:response.Content.ReadAsStringAsync().Result;
                //StringContent content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "text/json");
                HttpResponseMessage response = new HttpResponseMessage() {Content = content};
                return response;
            }
        }

        public static HttpResponseMessage ApiResult(this ApiController controller, ApiStatusCodes code)
        {
            string msg = string.Empty;
            if (code == ApiStatusCodes.Success)
            {
                msg = "调用成功";
            }
            else
            {
                msg = "调用失败!";
            }
            return ApiResult(controller, (ApiStatusCodes) code, msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="returnObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static HttpResponseMessage ApiResult<T>(this ApiController controller, T returnObject)
        {
            var result = new ApiResult<T>(returnObject);
            if (controller.Request != null)
            {
                return controller.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(result));//接收:response.Content.ReadAsStringAsync().Result;
                //StringContent content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "text/json");
                HttpResponseMessage response = new HttpResponseMessage() {Content = content};
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="code"></param>
        /// <param name="returnObject"></param>
        /// <returns></returns>
        public static HttpResponseMessage ApiResult<T>(this ApiController controller, ApiStatusCodes code, T returnObject)
        {
            var result = new ApiResult<T>(code, returnObject);
            if (controller.Request != null)
            {
                return controller.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(result));//接收:response.Content.ReadAsStringAsync().Result;
                //StringContent content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "text/json");
                HttpResponseMessage response = new HttpResponseMessage() { Content = content };
                return response;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static RedirectResult ApiRedirectResult(this ApiController controller, string url)
        {
            Uri tempUri = new Uri(url);
            var result = new System.Web.Http.Results.RedirectResult(tempUri, controller);
            return result;
        }

    }
}