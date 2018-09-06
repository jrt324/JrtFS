using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrtFS.Controllers;

namespace JrtFS.Models
{
    public class FsEntity
    {
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// 获取表示完整路径的字符串。
        /// </summary>
        public string FullPath { get; set; }


        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// 获取或设置确定当前文件是否为只读的值。
        /// </summary>
        //public bool IsReadOnly { get; set; }

        /// <summary>
        /// 获取当前文件的大小（字节）。
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 文件名。
        /// </summary>
        public string Name { get; set; }
    }

    [Flags]
    public enum ObjectType
    {
        File = 1,
        Dir = 2,
    }
}