using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Http
{

    /// <summary>
    /// 检测WebApi 的Action是否支持多参数，可以传递Json格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MultiParameterSupportAttribute : Attribute
    {

    }
}