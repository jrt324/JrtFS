using Newtonsoft.Json;

namespace JrtFS.Common
{
    /// <summary>
    /// 开放平台Api返回结果定义
    /// </summary>
    public class ApiResult : IApiResult
    {
        public static ApiResult Ok
        {
            get
            {
                return new ApiResult(ApiStatusCodes.Success);
            }
        }

        public ApiResult()
        {
            this.ret = (int)ApiStatusCodes.Success;
        }

        public ApiResult(object data)
        {
            this.ret = (int)ApiStatusCodes.Success;
            this.data = data;
        }

        public ApiResult(ApiStatusCodes code, string msg)
        {
            this.ret = (int)code;
            this.msg = msg;
        }

        public ApiResult(int ret, string msg)
        {
            this.ret = ret;
            this.msg = msg;
        }

        /// <summary>
        /// 设置返回错误代码和错误信息
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ApiResult Return(int ret, string msg)
        {
            this.ret = ret;
            this.msg = msg;
            return this;
        }

        #region 属性

        /// <summary>
        /// 设置返回值的消息体
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 返回状态，1为正确，其他值表示调用返回错误，具体错误原因参见msg
        /// </summary>
        public int ret { get; set; }


        private string _msg = string.Empty;

        /// <summary>
        /// 接口调用时发生错误的描述
        /// </summary>
        public string msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public double ExcuteTime { get; set; }

        #endregion

        /// <summary>
        /// 调用是否成功
        /// </summary>
        [JsonIgnore]
        public bool Success
        {
            get { return ret == (int)ApiStatusCodes.Success; }
        }
    }
}