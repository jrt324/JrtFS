using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JrtFS.Common
{
    public class ApiSecertUtils
    {
        /// <summary>
        /// 验证访问授权是否合法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secert"></param>
        public static bool Vaild(string key, string secert)
        {
            var ret= AccessKeys.Any(a => a.AccessKeyId == key && a.AccessKeySecret == secert);
            return ret;
        }

        private static List<FileServerSecertKey> accessKeys;
        public static List<FileServerSecertKey> AccessKeys
        {
            get
            {
                if (accessKeys == null)
                {
                    accessKeys = new List<FileServerSecertKey>();
                    var configStr = ConfigurationManager.AppSettings["SecertKeys"];
                    if (configStr.IsNull())
                    {
                        return null;
                    }
                    var pars = configStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (pars.Length > 0)
                    {
                       
                        foreach (var strPar in pars)
                        {
                            var par = strPar.Split(',');
                            if (par.Length > 1)
                            {
                                var secertKey=new FileServerSecertKey();
                                secertKey.AccessKeyId = par[0];
                                secertKey.AccessKeySecret = par[1];
                                accessKeys.Add(secertKey);
                            }
                        }
                    }

                }
                return accessKeys;
            }
        }
    }

    public class FileServerSecertKey
    {
        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }
    }
}