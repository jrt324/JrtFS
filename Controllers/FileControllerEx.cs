using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using JrtFS.Comm;
using JrtFS.Common;
using JrtFS.Filters;
using JrtFS.Models;

using Newtonsoft.Json;

namespace JrtFS.Controllers
{
    public partial class FileController
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("exists")]
        public HttpResponseMessage Exists(string path)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);
            var ext = Path.GetExtension(fullPath);
            var exist = false;
            if (!ext.IsNull())
            {
                exist = File.Exists(fullPath);
            }
            else
            {
                exist = Directory.Exists(fullPath);
            }
            return this.ApiResult(ApiStatusCodes.Success, exist.ConvertTo<int>());
        }

        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("getObject")]
        public HttpResponseMessage GetObject(string path)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);

            FsEntity fs = new FsEntity();
            if (File.Exists(fullPath))
            {
                var info = new FileInfo(fullPath);
                fs.FullPath = path.FixFilePath();
                fs.Length = info.Length;
                fs.Name = info.Name;
                fs.ObjectType = ObjectType.File;
                fs.LastUpdateTime = File.GetLastWriteTime(fullPath);
            }
            else if (Directory.Exists(fullPath))
            {
                var info = new DirectoryInfo(fullPath);
                fs.FullPath = fullPath.FixDirPath();
                fs.Name = info.Name;
                fs.ObjectType = ObjectType.Dir;
                fs.LastUpdateTime = Directory.GetLastWriteTime(fullPath);
            }
            return this.ApiResult(ApiStatusCodes.Success, fs);
        }

        /// <summary>
        /// 列举目录下的文件和子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listObjectType">列举子项的类型：文件=1，文件夹=2，两者都包含=3</param>
        /// <param name="start">开始</param>
        /// <param name="pageSize">每页包含多少项</param>
        /// <returns></returns>
        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("listChildren")]
        public HttpResponseMessage ListChildren(string path, ObjectType listObjectType)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);
            var dirPath = fullPath;
            if (!Directory.Exists(fullPath))
                dirPath = Path.GetDirectoryName(fullPath);

            IEnumerable<FsEntity> list = new List<FsEntity>();

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists)
            {
                if ((listObjectType & ObjectType.Dir) == ObjectType.Dir)
                {
                    list = dir.EnumerateDirectories()
                        .Select(f => new FsEntity
                        {
                            ObjectType = ObjectType.Dir,
                            FullPath = path.UrlCombine(f.Name).FixDirPath(),
                            Length = 0,
                            Name = f.Name,
                            LastUpdateTime = File.GetLastWriteTime(f.FullName)
                        });
                }

                if ((listObjectType & ObjectType.File) == ObjectType.File)
                {
                    var list1 = dir.EnumerateFiles()
                        .Select(f => new FsEntity
                        {
                            ObjectType = ObjectType.File,
                            FullPath = path.UrlCombine(f.Name).FixFilePath(),
                            Length = f.Length,
                            Name = f.Name,
                            LastUpdateTime = Directory.GetLastWriteTime(f.FullName)
                        });
                    list = list.Union(list1);
                }
            }
            return this.ApiResult(ApiStatusCodes.Success, list);
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("createFolder")]
        public HttpResponseMessage CreateFolder(string path)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);

            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                if (Directory.Exists(fullPath))
                {
                    FsEntity fs = new FsEntity();
                    var info = new DirectoryInfo(fullPath);
                    fs.FullPath = path.UrlCombine(info.Name);
                    fs.Name = info.Name;
                    fs.ObjectType = ObjectType.Dir;
                    fs.LastUpdateTime = Directory.GetLastWriteTime(fullPath);
                    return this.ApiResult(ApiStatusCodes.Success, fs);
                }
                return this.ApiResult(ApiStatusCodes.Success);
            }
            catch (Exception e)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, e.Message);
            }
        }

        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("createFile")]
        public HttpResponseMessage CreateFile(string path)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);

            try
            {
                if (!File.Exists(fullPath))
                {
                    var dir = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    using (var stream = File.Create(fullPath))
                    {
                        stream.Write(new byte[] { }, 0, 0);
                    }
                }

                if (File.Exists(fullPath))
                {
                    FsEntity fs = new FsEntity();
                    var info = new FileInfo(fullPath);
                    fs.FullPath = path.UrlCombine(info.Name);
                    fs.Length = info.Length;
                    fs.Name = info.Name;
                    fs.ObjectType = ObjectType.File;
                    fs.LastUpdateTime = File.GetLastWriteTime(fullPath);
                    return this.ApiResult(ApiStatusCodes.Success, fs);
                }
                return this.ApiResult(ApiStatusCodes.Success);
            }
            catch (Exception e)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, e.Message);
            }
        }

        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("delete")]
        public HttpResponseMessage Delete(string path)
        {
            var bucket = this.GetBucket();
            var fullPath = FileConfig.MapPath(bucket, path);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                else if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                }
                return this.ApiResult(ApiStatusCodes.Success);
            }
            catch (Exception e)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, e.Message);
            }
        }


        [Route("rename")]
        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        public HttpResponseMessage Rename(string oldPath, string newPath)
        {
            if (oldPath.Equals(newPath, StringComparison.CurrentCultureIgnoreCase))
            {
                return this.ApiResult(ApiStatusCodes.Success);
            }
            var bucket = this.GetBucket();
            var oldFullPath = FileConfig.MapPath(bucket, oldPath);
            var newFullPath = FileConfig.MapPath(bucket, newPath);
            try
            {
                if (File.Exists(oldFullPath))
                {
                    var oldDir = Path.GetDirectoryName(oldFullPath);
                    var newDir = Path.GetDirectoryName(newFullPath);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    File.Move(oldFullPath, newFullPath);
                }
                else if (Directory.Exists(oldFullPath))
                {
                    //if (!Directory.Exists(newFullPath))
                    //{
                    //    Directory.CreateDirectory(newFullPath);
                    //}
                    Directory.Move(oldFullPath, newFullPath);
                }
                return this.ApiResult(ApiStatusCodes.Success);
            }
            catch (Exception e)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, e.Message);
            }
        }

        [HttpPost, AuthorizedAccess]
        [MultiParameterSupport]
        [Route("copyFile")]
        public HttpResponseMessage CopyFile(string oldPath, string newPath)
        {
            var bucket = this.GetBucket();
            var oldFullPath = FileConfig.MapPath(bucket, oldPath);
            var newFullPath = FileConfig.MapPath(bucket, newPath);
            try
            {
                if (File.Exists(oldFullPath))
                {
                    var oldDir = Path.GetDirectoryName(oldFullPath);
                    var newDir = Path.GetDirectoryName(newFullPath);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    File.Copy(oldFullPath, newFullPath);
                }
                return this.ApiResult(ApiStatusCodes.Success);
            }
            catch (Exception e)
            {
                return this.ApiResult(ApiStatusCodes.NormalError, e.Message);
            }
        }




    }



}