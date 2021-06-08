using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Excel;
using System.Data;
using System.IO;
using Newtonsoft.Json;  
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System;   

public class ExcelUtility
{

    /// <summary>
    /// 表格数据集合
    /// </summary>
    private DataSet mResultSet;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="excelFile">Excel file.</param>
    public ExcelUtility(string excelFile)
    {
        if (File.Exists(excelFile))
        {
            FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
            IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
            mResultSet = mExcelReader.AsDataSet();
        }
        
    }

    /// <summary>
    /// 转换为实体类列表
    /// </summary>
    public List<T> ConvertToList<T>()
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return null;
        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return null;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //准备一个列表以保存全部数据
        List<T> list = new List<T>();

        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //创建实例
            Type t = typeof(T);
            ConstructorInfo ct = t.GetConstructor(System.Type.EmptyTypes);
            T target = (T)ct.Invoke(null);
            for (int j = 0; j < colCount; j++)
            {
                //读取第1行数据作为表头字段
                string field = mSheet.Rows[0][j].ToString();
                object value = mSheet.Rows[i][j];
                //设置属性值
                SetTargetProperty(target, field, value);
            }

            //添加至列表
            list.Add(target);
        }

        return list;
    }
    public string GetAssetPath(string name)
    {
        string Platform = "";
#if UNITY_IOS
			Platform="iOS";
#elif UNITY_ANDROID
        Platform = "Android";
#elif UNITY_WEBPLAYER
            Platform ="WebPlayer";
#elif UNITY_WP8
			Platform="WP8Player";
#elif UNITY_METRO
            Platform = "MetroPlayer";
#elif UNITY_OSX || UNITY_STANDALONE_OSX
		Platform = "StandaloneOSXIntel";
#else
        Platform = "StandaloneWindows";
#endif
        string path = Path.Combine(Platform, name);  //System.String.Format("{0}/{1}", Platform, name);
        //string path = Platform + "/" + name;  
        return path;
    }
    /// <summary>
    /// 转换为Json
    /// </summary>
    /// <param name="JsonPath">Json文件路径</param>
    /// <param name="Header">表头行数</param>
    public void ConvertToJson(string JsonPath, Encoding encoding, string name)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //准备一个列表存储整个表的数据
        //Dictionary<string, List<Dictionary<string, object>>> dic = new Dictionary<string, List<Dictionary<string, object>>>();
        List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

        //string excelName = mSheet.Rows[1][0].ToString();
        //dic[excelName] = table;
        //读取数据
        for (int i = 2; i < rowCount; i++)
        {
            //准备一个字典存储每一行的数据
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int j = 0; j < colCount; j++)
            {
                //读取第2行数据作为表头字段
                string field = mSheet.Rows[1][j].ToString();
                if (String.IsNullOrEmpty(field)) continue;
                //Key-Value对应
                row[field] = mSheet.Rows[i][j];
            }

            //添加到表数据中
            table.Add(row);
        }

        //生成Json字符串
        string json = JsonConvert.SerializeObject(table, Newtonsoft.Json.Formatting.Indented);
        //json = StringEncryption.EncryptDES(json);   //加密
        //写入文件
        //string newJsonPath = Application.streamingAssetsPath + "/" + GetAssetPath(excelName + ".json");

        using (FileStream fileStream = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
            {
                textWriter.Write(json);
            }
        }
    }

    public void ConvertToJson(string AssetPath, Encoding encoding)
    {
        string fileName = "test";
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;
        for(int ln = 0; ln < mResultSet.Tables.Count;ln++ )
        {
            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[ln];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                continue;
            if (!mSheet.Rows[0][0].ToString().Equals("类名"))
                continue;
            fileName = mSheet.Rows[1][0].ToString();
            if (String.IsNullOrEmpty(fileName)) continue;
            fileName += ".json";
            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            //Dictionary<string, List<Dictionary<string, object>>> dic = new Dictionary<string, List<Dictionary<string, object>>>();
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //string excelName = mSheet.Rows[1][0].ToString();
            //dic[excelName] = table;
            //读取数据
            for (int i = 2; i < rowCount; i++)
            {
                bool isNext = false;
                for (int m = 0; m < colCount; m++)
                {
                    string objStr = mSheet.Rows[i][m].ToString();
                    if (!string.IsNullOrEmpty(objStr) && objStr != "null")
                    {
                        isNext = true;
                        break;
                    }
                }
                if (!isNext) continue;
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 1; j < colCount; j++)
                {
                    //读取第2行数据作为表头字段
                    string field = mSheet.Rows[1][j].ToString();
                    if (String.IsNullOrEmpty(field)) continue;
                    //Key-Value对应
                    row[field] = mSheet.Rows[i][j];
                }

                //添加到表数据中
                    table.Add(row);
            }

            //生成Json字符串
            string json = JsonConvert.SerializeObject(table, Newtonsoft.Json.Formatting.Indented);
            //json = StringEncryption.EncryptDES(json);   //加密
            //写入文件
            //string newJsonPath = Application.streamingAssetsPath + "/" + GetAssetPath(excelName + ".json");
            string JsonPath = Path.Combine(AssetPath, fileName);
            using (FileStream fileStream = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(json);
                }
            }
        }


        
    }

    /// <summary>
	/// 转换为lua
	/// </summary>
	/// <param name="luaPath">lua文件路径</param>
	public void ConvertToLua(string luaPath, Encoding encoding ,ref List<string> fileNames)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;
        
        

        //读取数据表
        foreach (DataTable mSheet in mResultSet.Tables)
        {
            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                continue;
            if (!mSheet.Rows[0][0].ToString().Equals("类名"))
                continue;
            string fileName = "test";
            fileName = mSheet.Rows[1][0].ToString();
            if (String.IsNullOrEmpty(fileName)) continue;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("local "+fileName+" = {");
            stringBuilder.Append("\r\n");
            
            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 2; i < rowCount; i++)
            {
                bool isNext = false;
                for (int m = 0; m < colCount; m++)
                {
                    string objStr = mSheet.Rows[i][m].ToString();
                    if (!string.IsNullOrEmpty(objStr) && objStr != "null")
                    {
                        isNext = true;
                        break;
                    }
                }
                if (!isNext) continue;
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 1; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[1][j].ToString();
                    //Key-Value对应
                    row[field] = mSheet.Rows[i][j];
                }
                //添加到表数据中
                table.Add(row);
            }
            //stringBuilder.Append(string.Format("\t\"{0}\" = ", mSheet.TableName));
            //stringBuilder.Append("{\r\n");
            foreach (Dictionary<string, object> dic in table)
            {
                stringBuilder.Append("\t\t{\r\n");
                foreach (string key in dic.Keys)
                {
                    if (string.IsNullOrEmpty(key)) continue;
                    object valueObj = dic[key];
                    Type type = valueObj.GetType();
                    if (valueObj.GetType().Name == "String")
                    {
                        string valueStr = valueObj as string;
                        string str = valueStr.Replace("\"", "\\\"");
                        if (!string.IsNullOrEmpty(valueStr))
                            stringBuilder.Append(string.Format("\t\t\t{0} = \"{1}\",\r\n", key, str));
                        else
                            stringBuilder.Append(string.Format("\t\t\t{0} = nil,\r\n", key));
                    }  
                    else if (valueObj == null || valueObj.GetType().Name == "DBNull")
                        stringBuilder.Append(string.Format("\t\t\t{0} = nil,\r\n", key));
                    else
                        stringBuilder.Append(string.Format("\t\t\t{0} = {1},\r\n", key, valueObj));
                }
                stringBuilder.Append("\t\t},\r\n");
            }
            stringBuilder.Append("}\r\n");
            stringBuilder.Append("_G."+fileName+" = "+ fileName);
            stringBuilder.Append("\r\n");
            stringBuilder.Append("setmetatable(_G," + fileName + ")");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("return " + fileName);
            fileNames.Add(fileName);
            fileName += ".lua";
            string writePath = Path.Combine(luaPath, fileName);
            //写入文件
            using (FileStream fileStream = new FileStream(writePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }

        
    }


    /// <summary>
    /// 转换为CSV
    /// </summary>
    public void ConvertToCSV(string CSVPath, Encoding encoding)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();

        //读取数据
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                //使用","分割每一个数值
                stringBuilder.Append(mSheet.Rows[i][j] + ",");
            }
            //使用换行符分割每一行
            stringBuilder.Append("\r\n");
        }
        
        //写入文件
        using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }

    /// <summary>
    /// 导出为Xml
    /// </summary>
    public void ConvertToXml(string XmlFile)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();
        //创建Xml文件头
        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        stringBuilder.Append("\r\n");
        //创建根节点
        stringBuilder.Append("<Table>");
        stringBuilder.Append("\r\n");
        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //创建子节点
            stringBuilder.Append("  <Row>");
            stringBuilder.Append("\r\n");
            for (int j = 0; j < colCount; j++)
            {
                stringBuilder.Append("   <" + mSheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append(mSheet.Rows[i][j].ToString());
                stringBuilder.Append("</" + mSheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append("\r\n");
            }
            //使用换行符分割每一行
            stringBuilder.Append("  </Row>");
            stringBuilder.Append("\r\n");
        }
        //闭合标签
        stringBuilder.Append("</Table>");
        //写入文件
        using (FileStream fileStream = new FileStream(XmlFile, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }

    /// <summary>
    /// 设置目标实例的属性
    /// </summary>
    private void SetTargetProperty(object target, string propertyName, object propertyValue)
    {
        //获取类型
        Type mType = target.GetType();
        //获取属性集合
        PropertyInfo[] mPropertys = mType.GetProperties();
        foreach (PropertyInfo property in mPropertys)
        {
            if (property.Name == propertyName)
            {
                property.SetValue(target, Convert.ChangeType(propertyValue, property.PropertyType), null);
            }
        }
    }
}

