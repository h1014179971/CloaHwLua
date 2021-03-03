using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using CYUtils;
using System.Text.RegularExpressions;

public class StoryCheckLangTools
{
    private static string storyPath = "XNRes/ToolConfig/config/data/story/";

    private static string guidePath = "XNRes/ToolConfig/config/data/guide/";
    private static string exportStroyPath = System.IO.Path.Combine(Application.dataPath,"stroyLang.txt");

    private static string guideExportPath = System.IO.Path.Combine(Application.dataPath,"guideLang.txt");

    static private void checkLang(string cPath,string sPath)
    {
        string path = System.IO.Path.Combine(Application.dataPath,cPath);
        List<string> files = new List<string>();
        FileUtils.searchAllFiles(path, files, new List<string> { ".lua" });
        Dictionary<string,string> oldMap = new Dictionary<string,string>();
        for (int i = 0; i < files.Count; i++)
        {
            string context = FileUtils.LoadFile(files[i]);
            // Debug.LogError("-----文件名："+ files[i]);
            //匹配双引号中的
            foreach(Match match in new Regex("\"([^\"]*)\"", RegexOptions.Singleline).Matches(context))
            {
                string matchStr = match.ToString();
                if(Regex.IsMatch(matchStr,"[\u4e00-\u9fa5]+"))
                {
                    if(!oldMap.ContainsKey(matchStr))
                    {
                        oldMap.Add(match.ToString(),"\"\"");
                        // Debug.LogError("---"+match.ToString());
                    }
                }
            }
            foreach(Match match in new Regex("\'([^\']*)\'", RegexOptions.Singleline).Matches(context))
            {
                string matchStr = match.ToString();
                if(Regex.IsMatch(matchStr,"[\u4e00-\u9fa5]+"))
                {
                    if(!oldMap.ContainsKey(matchStr))
                    {
                        oldMap.Add(match.ToString(),"\'\'");
                        // Debug.LogError("---"+match.ToString());
                    }
                }
            }
        }
        writeText(sPath,oldMap);
    }

    static private void writeText(string path, Dictionary<string,string> oldMap)
    {
        StreamWriter f = new StreamWriter(path, false);
        foreach(var dic in oldMap)
        {
            string content = String.Format("{0}={1};",dic.Key,dic.Value);
            // Debug.LogError("writeText:"+content);
            f.WriteLine(content);
        }
        f.Close();
    }


    [MenuItem("Assets/剧情文字替换/导出剧情文字", false, 4012)]
    static private void checkStoryLang()
    {
        checkLang(storyPath,exportStroyPath);
        Debug.LogError("导出剧情文字完成");
    }

    [MenuItem("Assets/剧情文字替换/导出引导文字", false, 4014)]
    static private void checkGuideLang()
    {
        checkLang(guidePath,guideExportPath);
        Debug.LogError("导出引导文字完成");
    }

    private static void ImportLang(string exportPath,string cPath)
    {
        Dictionary<string,string> newMap = getNewLangMap(exportPath);
        string path = System.IO.Path.Combine(Application.dataPath,cPath);
        List<string> files = new List<string>();
        FileUtils.searchAllFiles(path, files, new List<string> { ".lua" });
        for (int i = 0; i < files.Count; i++)
        {
            string context = FileUtils.LoadFile(files[i]);
            // Debug.LogError("-----文件名："+ files[i]);
            foreach(Match match in new Regex("\"([^\"]*)\"", RegexOptions.Singleline).Matches(context))
            {
                string matchStr = match.ToString();
                if(Regex.IsMatch(matchStr,"[\u4e00-\u9fa5]+"))
                {
                    if(newMap.ContainsKey(matchStr))
                    {
                        //存在就替换
                        // Debug.LogError("替换："+matchStr+ "|为："+newMap[matchStr]);
                        context = context.Replace(matchStr,newMap[matchStr]);
                    }
                }
            }
            foreach(Match match in new Regex("\'([^\']*)\'", RegexOptions.Singleline).Matches(context))
            {
                string matchStr = match.ToString();
                if(Regex.IsMatch(matchStr,"[\u4e00-\u9fa5]+"))
                {
                    if(newMap.ContainsKey(matchStr))
                    {
                        // Debug.LogError("替换："+matchStr+ "|为："+newMap[matchStr]);
                        //存在就替换
                        context = context.Replace(matchStr,newMap[matchStr]);
                    }
                }
            }
            FileUtils.SaveFile(files[i],context);
        }
    }


    [MenuItem("Assets/剧情文字替换/导入剧情文字",false,4013)]
    public static void ImportStroyLang()
    {
        ImportLang(exportStroyPath,storyPath);
        Debug.LogError("导入剧情文字成功...");
    }
    
    [MenuItem("Assets/剧情文字替换/导入引导文字",false,4015)]
    public static void ImportGuideLang()
    {
        ImportLang(guideExportPath,guidePath);
        Debug.LogError("导入引导文字成功...");
    }

    private static Dictionary<string,string> getNewLangMap(string exportPath)
    {
        Dictionary<string,string> langMap = new Dictionary<string,string>();
        StreamReader sr = new StreamReader(exportPath, Encoding.UTF8);
        string line;
        string lastLine = "";
        while((line = sr.ReadLine())!= null)
        {
            if(lastLine.EndsWith(";"))
            {
                splitStr(lastLine, ref langMap);
                lastLine = "";
            }
            else
            {
                if (string.IsNullOrEmpty(lastLine))
                {
                    lastLine = line;
                }
                else
                {
                    lastLine = lastLine + @"\n" + line;
                }
                if (lastLine.EndsWith(";"))
                {
                    splitStr(lastLine, ref langMap);
                    lastLine = "";
                }
            }
        }
        return langMap;
    }

    private static void splitStr(string line,ref Dictionary<string, string> dict)
    {
        line = line.Replace(";","");
        string[] lst = line.Split('=');
        string key = lst[0];
        string value = lst[1];
        if(string.IsNullOrEmpty(key) == false && string.IsNullOrEmpty(value) == false)
        {
            if(value != "\"\"" && value != "\'\'" && !dict.ContainsKey(key))
            {
                dict.Add(key,value);
            }
        }
    }
}
