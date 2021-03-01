using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

using UnityEngine;

public class StringEncryption
{

    #region  方法一 C#中对字符串加密解密（对称算法）  
    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    /// <summary>  
    /// DES加密字符串  
    /// </summary>  
    /// <param name="encryptString">待加密的字符串</param>  
    /// <param name="encryptKey">加密密钥,要求为8位</param>  
    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>  
    public static string EncryptDES(string encryptString, string encryptKey = "HWZdkTNm")
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }

    /// <summary>  
    /// DES解密字符串  
    /// </summary>  
    /// <param name="decryptString">待解密的字符串</param>  
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>  
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>  
    public static string DecryptDES(string decryptString, string decryptKey = "HWZdkTNm")
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            Debug.Log("catch");
            return decryptString;
        }
    }
    #endregion






    #region MD5不可逆加密  
    //32位加密  
    public string GetMD5_32(string s, string _input_charset)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < t.Length; i++)
        {
            sb.Append(t[i].ToString("x").PadLeft(2, '0'));
        }
        return sb.ToString();
    }

    //16位加密   
    public static string GetMd5_16(string ConvertString)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
        t2 = t2.Replace("-", "");
        return t2;
    }
    #endregion
}
