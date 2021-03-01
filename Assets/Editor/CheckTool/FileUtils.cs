

namespace CYUtils
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    public class FileUtils
    {
        public static string GetFullPath(string root, string absPath)
        {
            return Path.Combine(root, absPath).Replace("\\", "/");
        }
        public static String LoadFile(String fileName)
        {
            if (File.Exists(fileName))
                using (StreamReader sr = File.OpenText(fileName))
                    return sr.ReadToEnd();
            else
                return String.Empty;
        }

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 检查某文件夹路径是否存在，如不存在，创建
        /// </summary>
        /// <param name="path"></param>
        static public void CheckDirection(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        static  public bool DeleteFiles(string path)
        {
            if (Directory.Exists(path) == false)
            {
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.FullName.Contains("$") && (!item.FullName.Contains("Boot")))
                        {
                            DeleteFiles(item.FullName);
                        }
                    }
                }
                Directory.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static public void ClearDirection(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                di.Delete(true);
            }
            CheckDirection(path);
        }

        /// <summary>
        /// 获取指定路径下第一层的子文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public List<string> GetSubFolders(string path)
        {
            if (!IsDirectoryExists(path))
            {
                return null;
            }
            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dirs = root.GetDirectories();
            List<string> folders = new List<string>();
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    folders.Add(dirs[i].FullName);
                }
            }

            return folders;

        }

        /// <summary>
        /// 单纯检查某个文件夹路径是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool IsDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取目录下的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetDirectoryFiles(string path, string exName)
        {
            if (!IsDirectoryExists(path))
            {
                return null;
            }
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++)
            {
                ex = GetExName(files[i].FullName);
                if (ex == exName)
                {
                    continue;
                }
                names.Add(files[i].FullName);
            }
            return names;
        }

		/// <summary>
		/// 获取目录下的文件
		/// </summary>
		/// <param name="path"></param>
		/// <param name="exName"></param>
		/// <returns></returns>
		static public List<string> GetDirectoryFilesByEx(string path, string ex)
		{
			if (!IsDirectoryExists(path))
			{
				return null;
			}
			List<string> names = new List<string>();
			DirectoryInfo root = new DirectoryInfo(path);
			FileInfo[] files = root.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				if (GetExName(files[i].FullName) == ex)
				{
					names.Add(files[i].FullName);
				}
			}
			return names;
		}

        /// <summary>
        /// 获取某目录下所有除了指定类型外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetAllFilesExcept(string path, string exName)
        {
            if (!IsDirectoryExists(path))
            {
                return null;
            }

            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++)
            {
                ex = GetExName(files[i].FullName);
                if (ex == exName)
                {
                    continue;
                }
                names.Add(files[i].FullName);
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    List<string> subNames = GetAllFilesExcept(dirs[i].FullName, exName);
                    if (subNames.Count > 0)
                    {
                        for (int j = 0; j < subNames.Count; j++)
                        {
                            names.Add(subNames[j]);
                        }
                    }
                }
            }

            return names;

        }
        /// <summary>
        /// 获得扩展名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string GetExName(string str)
        {
            string rexStr = @"(?<=\\[^\\]+.)[^\\.]+$|(?<=/[^/]+.)[^/.]+$";
            return GetFirstMatch(str, rexStr);
        }
        /// <summary>
        /// 获取第一个匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public string GetFirstMatch(string str, string regexStr)
        {
            Match m = Regex.Match(str, regexStr);
            if (!string.IsNullOrEmpty(m.ToString()))
            {
                return m.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string GetFileName(string str)
        {
            string rexStr = @"(?<=\\)[^\\]+$|(?<=/)[^/]+$";
            return GetFirstMatch(str, rexStr);
        }
        /// <summary>
        /// 去除扩展名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string RemoveExName(string str)
        {
            string returnStr = str;
            string rexStr = @"[^\.]+(?=\.)";
            string xStr = GetFirstMatch(str, rexStr);
            if (!string.IsNullOrEmpty(xStr))
            {
                returnStr = xStr;
            }
            return returnStr;

        }
        static public void RecursionCreateFolder(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            path = path.Replace("\\", "/");
            //这个是防止有一个相同路径名的文件存在。导致创建失败
            string tmpPath = path;
            if (path.EndsWith("/"))
            {
                int index = path.LastIndexOf("/");
                tmpPath = tmpPath.Substring(0, index);
            }
            if (IsFileExists(tmpPath))
            {
                DelFile(tmpPath);
            }
            Directory.CreateDirectory(path);//这个方法在指定路径中创建所有目录和子目录，除非它们已经存在。
        }

        static public void DelFile(string filePath)
        {
            if (IsFileExists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// 获取指定路径下一层的除指定格式以外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        public static List<string> GetSubFilesExcept(string path, string exName)
        {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++)
            {
                ex = GetExName(files[i].FullName);
                if (ex == exName)
                {
                    continue;
                }
                names.Add(files[i].FullName);
            }

            return names;
        }
        static public void CheckFileSavePath(string path)
        {
            string realPath = path.Replace("\\", "/");
            int ind = realPath.LastIndexOf("/");
            if (ind >= 0)
            {
                realPath = realPath.Substring(0, ind);
            }
            RecursionCreateFolder(realPath);
        }
        static public void SaveFile(string path, string content)
        {
            if (!IsFileExists(path))
            {
                CheckFileSavePath(path);
            }
            StreamWriter f = new StreamWriter(path, false);
            f.WriteLine(content);
            f.Close();
        }

        static public void SaveFile(string path, byte[] content)
        {
            if (!IsFileExists(path))
            {
                CheckFileSavePath(path);
            }
            StreamWriter f = new StreamWriter(path, false);
            //f.WriteLine(content);
            f.Write(content);
            f.Close();
        }

        static public void SaveFileByByte(string path, byte[] content)
        {
            if (!IsFileExists(path))
            {
                CheckFileSavePath(path);
            }
            FileStream newFs = new FileStream(path, FileMode.Create, FileAccess.Write);
            newFs.Write(content, 0, content.Length);
            newFs.Close();
            newFs.Dispose();
        }

        /// <summary>
        /// 将List中的数据存到文件中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        static public void ListSaveFile(string path, List<string> list)
        {
            int count = list.Count;
            if (!IsFileExists(path) && count > 0)
            {
                StreamWriter sw = new StreamWriter(File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                for (int i = 0; i < count; i++)
                {
                    sw.WriteLine(list[i]);
                }
                sw.Flush();
                sw.Close();
            }
        }

        public static byte[] LoadByteFile(String fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllBytes(fileName);
            else
                return null;
        }
        public static string GetDirectoryName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }
        static public void CreateAssetFolder(string absFolder)
        {
            string path = UnityEngine.Application.dataPath;
            int index = path.LastIndexOf("/");
            if (index != -1)
            {
                path = path.Substring(0, index + 1);
            }
            path = Path.Combine(path, absFolder);
            ClearDirection(path);
        }

        /// <summary>
        /// 搜索所有包含的的文件
        /// </summary>
        /// <param name="srcPath">原路径</param>
        /// <param name="fileList">文件列表</param>
        /// <param name="include">包含的后缀名</param>
        static public void searchAllFiles(string srcPath,List<string> fileList, List<string> include)
        {
            string[] subPaths = Directory.GetDirectories(srcPath);//得到所有子目录
            foreach (string path in subPaths)
            {
                searchAllFiles(path, fileList,include);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(srcPath);
            foreach (string file in files)
            {
                //过滤一些不需要打包进入zip包的文件
                if (include != null)
                {
                    bool isInclude = false;
                    for (int i = 0; i < include.Count; i++)
                    {
                        if (file.ToLower().EndsWith(include[i].ToLower()))
                        {
                            isInclude = true;
                            break;
                        }
                    }
                    if (!isInclude)
                    {
                        continue;
                    }
                }
                fileList.Add(file.Replace("\\", "/"));//将当前目录中的所有文件全名存入文件List
            }
        }

        /// <summary>
        /// 获取文件下的所有文件 和空的文件夹
        /// </summary>
        /// <param name="srcPath">路径</param>
        /// <param name="fileList">文件lst</param>
        /// <param name="pathList">空目录lst</param>
        /// <param name="filter">过滤后缀名</param>
        static public void getAllFiles(string srcPath, List<string> fileList, List<string> pathList, List<string> filter)
        {
            string[] subPaths = Directory.GetDirectories(srcPath);//得到所有子目录
            foreach (string path in subPaths)
            {
                getAllFiles(path, fileList, pathList, filter);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(srcPath);
            foreach (string file in files)
            {
                //过滤一些不需要打包进入zip包的文件
                if (filter != null)
                {
                    bool isNotInclude = false;
                    for (int i = 0; i < filter.Count; i++)
                    {
                        if (file.EndsWith(filter[i]))
                        {
                            isNotInclude = true;
                            break;
                        }
                    }
                    if (isNotInclude)
                    {
                        continue;
                    }
                }
                fileList.Add(file.Replace("\\", "/"));//将当前目录中的所有文件全名存入文件List
            }
            if (pathList != null)
            {
                if (subPaths.Length == files.Length && files.Length == 0)//如果是空目录
                {
                    pathList.Add(srcPath.Replace("\\", "/"));//记录空目录
                }
            }
        }



		public static void CopyFolder(string from, string to)
		{
			if (!Directory.Exists(to))
				Directory.CreateDirectory(to);
			
			// 子文件夹
			foreach (string sub in Directory.GetDirectories(from))
				CopyFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");
			
			// 文件
			foreach (string file in Directory.GetFiles(from))
				File.Copy(file, to + Path.GetFileName(file), true);
		}
	
		public static void CutFolder(string from, string to)
		{
			if (!Directory.Exists(to))
				Directory.CreateDirectory(to);
			
			// 子文件夹
			foreach (string sub in Directory.GetDirectories(from))
				CutFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");
			
			// 文件
			foreach (string file in Directory.GetFiles(from)){
				File.Copy(file, to + Path.GetFileName(file), true);
				File.Delete(file);
			}
		}


        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="srcPath">原路径</param>
        /// <param name="dstPath">目标路径</param>
        /// <param name="filter">过滤的文件后缀</param>
        static public void CopyFiles(string srcPath, string dstPath, List<string> filter = null)
        {
            srcPath = getFilePath(srcPath);
            dstPath = getFilePath(dstPath);
            List<string> fileList = new List<string>();
            List<string> pathList = new List<string>();
            getAllFiles(srcPath, fileList, pathList, filter);
            foreach (string ps in pathList)
            {
                string tmp = ps.Replace(srcPath, dstPath);
                RecursionCreateFolder(tmp);
            }

            foreach (string fs in fileList)
            {
                string tmp = fs.Replace(srcPath, dstPath);
                CheckFilePath(tmp);
                File.Copy(fs, tmp, true);
            }
        }
        static public void CopyFilesToLower(string srcPath, string dstPath, List<string> filter = null)
        {
            srcPath = getFilePath(srcPath);
            dstPath = getFilePath(dstPath);
            List<string> fileList = new List<string>();
            List<string> pathList = new List<string>();
            getAllFiles(srcPath, fileList, pathList, filter);
            foreach (string ps in pathList)
            {
                string absPath = ps.Replace(srcPath,"").ToLower();
                string tmp = string.Format("{0}{1}",dstPath, absPath);
                RecursionCreateFolder(tmp);
            }

            foreach (string fs in fileList)
            {
                string absPath = fs.Replace(srcPath, "").ToLower();
                string tmp = string.Format("{0}{1}", dstPath, absPath);
                CheckFilePath(tmp);
                File.Copy(fs, tmp, true);
            }
        }
        static public string getFilePath(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            string tmpPath = filePath;
            int index = tmpPath.LastIndexOf("/");
            if (index != -1)
            {
                tmpPath = tmpPath.Substring(0, index);
            }
            return tmpPath;
        }
        /// <summary>
        /// 检查文件目录,没有此目录递归创建此文件目录
        /// </summary>
        /// <param name="filePath"></param>
        static public void CheckFilePath(string filePath)
        {
            string tmpPath = getFilePath(filePath);
            RecursionCreateFolder(tmpPath);
        }
    }
}
