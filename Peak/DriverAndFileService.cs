using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Peak
{
    public class DriverAndFileService
    {
        public const int OfReadwrite = 2;
        public const int OfShareDenyNone = 0x40;
        public static readonly IntPtr HfileError = new IntPtr(-1);

        public List<string> ErroFile=new List<string>();
        private readonly List<string> _fileList = new List<string>();
        public  List<string> AllFileList => _fileList;

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        ///     获得驱动器列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDriversList()
        {
            var list = new List<string>();
            var volumes = DriveInfo.GetDrives();

            foreach (var di in volumes)
                list.Add(di.Name);
            return list;
        }

        /// <summary>
        ///     列出文件夹下面的所有文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="filter">Filter为null列出所有文件，注意扩展名必须带.</param>
        /// <param name="worker"></param>
        /// <returns></returns>
        public List<string> GetPathFiles(FileSystemInfo info, string[] filter, BackgroundWorker worker)
        {
            try
            {
                var dir = info as DirectoryInfo;
                //不是目录 

                if (dir != null)
                {

                    var files = dir.GetFileSystemInfos();
                    foreach (FileSystemInfo t in files)
                    {
                        if ((t.Attributes & FileAttributes.Hidden) != 0 || t.FullName.ToLower().Contains("$RECYCLE.BIN".ToLower()) || t.FullName.ToLower().Contains("System Volume Information".ToLower()))
                            continue;
                        var file = t as FileInfo;
                        if (file != null)
                        {
                            if (filter != null)
                            {
                                foreach (string f in filter)
                                {
                                    if (file.Extension.ToLower().Equals(f.ToLower()))
                                    {
                                        _fileList.Add(file.FullName);
                                        worker?.ReportProgress(_fileList.Count);
                                    }
                                }
                            }
                            else
                            {
                                _fileList.Add(file.FullName);
                            }
                        }
                        //对于子目录，进行递归调用 
                        else
                        {
                            GetPathFiles(t, filter, worker);
                        }
                    }
                }
              
            }
            catch (Exception)
            {
                ErroFile.Add(info.FullName);
            }
            return _fileList;
        }
        /// <summary>
        ///     列出系统文件
        /// </summary>

        /// <param name="worker"></param>
        /// <returns></returns>
        public List<string> GetAllPathFiles(BackgroundWorker worker)
        {
            String[] drives = Environment.GetLogicalDrives();
            foreach (var item in drives)
            {
                GetPathFiles(new DirectoryInfo(item), null, null);
            }
          
            return _fileList;
        }
        public List<string> GetAllPathFiles()
        {
            GetAllPathFiles(null);
            return _fileList;
        }



        /// <summary>
        ///     列出文件夹下面的所有文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> GetPathFiles(FileSystemInfo info, string[] filter)
        {
            return GetPathFiles(info, filter, null);
        }


        /// <summary>
        ///     检查文件是否处于编辑状态
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool IsUsing(string fileName)
        {
            var vFileName = fileName;
            var vHandle = _lopen(vFileName, OfReadwrite | OfShareDenyNone);
            if (vHandle == HfileError)
                return true;
            CloseHandle(vHandle);
            return false;
        }
        /// <summary>
        /// 公用检索
        /// </summary>
        /// <param name="enumList"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static List<string> GetNtfsFiles(IEnumerable<string> enumList, string[] filters)
        {
            var ntfsList = new List<string>();
            foreach (var s in enumList)
            foreach (var filter in filters)
                if (s.ToLower().EndsWith(filter))
                    if (File.Exists(s) && s.Contains("$RECYCLE.BIN") == false &&
                        File.GetAttributes(s) != FileAttributes.Hidden)
                        ntfsList.Add(s);
            return ntfsList;
        }
    }
}