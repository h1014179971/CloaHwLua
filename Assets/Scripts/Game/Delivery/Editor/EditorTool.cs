using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace Delivery
{
    public class EditorTool
    {
        [MenuItem("Tools/数据管理/删除玩家数据")]
        public static void DeletePlayerData()
        {
            string playerPath = Application.persistentDataPath + "/JsonData/";
            string playerFileName = Files.hashPlayer;
            playerPath = playerPath + playerFileName;
            if (File.Exists(playerPath))
                File.Delete(playerPath);
            else
                Debug.LogError($"玩家数据路径有错{playerPath}");
        }
        [MenuItem("Tools/数据管理/删除全部数据")]
        public static void DeleteAllPlayerData()
        {
            string playerPath = Application.persistentDataPath + "/JsonData";
            if (Directory.Exists(playerPath))
                Directory.Delete(playerPath,true);
        }
        [MenuItem("Tools/数据管理/打开玩家数据")]
        public static void OpenPlayerData()
        {
            string playerPath = Application.persistentDataPath + "/JsonData";
            playerPath = playerPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer.exe", playerPath);
        }
        [MenuItem("Tools/数据管理/解密玩家数据")]
        public static void OpenPlayer()
        {
            string playerPath = Application.persistentDataPath + "/JsonData/";
            string playerFileName = Files.hashPlayer;
            string keyStr = playerPath+playerFileName;
            string jsonStr = File.ReadAllText(keyStr);
            jsonStr = StringEncryption.DecryptDES(jsonStr); //////解密
            File.WriteAllText(playerPath + playerFileName + "_1", jsonStr);
            
        }
        [MenuItem("Tools/游戏设置/加速（5倍）")]
        public static void UpSpeed()
        {
            Time.timeScale = 5;
        }
        [MenuItem("Tools/游戏设置/正常速度")]
        public static void NormalSpeed()
        {
            Time.timeScale = 1;
        }
    }
}

