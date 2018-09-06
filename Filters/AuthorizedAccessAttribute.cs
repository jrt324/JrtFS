using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using System.Web.SessionState;
using JrtFS.Common;


namespace JrtFS.Filters
{
    /// <summary>
    /// 验证用户访问Api的合法性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class AuthorizedAccessAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthorizedAccessAttribute()
        {
        }


        /// <summary>
        /// api controller 访问拦截器
        /// 集成权限验证，数据库访问，日志记录，错误拦截，api约定输出
        /// </summary>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ApiResult apiResult = null;

            #region 获取token对应的用户信息

            try
            {
                var bucket = actionContext.Request.IsValidRequest();
            }
            catch (Exception e)
            {
                apiResult = new ApiResult(ApiStatusCodes.NormalError, e.Message);
            }


            #endregion

            //返回错误信息
            if (apiResult != null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, apiResult);
            }
        }

    }
}