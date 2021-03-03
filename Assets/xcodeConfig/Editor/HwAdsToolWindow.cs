using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml;

public class HwAdsToolWindow : EditorWindow
{
    private static HwAdsToolWindow instance;
    private static Vector2 scrollPos;
    private static string dataPath;
    private static string hwAds_projectId;
    private static string hwads_AppToken;
    private static string hwads_ImportantToken;
    private static string hwads_UACToken;
    private static string hwads_MonetizationToken;
    private static string ga_GameKey;
    private static string ga_gameSecret;
    private static string ga_buildId;
    private static string dp_appID;
    private static string dp_appName;
    private static string dp_channel;

    private static bool isIos = true;

    private enum Dp_serviceVendorEnum
    {
        NONE,
        CN,
        VA,
        SG
    }
    private static Dp_serviceVendorEnum dp_serviceVendor;
    private static string bugly_appID;
    private static string setting_PROVISIONING;
    private static string plist_admobID;
    private static string plist_facebookID;
    private static string plist_facebookName;
    //↓↓↓↓↓↓↓↓↓↓Android↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
    private static string android_admobID;
    private static string android_facebookID;
    //↑↑↑↑↑↑↑↑↑↑Android↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
    [MenuItem("SDK数据/编写数据")]
    static void ShowWindow()
    {
        dataPath = Path.Combine(Application.streamingAssetsPath, "Ios/SdkData.json"); 
        LoadData();
        LoadPlist();
        LoadXml();
        instance = EditorWindow.GetWindow<HwAdsToolWindow>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        instance.Show();
    }
    static void CreateDirectory()
    {
        string directoryPath = Path.Combine(Application.streamingAssetsPath, "Ios");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        directoryPath = Path.Combine(Application.streamingAssetsPath, "Android");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }
    static void LoadData()
    {
        CreateDirectory();
        if (isIos)
            dataPath = Path.Combine(Application.streamingAssetsPath, "Ios/SdkData.json");
        else
            dataPath = Path.Combine(Application.streamingAssetsPath, "Android/SdkData.json");
        if (!File.Exists(dataPath))
        {
            File.Create(dataPath).Dispose(); 
            return;
        }
        StreamReader streamReader = new StreamReader(dataPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();
        JObject jo = null;
        if (String.IsNullOrEmpty(json))
            jo = new JObject();
        else
        {
            var jToken = JToken.Parse(json);
            jo = jToken as JObject;
        }  
        hwAds_projectId = jo.Value<int>("hwads_projectId").ToString(); 
        hwads_AppToken = jo.Value<string>("hwads_AppToken"); 
        hwads_ImportantToken = jo.Value<string>("hwads_ImportantToken");
        hwads_UACToken = jo.Value<string>("hwads_UACToken");
        hwads_MonetizationToken = jo.Value<string>("hwads_MonetizationToken");

        //GA
        ga_GameKey = jo.Value<string>("ga_GameKey");
        ga_gameSecret = jo.Value<string>("ga_gameSecret");
        ga_buildId = jo.Value<string>("ga_buildId");
        //DataPlayer
        dp_appID = jo.Value<string>("dp_appID");
        dp_appName = jo.Value<string>("dp_appName"); 
        dp_channel = jo.Value<string>("dp_channel");
        string dp_service = jo.Value<string>("dp_serviceVendor");
        if (String.IsNullOrEmpty(dp_service))
            dp_serviceVendor = Dp_serviceVendorEnum.NONE;
        else
            dp_serviceVendor = (Dp_serviceVendorEnum)Enum.Parse(typeof(Dp_serviceVendorEnum), dp_service);
        //bugly
        bugly_appID = jo.Value<string>("bugly_appID");
    }
    static void LoadPlist()
    {
        string configPath = "Assets/xcodeConfig/Editor/XCodeConfig.json";
        StreamReader streamReader = new StreamReader(configPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();
        var jToken = JToken.Parse(json);
        JObject jo = jToken as JObject;
        JObject plistJo = jo.Value<JObject>("plist");
        plist_admobID = plistJo.Value<string>("GADApplicationIdentifier");
        plist_facebookID = plistJo.Value<string>("FacebookAppID"); 
        plist_facebookName = plistJo.Value<string>("FacebookDisplayName");
        JObject propertiesJo = jo.Value<JObject>("properties");
        setting_PROVISIONING = propertiesJo["="]["PROVISIONING_PROFILE_SPECIFIER"].ToString();
    }
    static void LoadXml()
    {
        // 读取xml
        string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);
        if (xmlDoc == null) return;
        XmlNode node = xmlDoc.SelectSingleNode("/manifest");
        node = FindNode(xmlDoc,"/manifest/application/meta-data", "android:name", "com.facebook.sdk.ApplicationId");
        string facebookId = node.Attributes["android:value"].Value;
        android_facebookID = facebookId.Replace("fb", "");
        node = FindNode(xmlDoc, "/manifest/application/meta-data", "android:name", "com.google.android.gms.ads.APPLICATION_ID");
        android_admobID = node.Attributes["android:value"].Value;
    }
    static XmlNode FindNode(XmlDocument xmlDoc, string xpath, string attributeName, string attributeValue)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes(xpath);
        //Debug.Log(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);
            string _attributeValue = node.Attributes[attributeName].Value;
            if (_attributeValue == attributeValue)
            {
                return node;
            }
        }
        return null;
    } 
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        bool isToggle = isIos;
        isToggle = EditorGUILayout.Toggle("IOS", isToggle);
        isToggle = !EditorGUILayout.Toggle("Android", !isToggle);
        if (isToggle != isIos)
        {
            isIos = isToggle;
            LoadData();
        }
                            
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("---------------------------------------------------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("HwAds_projectId", GUILayout.Width(150));
        hwAds_projectId = EditorGUILayout.TextField("", hwAds_projectId);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("hwads_AppToken", GUILayout.Width(150));
        hwads_AppToken =  EditorGUILayout.TextField("",hwads_AppToken);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("hwads_ImportantToken", GUILayout.Width(150));
        hwads_ImportantToken =  EditorGUILayout.TextField("",hwads_ImportantToken);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("hwads_UACToken", GUILayout.Width(150));
        hwads_UACToken =  EditorGUILayout.TextField("",hwads_UACToken);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("hwads_MonetizationToken", GUILayout.Width(150));
        hwads_MonetizationToken =  EditorGUILayout.TextField("",hwads_MonetizationToken);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("-----------------------------------GA---------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ga_GameKey", GUILayout.Width(150));
        ga_GameKey = EditorGUILayout.TextField("", ga_GameKey);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ga_gameSecret", GUILayout.Width(150));
        ga_gameSecret = EditorGUILayout.TextField("", ga_gameSecret);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ga_buildId", GUILayout.Width(150));
        ga_buildId = EditorGUILayout.TextField("", ga_buildId);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("-----------------------------------DataPlayer---------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("dp_appID", GUILayout.Width(150));
        dp_appID = EditorGUILayout.TextField("", dp_appID);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("dp_appName", GUILayout.Width(150));
        dp_appName = EditorGUILayout.TextField("", dp_appName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("dp_channel", GUILayout.Width(150));
        dp_channel = EditorGUILayout.TextField("", dp_channel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("dp_serviceVendor", GUILayout.Width(150));
        dp_serviceVendor = (Dp_serviceVendorEnum)EditorGUILayout.EnumPopup(dp_serviceVendor);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("-----------------------------------bugly---------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("bugly_appID", GUILayout.Width(150));
        bugly_appID = EditorGUILayout.TextField("", bugly_appID);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存数据"))
        {
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (isIos)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("-----------------------------------plist---------------------------------------");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("plist_admobID", GUILayout.Width(150));
            plist_admobID = EditorGUILayout.TextField("", plist_admobID);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("plist_facebookID", GUILayout.Width(150));
            plist_facebookID = EditorGUILayout.TextField("", plist_facebookID);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("plist_facebookName", GUILayout.Width(150));
            plist_facebookName = EditorGUILayout.TextField("", plist_facebookName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("-----------------------------------Build Settings---------------------------------------");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("setting_PROVISIONING", GUILayout.Width(150));
            setting_PROVISIONING = EditorGUILayout.TextField("", setting_PROVISIONING);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存Plist数据"))
            {
                SavePlist();
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("-----------------------------------xml---------------------------------------");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("android_admobID", GUILayout.Width(150));
            android_admobID = EditorGUILayout.TextField("", android_admobID);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("android_facebookID", GUILayout.Width(150));
            android_facebookID = EditorGUILayout.TextField("", android_facebookID);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存xml数据"))
            {
                SaveXml();
            }
            GUILayout.EndHorizontal();
        }
        
        

    }
    static void SaveData()
    {
        using (StreamReader streamReader = new StreamReader(dataPath))
        {
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            JObject jo = null;
            if (String.IsNullOrEmpty(json))
                jo = new JObject();
            else
            {
                var jToken = JToken.Parse(json);
                jo = jToken as JObject;
            }
            if (!string.IsNullOrEmpty(hwAds_projectId))
            {
                hwAds_projectId = hwAds_projectId.Replace(" ", "");
                jo["hwads_projectId"] = int.Parse(hwAds_projectId);
            }
            if (!string.IsNullOrEmpty(hwads_AppToken))
            {
                hwads_AppToken = hwads_AppToken.Replace(" ", "");
                jo["hwads_AppToken"] = hwads_AppToken;
            }
            if (!string.IsNullOrEmpty(hwads_ImportantToken))
            {
                hwads_ImportantToken = hwads_ImportantToken.Replace(" ", "");
                jo["hwads_ImportantToken"] = hwads_ImportantToken;
            }
            if (!string.IsNullOrEmpty(hwads_UACToken))
            {
                hwads_UACToken = hwads_UACToken.Replace(" ", "");
                jo["hwads_UACToken"] = hwads_UACToken;
            }
            if (!string.IsNullOrEmpty(hwads_MonetizationToken))
            {
                hwads_MonetizationToken = hwads_MonetizationToken.Replace(" ", "");
                jo["hwads_MonetizationToken"] = hwads_MonetizationToken;
            }

            //GA
            if (!string.IsNullOrEmpty(ga_GameKey))
            {
                ga_GameKey = ga_GameKey.Replace(" ", "");
                jo["ga_GameKey"] = ga_GameKey;
            }
            if (!string.IsNullOrEmpty(ga_gameSecret))
            {
                ga_gameSecret = ga_gameSecret.Replace(" ", "");
                jo["ga_gameSecret"] = ga_gameSecret;
            }
            if (!string.IsNullOrEmpty(ga_buildId))
            {
                ga_buildId = ga_buildId.Replace(" ", "");
                jo["ga_buildId"] = ga_buildId;
            }
            //DataPlayer
            if (!string.IsNullOrEmpty(dp_appID))
            {
                dp_appID = dp_appID.Replace(" ", "");
                jo["dp_appID"] = dp_appID;
            }
            if (!string.IsNullOrEmpty(dp_appName))
            {

                jo["dp_appName"] = dp_appName;
            }
            if (!string.IsNullOrEmpty(dp_channel))
            {

                jo["dp_channel"] = dp_channel;
            }
            string _serviceVendor = dp_serviceVendor.ToString();
            if (!string.IsNullOrEmpty(_serviceVendor))
            {
                jo["dp_serviceVendor"] = _serviceVendor;
            }
            //bugly
            if (!string.IsNullOrEmpty(bugly_appID))
            {
                bugly_appID = bugly_appID.Replace(" ", "");
                jo["bugly_appID"] = bugly_appID;
            }

            string str = jo.ToString();
            StreamWriter streamWriter = new StreamWriter(dataPath);
            streamWriter.Write(str);
            streamWriter.Close();
            AssetDatabase.Refresh();
        }  
    }
    static void SavePlist()
    {
        string configPath = "Assets/xcodeConfig/Editor/XCodeConfig.json";
        StreamReader streamReader = new StreamReader(configPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();
        var jToken = JToken.Parse(json);
        JObject jo = jToken as JObject;
        JObject plistJo = jo.Value<JObject>("plist");
        if (!string.IsNullOrEmpty(plist_admobID))
        {
            plist_admobID = plist_admobID.Replace(" ", "");
            plistJo["GADApplicationIdentifier"] = plist_admobID;
        }
        if (!string.IsNullOrEmpty(plist_facebookID))
        {
            plist_facebookID = plist_facebookID.Replace(" ", "");
            plistJo["FacebookAppID"] = plist_facebookID;
            JArray cFBundleURLTypesJo = plistJo.Value<JArray>("CFBundleURLTypes");
            JArray cFBundleURLSchemesJa = cFBundleURLTypesJo[0].Value<JArray>("CFBundleURLSchemes");
            cFBundleURLSchemesJa[0] = "fb" + plist_facebookID;
        }
        if (!string.IsNullOrEmpty(plist_facebookName))
        {
            plistJo["FacebookDisplayName"] = plist_facebookName;
        }
        JObject propertiesJo = jo.Value<JObject>("properties");
        if (!string.IsNullOrEmpty(setting_PROVISIONING))
        {
            propertiesJo["="]["PROVISIONING_PROFILE_SPECIFIER"] = setting_PROVISIONING;
        }
        string str = jo.ToString();
        StreamWriter streamWriter = new StreamWriter(configPath);
        streamWriter.Write(str);
        streamWriter.Close();
        AssetDatabase.Refresh();
    }
    static void SaveXml()
    {
        string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);
        if (xmlDoc == null) return;
        XmlNode node = xmlDoc.SelectSingleNode("/manifest");
        node = FindNode(xmlDoc, "/manifest/application/meta-data", "android:name", "com.facebook.sdk.ApplicationId");

        string facebookId = android_facebookID;
        facebookId = "fb" + facebookId;
        node.Attributes["android:value"].Value = facebookId; 
        node = FindNode(xmlDoc, "/manifest/application/meta-data", "android:name", "com.google.android.gms.ads.APPLICATION_ID");
        node.Attributes["android:value"].Value  = android_admobID;
        node = FindNode(xmlDoc, "/manifest/application/provider", "android:name", "com.bytedance.sdk.openadsdk.multipro.TTMultiProvider");
        string authorities = PlayerSettings.applicationIdentifier + ".TTMultiProvider";  
        node.Attributes["android:authorities"].Value = authorities;
        xmlDoc.Save(xmlPath);
        AssetDatabase.Refresh();
    }
}
