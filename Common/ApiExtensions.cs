using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using JrtFS.Common;

namespace JrtFS.Common
{
    public static class ApiExtensions
    {
        private static bool? _allowEmptyBucket;
        public static bool AllowEmptyBucket
        {
            get
            {
                if (_allowEmptyBucket == null)
                {
                    _allowEmptyBucket = ConfigurationManager.AppSettings["AllowEmptyBucket"] == "true";
                }

                return _allowEmptyBucket.Value;
            }
        }

        /// <summary>
        /// 根据设备id
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string GetBucket(this ApiController controller)
        {
            var bukcet = controller.Request.GetHeaderValue("Bucket");
            if (!AllowEmptyBucket && bukcet.IsNull())
            {
                throw new ArgumentNullException("bukcet", "不能为空");
            }
            return bukcet;
        }

        public static string IsValidRequest(this HttpRequestMessage request)
        {
            var bukcet = request.GetHeaderValue("Bucket");
            var accessKeyId = request.GetHeaderValue("AccessKeyId");
            var accessKeySecret = request.GetHeaderValue("AccessKeySecret");
            if (!AllowEmptyBucket && bukcet.IsNull())
            {
                throw new ArgumentNullException("Bucket", "不能为空");
            }
            if (accessKeyId.IsNull())
            {
                throw new ArgumentNullException("AccessKeyId", "不能为空");
            }
            if (accessKeySecret.IsNull())
            {
                throw new ArgumentNullException("AccessKeySecret", "不能为空");
            }
            var isVaild = ApiSecertUtils.Vaild(accessKeyId, accessKeySecret);
            if (!isVaild)
            {
                throw new UnauthorizedAccessException("未经许可的授权");
            }
            return bukcet; ;
        }

        public static string GetHeaderValue(this ApiController controller, string key)
        {
            return GetHeaderValue(controller.Request, key);
        }

        public static string GetHeaderValue(this HttpRequestMessage request, string key)
        {
            if (request.Headers.Contains(key))
            {
                return request.Headers.GetValues(key).First();
            }
            return null;
        }
    }
}