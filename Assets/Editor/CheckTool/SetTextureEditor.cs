using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CYUtils;
using System.IO;

public enum AndroidOpaqueTextureType
{
    RGB_Compressed_ETC_4_bits = 0,
    RGB_Compressed_ETC2_4_bits = 1,
}

public enum IphoneOpaqueTextureType
{
    RGB_Compressed_PVRTC_2_bits = 0,
    RGB_Compressed_PVRTC_4_bits = 1,
}

public enum AndroidTransparentTextureType
{
    RGBA_Compressed_ETC2_8_bits = 0
}

public enum IphoneTransparentTextureType
{
    RGBA_Compressed_PVRTC_2_bits = 0,
    RGBA_Compressed_PVRTC_4_bits = 1
}


public class SetTextureEditor : EditorWindow
{
    //类型
    public AndroidOpaqueTextureType androidOpaqueType = AndroidOpaqueTextureType.RGB_Compressed_ETC_4_bits;
    public IphoneOpaqueTextureType iphoneOpaqueType = IphoneOpaqueTextureType.RGB_Compressed_PVRTC_4_bits;
    public AndroidTransparentTextureType androidTransparentType = AndroidTransparentTextureType.RGBA_Compressed_ETC2_8_bits;
    public IphoneTransparentTextureType iphoneTransparentType = IphoneTransparentTextureType.RGBA_Compressed_PVRTC_4_bits;
    public string[] maxSizeDisplayeds = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };
    public int[] maxSizeValues = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
    public bool needDetailOption = false;
    public bool mipMaps = false;

    //平台
    //-----------------------------------------------------------------------------------------------
    private static string androidPlatform = "Android";  //平台
    private static string iphonePlatform = "iPhone";  //平台
    private static string standalonePlatform = "Standalone";  //平台
    private static int texMaxSize = 1024; //maxSize
    private static TextureImporterFormat androidOpaqueImporter = TextureImporterFormat.RGB24;    //Android不透明
    private static TextureImporterFormat iPhoneOpaqueImporter = TextureImporterFormat.RGB24;    //iPhone不透明
    private static TextureImporterFormat androidTransparentImporter = TextureImporterFormat.RGB24;    //Android透明
    private static TextureImporterFormat iPhoneTransparentImporter = TextureImporterFormat.RGB24;    //iPhone透明

    private static List<string> fileList; //图片路径集合
    private static SetTextureEditor myWindow;
    private static string texturePath;
    private static bool isUIAtlas = false;
    private static bool changeDetailOption = false;
    private static bool generateMipMaps = false;
    private static bool isForceSetMaxSize = false;
    private static string[] strs;

    [MenuItem("Assets/图片检查工具/修改选中目录下图片的压缩格式", false, 4010)]
    public static void InitTextureEditor()
    {
        RefreshSelectedData();
        myWindow = EditorWindow.GetWindow<SetTextureEditor>();      //显示Gui界面
    }

    public static void RefreshSelectedData()
    {
        fileList = new List<string>();
        texturePath = string.Empty;
        strs = Selection.assetGUIDs;
        if (strs == null || strs.Length == 0)
        {
            return;
        }
        string[] guids = strs;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (string.IsNullOrEmpty(path) == false && path.StartsWith("Atlas"))
        {
            isUIAtlas = true;
        }
        else
        {
            isUIAtlas = false;
        }
        Texture selectRes = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
        if (selectRes != null)
        {
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            texMaxSize = GetNextPowOf2(Mathf.Max(texture.height, texture.width));
            texturePath = path;
        }
        else
        {
            texMaxSize = 1024;
            FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", ".psd", ".jpeg", ".tga" });
        }
    }

    public static void ShowSelectedInfo()
    {
        if (string.IsNullOrEmpty(texturePath) == false)
        {
            EditorGUILayout.LabelField("选择的图片路径为: " + texturePath);
        }
    }

    private void OnGUI()
    {
        ShowSelectedInfo();
        if (isUIAtlas == false)
        {
            androidOpaqueType = (AndroidOpaqueTextureType)EditorGUILayout.EnumPopup("Android(不透明)", androidOpaqueType);        //Android 不透明UI选项
            iphoneOpaqueType = (IphoneOpaqueTextureType)EditorGUILayout.EnumPopup("iPhone(不透明)", iphoneOpaqueType);      //iPhone 不透明UI选项
        }
        else
        {
            EditorGUILayout.LabelField("选中UI图集文件夹，压缩类型默认包含a通道");
        }
        androidTransparentType = (AndroidTransparentTextureType)EditorGUILayout.EnumPopup("Android(透明)", androidTransparentType);        //Android 透明UI选项
        iphoneTransparentType = (IphoneTransparentTextureType)EditorGUILayout.EnumPopup("iPhone(透明)", iphoneTransparentType);      //iPhone 透明UI选项
        needDetailOption = EditorGUILayout.Toggle("是否需要修改MipMap设置", needDetailOption);
        if (needDetailOption)
        {
            mipMaps = EditorGUILayout.Toggle("Generate Mip Maps", mipMaps);
        }
        isForceSetMaxSize = EditorGUILayout.Toggle("是否强制设置MaxSize", isForceSetMaxSize);
        if (isForceSetMaxSize)
        {
            texMaxSize = EditorGUILayout.IntPopup("MaxSize", texMaxSize, maxSizeDisplayeds, maxSizeValues);      //maxSize
        }
        else
        {
            EditorGUILayout.LabelField("不勾选强制设置规则：1. UI图集设置成1024  2. 非UI图集设置成最接近2次幂");
        }
        if (GUILayout.Button("修改"))
        {
            generateMipMaps = mipMaps;
            changeDetailOption = needDetailOption;
            //不透明压缩类型
            switch (iphoneOpaqueType)
            {
                case IphoneOpaqueTextureType.RGB_Compressed_PVRTC_2_bits:
                    iPhoneOpaqueImporter = TextureImporterFormat.PVRTC_RGB2;
                    break;
                case IphoneOpaqueTextureType.RGB_Compressed_PVRTC_4_bits:
                    iPhoneOpaqueImporter = TextureImporterFormat.PVRTC_RGB4;
                    break;
            }
            switch (androidOpaqueType)
            {
                case AndroidOpaqueTextureType.RGB_Compressed_ETC_4_bits:
                    androidOpaqueImporter = TextureImporterFormat.ETC_RGB4;
                    break;
                case AndroidOpaqueTextureType.RGB_Compressed_ETC2_4_bits:
                    androidOpaqueImporter = TextureImporterFormat.ETC2_RGB4;
                    break;
            }

            //透明压缩类型
            switch (iphoneTransparentType)
            {
                case IphoneTransparentTextureType.RGBA_Compressed_PVRTC_2_bits:
                    iPhoneTransparentImporter = TextureImporterFormat.PVRTC_RGBA2;
                    break;
                case IphoneTransparentTextureType.RGBA_Compressed_PVRTC_4_bits:
                    iPhoneTransparentImporter = TextureImporterFormat.PVRTC_RGBA4;
                    break;
            }
            switch (androidTransparentType)
            {
                case AndroidTransparentTextureType.RGBA_Compressed_ETC2_8_bits:
                    androidTransparentImporter = TextureImporterFormat.ETC2_RGBA8;
                    break;
            }

            if (fileList != null && fileList.Count > 0)
            {
                int i = 0;
                foreach (var item in fileList)
                {
                    ChangeSettingByTexturePath(item);
                    EditorUtility.DisplayProgressBar("Set Texture Setting", item, i / fileList.Count);
                    i++;
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
            else if (string.IsNullOrEmpty(texturePath) == false)
            {
                ChangeSettingByTexturePath(texturePath);
            }
            else
            {
                Debug.LogError("未选中图片文件或该文件夹未包含图片");
            }
            ClearSelectedData();
            Debug.Log("设置完毕");
            myWindow.Close();
        }
    }

    static private void ChangeSettingByTexturePath(string texPath)
    {
        bool isTransparent = false;
        bool needCheckIsTransparent = true;
        TextureImporter textImport = AssetImporter.GetAtPath(texPath) as TextureImporter;
        TextureImporterFormat androidFormatSetting;
        TextureImporterFormat iphoneFormatSetting;
        TextureImporterFormat standaloneFormatSetting;
        int maxTextureSize;
        textImport.GetPlatformTextureSettings(standalonePlatform, out maxTextureSize, out standaloneFormatSetting);
        textImport.GetPlatformTextureSettings(androidPlatform, out maxTextureSize, out androidFormatSetting);
        textImport.GetPlatformTextureSettings(iphonePlatform, out maxTextureSize, out iphoneFormatSetting);
        if (isForceSetMaxSize == false)
        {
            if (isUIAtlas)
            {
                texMaxSize = 1024;
            }
            else
            {
                if (CheckBuildTargetHasSetting(texPath) == false)
                {
                    Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D));
                    texMaxSize = GetNextPowOf2(Mathf.Max(tex.height, tex.width));
                    isTransparent = true;
                    needCheckIsTransparent = false;
                }
                else
                {
                    texMaxSize = maxTextureSize;
                }
            }
        }
        if ((androidFormatSetting == androidOpaqueImporter && iphoneFormatSetting == iPhoneOpaqueImporter && maxTextureSize == texMaxSize) ||
            (androidFormatSetting == androidTransparentImporter && iphoneFormatSetting == iPhoneTransparentImporter && maxTextureSize == texMaxSize))
        {
            return;
        }
        if ((androidFormatSetting == TextureImporterFormat.ETC_RGB4 || androidFormatSetting == TextureImporterFormat.ETC2_RGB4) &&
            (iphoneFormatSetting == TextureImporterFormat.PVRTC_RGB2 || iphoneFormatSetting == TextureImporterFormat.PVRTC_RGB4)&& 
            maxTextureSize == texMaxSize)
        {
            return;
        }
        else if((iphoneFormatSetting == TextureImporterFormat.PVRTC_RGBA2 || iphoneFormatSetting == TextureImporterFormat.PVRTC_RGBA4)&&
            androidFormatSetting == TextureImporterFormat.ETC2_RGBA8 && maxTextureSize == texMaxSize)
        {
            return;
        }
        else if((androidFormatSetting == TextureImporterFormat.ETC_RGB4 || androidFormatSetting == TextureImporterFormat.ETC2_RGB4) ||
             (iphoneFormatSetting == TextureImporterFormat.PVRTC_RGB2 || iphoneFormatSetting == TextureImporterFormat.PVRTC_RGB4))
        {
            needCheckIsTransparent = false;
        }
        if (standaloneFormatSetting == TextureImporterFormat.ETC2_RGB4 || standaloneFormatSetting == TextureImporterFormat.ETC2_RGBA8)
        {
            isTransparent = true;
            needCheckIsTransparent = false;
        }
        textImport.textureType = TextureImporterType.Default;
        textImport.isReadable = true;
        AssetDatabase.ImportAsset(texPath);
        if (isUIAtlas == false && needCheckIsTransparent)
        {
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D));
            Color[] colArray = texture.GetPixels();
            for (int j = 0; j < colArray.Length; j++)
            {
                if ((colArray[j].r + colArray[j].g + colArray[j].b == 0) && colArray[j].a < 1 ||
                    (colArray[j].r + colArray[j].g + colArray[j].b >= 0) && colArray[j].a < 1 && colArray[j].a > 0.1f)
                {
                    isTransparent = true;
                    break;
                }
            }
        }
        if (changeDetailOption)
        {
            textImport.mipmapEnabled = generateMipMaps;
        }
        if (isTransparent || isUIAtlas)
        {
            textImport.SetPlatformTextureSettings(androidPlatform, texMaxSize, androidTransparentImporter);     //设置Android平台
            textImport.SetPlatformTextureSettings(iphonePlatform, texMaxSize, iPhoneTransparentImporter);   //设置iPhone平台
        }
        else
        {
            textImport.SetPlatformTextureSettings(androidPlatform, texMaxSize, androidOpaqueImporter);     //设置Android平台
            textImport.SetPlatformTextureSettings(iphonePlatform, texMaxSize, iPhoneOpaqueImporter);   //设置iPhone平台
        }
        textImport.isReadable = false;
        AssetDatabase.ImportAsset(texPath);
    }

    public static bool CheckBuildTargetHasSetting(string texPath)
    {

        if (string.IsNullOrEmpty(texPath))
        {
            return false;
        }
        string metaPath = GetMetaPathByTexPath(texPath);
        string[] allLineStrArray = ReadFile(metaPath);
        if (allLineStrArray == null || allLineStrArray.Length == 0)
        {
            Debug.LogError("读取meta文件失败，metaPath：" + metaPath);
            return false;
        }
        bool isContainiPhoneTarget = false;
        bool isContainAndroidTarget = false;
        for (int i = 0; i < allLineStrArray.Length; i++)
        {
            string tempStr = allLineStrArray[i];
            tempStr = tempStr.Replace(" ", "");
            if (tempStr.IndexOf("buildTarget:iPhone") != -1)
            {
                isContainiPhoneTarget = true;
            }
            if(tempStr.IndexOf("buildTarget:Android") == -1)
            {
                isContainAndroidTarget = true;
            }
        }
        if (isContainiPhoneTarget && isContainAndroidTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetMetaPathByTexPath(string texPath)
    {
        if (string.IsNullOrEmpty(texPath))
        {
            return null;
        }
        return texPath + ".meta";
    }

    static bool anotherIs2Power(int num)
    {
        if (num < 2)
        {
            return false;
        }

        if ((num & num - 1) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static int GetNextPowOf2(int num)
    {
        if (anotherIs2Power(num))
        {
            return num;
        }
        else
        {
            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;
            if (num + 1 >= 4096)
            {
                return 4096;
            }
            else
            {
                return num + 1;
            }
        }
    }

    static private void ClearSelectedData()
    {
        fileList.Clear();
        texturePath = string.Empty;
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
