using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JrtFS
{
    /// <summary>
    /// api返回码 
    /// 所有的api返回码应该都注册到返回码列表里面 100一个业务区间 之后增加code码需增加在所在的业务区间
    /// </summary>
    public enum FsAPiCode
    {
        /// <summary>
        ///  验证码出错
        /// </summary>
        [Description("验证码出错")]
        VerifyError = -10,

        /// <summary>
        /// access_token过期失效
        /// </summary>
        TokenExpire = -9,

        /// <summary>
        /// access_token无效
        /// </summary>
        TokenInvalid = -8,

        /// <summary>
        /// 请求超时
        /// </summary>
        ReqOverTime = -7,


        /// <summary>
        /// 未找到或不正确
        /// </summary>
        NotExist = -5,

        /// <summary>
        /// 签名错误
        /// </summary>
        SignErr = -4,

        /// <summary>
        /// 签名验证失败 
        /// </summary>
        SignInvalid = -3,



        /// <summary>
        /// 缺少参数 -2
        /// </summary>
        MisParameter = -2,


        /// <summary>
        /// 普通错误 -1
        /// </summary>
        NormalError = -1,


        /// <summary>
        /// 正常返回 1
        /// </summary>
        Success = 1,

    }
}