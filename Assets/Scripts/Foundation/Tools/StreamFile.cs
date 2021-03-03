using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

namespace Foundation
{
    public class StreamFile
    {
        public static string PersistentLocalPath()
        {
            return Application.persistentDataPath + "/";
        }
        public static string StreamingLocalPath()
        {
            return Application.streamingAssetsPath + "/";
        }
        public static string GetFilePath(string name)
        {
            string path = "";
            path = PersistentLocalPath() + name;
            bool currPersistentExist = File.Exists(path);
            if (!currPersistentExist)
            {
                path = StreamingLocalPath() + name;
            }
            if (path.IndexOf("://") == -1)
            {
#if UNITY_EDITOR_WIN
                path = "file:///" + path;
#else
                path = "file://" + path;
#endif
            }
            return path;
        }

        public static void ReaderFile(string fileName, MonoBehaviour mono, Action<string> callback)
        {
            string file_path = GetFilePath(fileName);
            mono.StartCoroutine(WWWReaderFile(file_path, callback));
        }
        static IEnumerator WWWReaderFile(string file_path, Action<string> callback)
        {                                           
            UnityWebRequest request = UnityWebRequest.Get(file_path);
            yield return request.SendWebRequest();
            if(request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(request.error);
                callback(null);
            }
            else
            {
                string str = request.downloadHandler.text; 
                str = StringReplace(str);
                //str = StringEncryption.DecryptDES(str);  //解密  
                callback(str);
            }
        }
        public static string StringReplace(string str)
        {
            //str = str.Replace(" ", "");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            str = str.Replace("\t", "");
            str = str.Replace("\v", "");
            //str = str.Replace("\"", "");
            return str;
        }
        public static string Combine(string path,string fileName)
        {
            return Path.Combine(path, fileName);
        }
        public static string RecordFilePath(string name)
        {
            string path = PersistentLocalPath() + name;
            return path;
        }
        public static void RecordFile(object jsonObj, string name)
        {
            name = name.Replace('\\','/');
            string[] directoryPaths = name.Split('/');
            string directory = "";
            for(int i =0;i< directoryPaths.Length - 1; i++)
            {
                directory += directoryPaths[i];
                if (i != directoryPaths.Length - 2)
                    directory += "/";
            }
            directory = RecordFilePath(directory);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string jsonStr = jsonObj.ToString();
            jsonStr = StringReplace(jsonStr);
            jsonStr = StringEncryption.EncryptDES(jsonStr); //加密
            string path = RecordFilePath(name);
            FileInfo fs = new FileInfo(path);
            fs.Delete();
            StreamWriter sw;
            if (!fs.Exists)
            {
                sw = fs.CreateText();
            }
            else
            {
                sw = fs.AppendText();
            }
            sw.Write(jsonStr);
            sw.Close();
            sw.Dispose();//文件流释放
        }
    }
}


