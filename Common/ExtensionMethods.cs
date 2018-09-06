using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JrtFS.Common
{
    public static class ExtensionMethods
    {
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        /// <summary>
        /// 转换数组对象为用Delimeter分隔的字符串
        /// </summary>
        /// <param name="s">要转换的数组对象</param>
        /// <param name="Delimeter">分割符</param>
        /// <param name="Quotation">每个数组成员之间用括号括起来，该处一般为" 或 '</param>
        /// <returns></returns>
        public static string Join(this IEnumerable s, string Delimeter, string Quotation)
        {
            string result = string.Empty;
            string vDelim = string.Empty;

            foreach (object el in s)
            {
                if (!string.IsNullOrEmpty(Quotation))
                {
                    result += vDelim + Quotation + el.ToString() + Quotation;
                }
                else
                {
                    result += vDelim + el.ToString();
                }
                vDelim = Delimeter;
            }
            return result;
        }
    }
}