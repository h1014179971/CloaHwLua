using System.IO;
using System.Text;

namespace AutoCode
{
    public static  class AutoSystemIOExtension
    {
        public static StringBuilder AddPrefix(this StringBuilder self, string prefixString)
        {
            self.Insert(0, prefixString);
            return self;
        }
        public static string GetFolderPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }
        public static string CreateDirIfNotExists(this string dirFullPath)
        {
            if (!Directory.Exists(dirFullPath))
            {
                Directory.CreateDirectory(dirFullPath);
            }

            return dirFullPath;
        }
    }
}

