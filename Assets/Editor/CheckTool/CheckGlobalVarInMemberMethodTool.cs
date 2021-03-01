using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CYUtils;
using UnityEditor;
using System.Text.RegularExpressions;

public class CheckGlobalVarInMemberMethodTool
{
    [MenuItem("Assets/查找Lua脚本成员方法中声明的全局变量", false, 4013)]
    public static void FindGlobalVarInMemberFunc()
    {
        string[] allLineStrArray;

        int memberFuncCount = 0;                                                       //大于0，表示进入成员方法中
        int braceCount = 0;                                                                   //大于0，表示进入花括号中

        List<string> localVarFilter = new List<string>();                      //外部局部变量过滤器
        List<string> funcLocalVarFilter = new List<string>();              //成员方法局部变量过滤器
        List<string> formParaFilter = new List<string>();                    //形参过滤器
        List<string> forCyclicParaFilter = new List<string>();              //迭代器变量过滤器

        List<string> fileList = new List<string>();
        string luaScrSubPath = "client/scripts";
        string uipath = System.IO.Path.Combine(Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")), luaScrSubPath);
        FileUtils.searchAllFiles(uipath, fileList, new List<string> { ".lua" });

        if (fileList != null && fileList.Count != 0)
        {
            for (int j = 0; j < fileList.Count; j++)
            {
                allLineStrArray = ReadFile(fileList[j]);
                memberFuncCount = 0;
                braceCount = 0;
                localVarFilter = new List<string>();
                if (allLineStrArray != null && allLineStrArray.Length != 0)
                {
                    for (int i = 0; i < allLineStrArray.Length; i++)
                    {
                        bool isFunc = allLineStrArray[i].IndexOf("function")==0;                        //等于0，则进入一个成员方法
                        bool isLocalFunc = allLineStrArray[i].IndexOf("local function")==0;       //等于0，则进入一个局部方法
                        bool isEnd = allLineStrArray[i].IndexOf("end")==0;                                 //等于0，则一个成员方法或局部方法结束

                        if (allLineStrArray[i].IndexOf("{") != -1)//在一个花括号中
                        {
                            braceCount++;
                        }
                        if (allLineStrArray[i].IndexOf("}") != -1)//离开一个花括号
                        {
                            braceCount--;
                        }

                        if (isFunc || isLocalFunc) //在成员方法或局部方法中
                        {
                            if (allLineStrArray[i].IndexOf("(") != -1)                                                          //查找形参，添加到形参过滤器
                            {
                                string formParaStr = 
                                    allLineStrArray[i].Substring(allLineStrArray[i].IndexOf("(") + 1, allLineStrArray[i].IndexOf(")") - allLineStrArray[i].IndexOf("(")-1);
                                if (string.IsNullOrEmpty(formParaStr)==false&& formParaStr != "...")     //有形参
                                {
                                    formParaStr = formParaStr.Replace(" ", "");
                                    if (formParaStr.IndexOf(",") != -1)                                                          //1.有多个形参
                                    {
                                        string[] parasArray = formParaStr.Split(',');
                                        if (parasArray != null && parasArray.Length > 0)
                                        {
                                            foreach (string temp in parasArray)
                                            {
                                                formParaFilter.Add(temp);
                                            }
                                        }
                                    }
                                    else                                                                                                        //2.只有一个形参
                                    {
                                        formParaFilter.Add(formParaStr);
                                    }
                                }
                            }
                            memberFuncCount++;
                            continue;
                        }
                        else if (isEnd)//离开一个成员方法或局部方法
                        {
                            memberFuncCount--;
                            funcLocalVarFilter = new List<string>();
                            formParaFilter = new List<string>();
                            forCyclicParaFilter = new List<string>();
                            continue;
                        }

                        if (memberFuncCount == 0)//在函数外
                        {
                            string tempStr = allLineStrArray[i].Trim();
                            if (tempStr.IndexOf("local") == 0)                                                               //查找局部变量，添加到过滤器
                            {
                                string str1 = tempStr.Substring(6);
                                string str2 = null;
                                if (str1.IndexOf(" ") != -1)                                                                        //1.针对写法:  local var = a
                                {
                                    str2 = str1.Substring(0, str1.IndexOf(" "));
                                }
                                else if (str1.IndexOf("=") != -1)                                                               //2.针对写法:  local var=a
                                {
                                    str2 = str1.Substring(0, str1.IndexOf("="));
                                }
                                else if (Regex.Match(str1.Replace("_",""), @"^[A-Za-z]+$").Success)     //3.针对写法:  local _var
                                {
                                    str2 = str1;
                                }
                                localVarFilter.Add(str2);
                                continue;
                            }
                        }
                        else if(memberFuncCount > 0 && braceCount==0)//在函数内且不在花括号中
                        {
                            string tempStr = allLineStrArray[i].Trim();
                            if (tempStr.IndexOf("local ") == 0)                                                                                    //查找局部变量，添加到过滤器中
                            {
                                string str1 = tempStr.Substring(6);
                                string str2 = null;
                                string[] strArray = null;
                                if (str1.IndexOf("=") != -1&&str1.Substring(0, str1.IndexOf("=")).IndexOf(",")!=-1)     //1.针对写法: local var1, var2 = a, b
                                {
                                    strArray = str1.Substring(0, str1.IndexOf("=")).Trim().Replace(" ", "").Split(',');
                                    if(strArray!=null&& strArray.Length > 0)
                                    {
                                        for (int x = 0; x < strArray.Length; x++) 
                                        {
                                            funcLocalVarFilter.Add(strArray[x]);
                                        }
                                    }
                                }
                                else if(str1.IndexOf(" ") != -1)                                                                                         //2.针对写法: local var = a
                                {
                                    str2 = str1.Substring(0, str1.IndexOf(" "));
                                }
                                else if (str1.IndexOf("=") != -1)                                                                                       //3.针对写法: local var=a
                                {
                                    str2 = str1.Substring(0, str1.IndexOf("="));
                                }
                                else if (Regex.Match(str1.Replace("_", ""), @"^[A-Za-z]+$").Success)                            //4.针对写法: local _var
                                {
                                    str2 = str1;
                                }
                                if (string.IsNullOrEmpty(str2) == false)
                                {
                                    funcLocalVarFilter.Add(str2);
                                }
                                continue;
                            }
                            else if (tempStr.IndexOf("for ") == 0)                                                                                  //在迭代器，添加变量名到过滤器中
                            {
                                if (tempStr.IndexOf("=") != -1)                                                                                         //1.针对写法: for i = 0, length do
                                {
                                    continue;
                                }
                                else                                                                                                                                   //2.针对写法: for k, v in pairs(table) do 
                                {
                                    string forCyclicParaStr =
                                         tempStr.Substring(tempStr.IndexOf(" ") + 1, tempStr.IndexOf("in") - tempStr.IndexOf(" ") - 1);
                                    if (string.IsNullOrEmpty(forCyclicParaStr) == false)
                                    {
                                        forCyclicParaStr = forCyclicParaStr.Replace(" ", "");
                                        if (forCyclicParaStr.IndexOf(",") != -1)
                                        {
                                            string[] parasArray = forCyclicParaStr.Split(',');
                                            if (parasArray != null && parasArray.Length > 0)
                                            {
                                                foreach (string temp in parasArray)
                                                {
                                                    forCyclicParaFilter.Add(temp);
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                            if (tempStr.IndexOf("if ") == 0 || tempStr.IndexOf("else") == 0 || tempStr.IndexOf("(") == 0 || tempStr.IndexOf("Logger") == 0 ||
                                tempStr.IndexOf("print") == 0 || tempStr.IndexOf("--") == 0 || tempStr.IndexOf("sub_print_r") == 0 || tempStr.IndexOf("return") == 0 ||
                                tempStr.IndexOf("break") == 0 || tempStr.IndexOf("self") == 0 || tempStr.IndexOf("table.") == 0 || tempStr.IndexOf("not ") == 0 ||
                                tempStr.IndexOf("while ") == 0 || tempStr.IndexOf("require") == 0 || tempStr.IndexOf("or ") == 0 || tempStr.IndexOf("and ") == 0 ||
                                tempStr.IndexOf("end") == 0 || tempStr.IndexOf("elseif ") == 0)//过滤关键字
                            {
                                continue;
                            }
                            else if (string.IsNullOrEmpty(tempStr) == false && tempStr[0] != ' ')
                            {
                                if (tempStr.IndexOf(" ") != -1)
                                {
                                    if (funcLocalVarFilter.Contains(tempStr.Substring(0, tempStr.IndexOf(" "))) == false &&
                                        localVarFilter.Contains(tempStr.Substring(0, tempStr.IndexOf(" "))) == false &&
                                        formParaFilter.Contains(tempStr.Substring(0, tempStr.IndexOf(" "))) == false && 
                                        forCyclicParaFilter.Contains(tempStr.Substring(0, tempStr.IndexOf(" "))) == false &&
                                        Regex.Match(tempStr.Substring(0, tempStr.IndexOf(" ")).Replace("_", ""), @"^[A-Za-z]+$").Success)
                                    {
                                        Debug.LogError("lua文件路径:" + fileList[j], AssetDatabase.LoadAssetAtPath((fileList[j]), typeof(Object)));
                                        Debug.LogError("请注意查看变量：" + tempStr.Substring(0, tempStr.IndexOf(" ")) + "             行数为：" + (i + 1));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static string[] ReadFile(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            string[] strArray = File.ReadAllLines(fileName);
            if (strArray.Length != 0)
            {
                return strArray;
            }
        }
        return null;
    }

}
