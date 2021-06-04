using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelTool
{
    [MenuItem("Tools/读取配置文件")]
    public static void CreateAndRead()
    {
        string assetPath = Path.Combine(Application.dataPath, "Res/JsonData");
        Debug.Log($"Application.dataPath==={Application.dataPath}");
        if (Directory.Exists(assetPath))
            Directory.Delete(assetPath, true);
        Directory.CreateDirectory(assetPath);  
        string path = Application.dataPath;
        path = path.Replace("Assets", "GameData/");
        List<string> excels = new List<string>();
        var dirInfo = new DirectoryInfo(path);
        foreach (FileInfo preaInfoFile in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
        {                                
            if (preaInfoFile.Name.EndsWith(".xlsx"))
            {
                //构造Excel工具类
                ExcelUtility excel = new ExcelUtility(preaInfoFile.ToString());
                //Encoding encoding = Encoding.GetEncoding("gb2312");
                Encoding encoding = new UTF8Encoding();
                excel.ConvertToJson(assetPath, encoding);
                excels.Add(preaInfoFile.Name.Split('.')[0]);
            }
                
        }
        Debug.Log("读取配置文件成功");
        AssetDatabase.Refresh();
    }
    [MenuItem("Tools/读取配置文件Lua")]
    public static void CreateLuaAndRead()
    {
        string assetPath = Path.Combine(Application.dataPath, "Lua/LuaData");
        Debug.Log($"Application.dataPath==={Application.dataPath}");
        if (Directory.Exists(assetPath))
            Directory.Delete(assetPath, true);
        Directory.CreateDirectory(assetPath);
        string path = Application.dataPath;
        path = path.Replace("Assets", "GameData/");
        List<string> excels = new List<string>();
        var dirInfo = new DirectoryInfo(path);
        List<string> luafileNames = new List<string>();
        foreach (FileInfo preaInfoFile in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (preaInfoFile.Name.EndsWith(".xlsx"))
            {
                //构造Excel工具类
                ExcelUtility excel = new ExcelUtility(preaInfoFile.ToString());
                //Encoding encoding = Encoding.GetEncoding("gb2312");
                Encoding encoding = new UTF8Encoding();
                excel.ConvertToLua(assetPath, encoding,ref luafileNames);
                excels.Add(preaInfoFile.Name.Split('.')[0]);
            }

        }
        CreateLuaData(assetPath, luafileNames);
        Debug.Log("读取配置文件成功");
        AssetDatabase.Refresh();
    }
    private static void CreateLuaData(string luaPath, List<string> luafileNames)
    {
        string luaData = "LuaData.lua";
        string writePath = Path.Combine(luaPath, luaData);
        StringBuilder stringBuilder = new StringBuilder();
        for(int i = 0; i < luafileNames.Count; i++)
        {
            string fileName = luafileNames[i];
            stringBuilder.Append("require(\"LuaData."+ fileName+"\")");
            stringBuilder.Append("\r\n");
        }
        
        //写入文件
        using (FileStream fileStream = new FileStream(writePath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, new UTF8Encoding()))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }
}
