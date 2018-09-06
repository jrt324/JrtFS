using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using JrtFS.Comm;


namespace JrtFS.Common
{
    public class FileConfig
    {
        private static string _rootPath;

        private static string RootPath
        {
            get
            {
                if (_rootPath.IsNull())
                {
                    _rootPath = ConfigurationManager.AppSettings["StoragePath"];
                    if (_rootPath.IsNull())
                    {
                        _rootPath = "~/Files";
                    }
                }
                return _rootPath;
            }
        }

        private static string absRootPath;
        public static string AbsRootPath
        {
            get
            {
                if (absRootPath == null)
                {
                    absRootPath = UrlUtils.ResolveServerUrl(RootPath);
                }
                return absRootPath;
            }
        }


        public static string MapPath(string bucket, string path)
        {
            if (bucket == null) bucket = "";
            var combinPath= RootPath.PathCombin(bucket).PathCombin(path);
            if (IsAbsolutePhysicalPath(combinPath))
                return combinPath;

            var fullPath = HttpContext.Current.Server.MapPath(combinPath);
            return fullPath;
        }


        #region 检查是否为绝对路径

        /// <summary>
        /// 检查是否为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAbsolutePhysicalPath(string path)
        {
            if (path == null || path.Length < 3)
                return false;

            // e.g c:\foo
            if (path[1] == ':' && IsDirectorySeparatorChar(path[2]))
                return true;

            // e.g \\server\share\foo or //server/share/foo
            return IsUncSharePath(path);
        }

        private static bool IsDirectorySeparatorChar(char ch)
        {
            return (ch == '\\' || ch == '/');
        }

        private static bool IsUncSharePath(string path)
        {
            // e.g \\server\share\foo or //server/share/foo
            if (path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]))
                return true;
            return false;

        } 
        #endregion
    }
}