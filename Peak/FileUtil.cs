using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Peak
{

    /// <summary>
    /// 提供文件安全操作
    /// </summary>
    public static class FileUtil
    {
        private static DriverAndFileService _driverAndFileService;


        #region 文件增加删除移动检索
        /// <summary>
        /// 删除指定路径文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void Delete(string filePath)
        {
            

        }
        /// <summary>
        /// 搜索指定文件
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>

        public static List<string> Search(string pattern)
        {
            if (_driverAndFileService == null)
            {
                _driverAndFileService=new DriverAndFileService();
                _driverAndFileService.GetAllPathFiles();
            }
            return _driverAndFileService.AllFileList.Where(i=>i.Contains(pattern)).ToList();
        }



        #endregion

        #region  方法
        private static bool IsSystemHidden(DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent == null)
            {
                return false;
            }
            string attributes = dirInfo.Attributes.ToString();
            if (attributes.IndexOf("Hidden", StringComparison.Ordinal) > -1 && attributes.IndexOf("System", StringComparison.Ordinal) > -1)
            {
                return true;
            }
            return false;
        }
        #endregion


    }
}
