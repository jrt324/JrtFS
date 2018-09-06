//using System;
//using System.ComponentModel;

//namespace JrtFS
//{

//    public interface IApiResult
//    {
//        /// <summary>
//        /// 执行时间
//        /// </summary>
//        double ExcuteTime { get; set; }
//    }

//    /// <summary>
//    /// 开放平台Api返回结果定义
//    /// </summary>
//    public class ApiResult : IApiResult
//    {
//        public ApiResult()
//        {
//            this.ret = (int)FsAPiCode.Success;
//        }

//        public ApiResult(object data)
//        {
//            this.ret = (int)FsAPiCode.Success;
//            this.data = data;
//        }

//        public ApiResult(FsAPiCode code, string msg)
//        {
//            this.ret = (int)code;
//            this.msg = msg;
//        }

//        public ApiResult(int ret, string msg)
//        {
//            this.ret = ret;
//            this.msg = msg;
//        }

//        /// <summary>
//        /// 设置返回错误代码和错误信息
//        /// </summary>
//        /// <param name="ret"></param>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public ApiResult Return(int ret, string msg)
//        {
//            this.ret = ret;
//            this.msg = msg;
//            return this;
//        }

//        #region 属性

//        /// <summary>
//        /// 设置返回值的消息体
//        /// </summary>
//        public object data { get; set; }

//        /// <summary>
//        /// 返回状态，1为正确，其他值表示调用返回错误，具体错误原因参见msg
//        /// </summary>
//        public int ret { get; set; }


//        private string _msg = string.Empty;

//        /// <summary>
//        /// 接口调用时发生错误的描述
//        /// </summary>
//        public string msg
//        {
//            get { return _msg; }
//            set { _msg = value; }
//        }

//        public double ExcuteTime { get; set; }

//        #endregion
//    }


//    /// <summary>
//    /// 开放平台Api返回结果定义
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class ApiResult<T> : IApiResult
//    {
//        public ApiResult()
//        {
//            this.ret = (int)FsAPiCode.Success;
//        }

//        public ApiResult(T data)
//        {
//            this.ret = (int)FsAPiCode.Success;
//            this.data = data;
//        }

//        public ApiResult(FsAPiCode code, T data)
//        {
//            this.ret = (int)code;
//            this.data = data;
//        }

//        public ApiResult(FsAPiCode code, string msg)
//        {
//            this.ret = (int)code;
//            this.msg = msg;
//        }

//        public ApiResult(int ret, string msg)
//        {
//            this.ret = ret;
//            this.msg = msg;
//        }

//        /// <summary>
//        /// 设置返回错误代码和错误信息
//        /// </summary>
//        /// <param name="ret"></param>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public ApiResult<T> Return(int ret, string msg)
//        {
//            this.ret = ret;
//            this.msg = msg;
//            return this;
//        }


//        /// <summary>
//        /// 设置返回值的消息体
//        /// </summary>
//        public T data { get; set; }

//        /// <summary>
//        /// 返回状态码，1为正确，其他值表示调用返回错误，具体错误原因参见msg
//        /// </summary>
//        public int ret { get; set; }


//        private string _msg = string.Empty;

//        /// <summary>
//        /// 接口调用时发生错误的描述
//        /// </summary>
//        public string msg
//        {
//            get { return _msg; }
//            set { _msg = value; }
//        }

//        public static implicit operator ApiResult<T>(Enum value)
//        {
//            var result = new ApiResult<T>();
//            result.ret = Convert.ToInt32(value);
//            return result;
//        }

//        /// <summary>
//        /// 指向时间（毫秒）
//        /// </summary>
//        public double ExcuteTime { get; set; }

//        //public static implicit operator ApiResult<T>(int otherType)
//        //{
//        //    return new SizeType
//        //    {
//        //        InternalValue = otherType
//        //    };
//        //}
//    }





//}
