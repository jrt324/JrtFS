using System.ComponentModel;

namespace JrtFS.Common
{
    /// <summary>
    /// 常规API调用返回状态码
    /// </summary>
    public enum ApiStatusCodes
    {
        /// <summary>
        /// 正常返回 1
        /// </summary>
        Success = 1,

        /// <summary>
        /// 验证码出错
        /// </summary>
        [Description("验证码出错")]
        VerifyError = -10,

        /// <summary>
        /// access_token过期失效 -9
        /// </summary>
        TokenExpire = -9,

        /// <summary>
        /// access_token无效 -8
        /// </summary>
        TokenInvalid = -8,


        /// <summary>
        /// 请求超时 -7
        /// </summary>
        ReqOverTime = -7,

        /// <summary>
        /// 参数不存在 -6
        /// </summary>
        ParamNoFull = -6,
        /// <summary>
        /// 未找到或不正确 -5
        /// </summary>
        NotExist = -5,

        /// <summary>
        /// sign不正确(签名验证失败) -4
        /// </summary>
        SignErr = -4,

        /// <summary>
        /// accesstoken登录失效 -3
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
        /// 特殊错误
        /// </summary>
        SpctialError = -500,

 
    }
}