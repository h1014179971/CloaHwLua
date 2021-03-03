using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;
using System.Reflection;

public class Utils
{
    #region XML
    /// <summary>
    /// Converts the xml to string.
    /// </summary>
    /// <returns>The xml to string.</returns>
    /// <param name="xmlDoc">Xml document.</param>
    public static string ConvertXmlToString(XmlDocument xmlDoc)
    {
        MemoryStream stream = new MemoryStream();
        XmlTextWriter writer = new XmlTextWriter(stream, null);
        writer.Formatting = Formatting.Indented;
        xmlDoc.Save(writer);
        StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
        stream.Position = 0;
        string xmlString = sr.ReadToEnd();
        sr.Close();
        stream.Close();
        return xmlString;
    }

    /// <summary>
    /// Froms the string to xml document.
    /// </summary>
    /// <returns>The string to xml document.</returns>
    /// <param name="bs">Bs.</param>
    public static XmlDocument ConvertByteArrayToXmlDoc(byte[] bs)
    {
        XmlDocument doc = new XmlDocument();
        string text = UTF8ByteArrayToString(bs);
        doc.LoadXml(text);
        return doc;
    }

    /// <summary>
    /// Strings to UT f8 byte array.
    /// </summary>
    /// <returns>The to UT f8 byte array.</returns>
    /// <param name="pXmlString">P xml string.</param>
    public static byte[] StringToUTF8ByteArray(string _string)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(_string);
        return byteArray;
    }

    /// <summary>
    /// UTs the f8 byte array to string.
    /// </summary>
    /// <returns>The f8 byte array to string.</returns>
    /// <param name="characters">Characters.</param>
    public static string UTF8ByteArrayToString(byte[] _bytes)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(_bytes);
        return (constructedString);
    }



    /// <summary>
    /// Gets the local xml document.
    /// </summary>
    /// <returns>The local xml document.</returns>
    /// <param name="xmlfile">Xmlfile.</param>
    public static XmlDocument GetLocalXmlDoc(string xmlfile)
    {
        string url;
#if REMOTE
		url = Application.persistentDataPath + xmlfile;
		byte[] bs = File.ReadAllBytes(url);
		XmlDocument doc = ConvertByteArrayToXmlDoc(bs);	
#else
        XmlDocument doc = new XmlDocument();
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            url = Application.dataPath + "/StreamingAssets" + xmlfile;
            doc.Load(url);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            url = "jar:file://" + Application.dataPath + "!/assets" + xmlfile;
            WWW www = new WWW(url);
            while (!www.isDone) { }
            try
            {
                doc.LoadXml(www.text);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                //error_msg += "LOAD " + xmlfile + " ERROR!" + "\n";
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = Application.dataPath + "/Raw" + xmlfile;
            doc.Load(url);
        }
#endif
        return doc;
    }
    #endregion

    #region Event
    public static void RemoveEvent<T>(T c, string name)
    {
        Delegate[] invokeList = GetObjectEventList(c, name);
        if (invokeList == null)
            return;
        foreach (Delegate del in invokeList)
        {
            typeof(T).GetEvent(name).RemoveEventHandler(c, del);
        }
    }

    public static Delegate[] GetObjectEventList(object p_Object, string p_EventName)
    {
        // Get event field  
        FieldInfo _Field = p_Object.GetType().GetField(p_EventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        if (_Field == null)
        {
            return null;
        }
        // get the value of event field which should be a delegate  
        object _FieldValue = _Field.GetValue(p_Object);

        // if it is a delegate  
        if (_FieldValue != null && _FieldValue is Delegate)
        {
            // cast the value to a delegate  
            Delegate _ObjectDelegate = (Delegate)_FieldValue;
            // get the invocation list  
            return _ObjectDelegate.GetInvocationList();
        }
        return null;
    }
    #endregion

    #region Path

    public static string GetDefaultFilePath()
    {
        return Application.persistentDataPath + "/";
    }

    public static string GetStreamingAssetsPath()
    {
        string path;
        //读取StreamingAssets下的文件
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.streamingAssetsPath + "/";
        }
        else
        {
            path = "file://" + Application.streamingAssetsPath + "/";
        }
        return path;
    }

    //Is file exist in default path
    public static bool IsFileExistInDefaultPath(string name)
    {
        return IsFileExistByPath(GetDefaultFilePath() + name); ;
    }

    //is file exist in custom path and name
    public static bool IsFileExistByPath(string path)
    {
        FileInfo info = new FileInfo(path);
        if (info == null || info.Exists == false)
        {
            return false;
        };
        return true;
    }

    public static bool IsDirectoryExist(string fullpath)
    {
        return Directory.Exists(fullpath);
    }

    public static void CreateDir(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static string[] GetAllFileFullNameInDir(string dirPath, string searchPatter  = "*.*")
    {
        try
        {
            return Directory.GetFiles(dirPath, searchPatter);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    #endregion

    #region Sprite

    public static Sprite LoadSprite(string spriteName)
    {
        string path = "SpritesRuntime/" + spriteName;
        GameObject go = Resources.Load<GameObject>(path);
        Sprite s = go.GetComponent<SpriteRenderer>().sprite;
        return s;
    }

    #endregion
    
    #region Color

    /// <summary>
    /// Color32 to int(0xARGB).
    /// </summary>
    /// <returns>The to int.</returns>
    /// <param name="c">C.</param>
    public static int Color32ToInt(Color32 c)
    {
        int retVal = 0;
        retVal |= c.a << 24;
        retVal |= c.r << 16;
        retVal |= c.g << 8;
        retVal |= c.b;
        return retVal;
    }
    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }
    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        if (hex.Length <= 6)
            hex += "FF";
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }
    #endregion
    public static bool IsInteger(string str)
    {
        string pattern = @"^[0-9]*[1-9][0-9]*$"; //正整数
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
        return regex.IsMatch(str);
    }
    /// <summary>
    /// 判断两直线是否相交
    /// </summary>
    /// <param name="result">交点</param>
    /// <param name="p1">线段1上的点</param>
    /// <param name="v1">向量1</param>
    /// <param name="p2">线段2上的点</param>
    /// <param name="v2">向量2</param>
    /// <returns></returns>
    public static bool IsLineCross(out Vector3 result, Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2)
    {
        result = Vector3.zero;
        float dot = Vector3.Dot(v1.normalized, v2.normalized);
        if (dot == 1 || dot == -1)
        {
            // 两线平行
            return false;
        }
        Vector3 startPointSeg = p2 - p1;
        Vector3 vecS1 = Vector3.Cross(v1, v2);            // 有向面积1
        Vector3 vecS2 = Vector3.Cross(startPointSeg, v2); // 有向面积2
        float num = Vector3.Dot(startPointSeg, vecS1);
        // 判断两这直线是否共面
        if (num >= 1E-05f || num <= -1E-05f)
        {
            return false;
        }
        // 有向面积比值，利用点乘是因为结果可能是正数或者负数
        if (vecS1 == Vector3.zero) return false;
        float num2 = Vector3.Dot(vecS2, vecS1) / vecS1.sqrMagnitude;
        result = p1 + v1 * num2;
        return true;
    }
}
































//
//#if REMOTE
//Debug.Log("LOAD XML:" + xmlfile);
//string url = Application.persistentDataPath + xmlfile;
//byte[] bs = File.ReadAllBytes(url);
//XmlDocument doc = fromStringToXmlDoc(bs);		
//#else
//XmlDocument doc = new XmlDocument();
//if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
//	doc.Load(Application.dataPath +  "/StreamingAssets" + xmlfile);
//else
//{
//	#if UNITY_ANDROID
//	WWW www = new WWW("jar:file://" + Application.dataPath + "!/assets" + xmlfile);
//	while (!www.isDone) {}
//	try
//	{
//		/*
//				System.IO.StringReader stringReader = new System.IO.StringReader(www.text);
//				stringReader.Read(); // skip BOM
//				System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stringReader);
//				*/
//		doc.LoadXml(www.text);
//		//doc.LoadXml(stringReader.ReadToEnd());
//	}
//	catch (Exception ex)
//	{
//		//error_msg += "LOAD " + xmlfile + " ERROR!" + "\n";
//	}
//	#elif UNITY_IPHONE
//	doc.Load(Application.dataPath + "/Raw" + xmlfile);
//	#endif
//}
//#endif	

