using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JrtFS
{
    public static class PathUtils
    {
        /// <summary>
        /// 修复文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string FixFilePath(this string path)
        {
            path = path.Replace("\\", "/").Replace("//", "/");
            if (path == "/")
            {
                path = "";
            }
            else
            {
                if (path.StartsWith("/"))
                {
                    path = path.Remove(0, 1);
                }
                else if (path.StartsWith("~/"))
                {
                    path = path.Remove(0, 2);
                }
            }

            return path;
        }

        /// <summary>
        /// 修复文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string FixDirPath(this string path)
        {
            path = FixFilePath(path);
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }
            return path;
        }
    }
}