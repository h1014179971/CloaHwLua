using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
public class TextureCheck  {

	[MenuItem("检查图片/检查文件夹贴图不符合的")]
	static void 检查是否有真彩贴图()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.LogError("选择一个文件夹");
			return;
		}

		// StringBuilder text = new StringBuilder();
		List<string> imageList = new List<string>();
		List<string> errorList = new List<string>();

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;
			if (textureImporter.textureType == TextureImporterType.Sprite)
				continue;
			//查看贴图类型
			if (textureImporter.textureType == TextureImporterType.Default || textureImporter.textureType == TextureImporterType.Cursor)
			{
				Debug.Log("类型不符合:" + assetPath + ",现在类型是:" + textureImporter.textureType);
				errorList.Add("类型不符合: " + ",现在类型是:" + textureImporter.textureType);
				imageList.Add(assetPath);
				continue;
			}
			//查看是否开启了minmap
			if (textureImporter.mipmapEnabled)
			{
				Debug.Log( assetPath + ",该图片开启了minmap");
				errorList.Add(",该图片开启了minmap");
				imageList.Add(assetPath);
				continue;
			}
			//查看压缩格式
			int maxsize = 0;
			TextureImporterFormat textureImporterFormat;
			string androidPlatform = "Android";
			if (textureImporter.GetPlatformTextureSettings(androidPlatform, out maxsize, out textureImporterFormat))
			{
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.ETC_RGB4)//不带alpha通道的
				{
					Debug.Log("格式不符合:" + assetPath + ",现在格式是:" + textureImporterFormat);
					imageList.Add(assetPath);
					errorList.Add("格式不符合: " + ",现在格式是:" + textureImporterFormat);
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.ETC2_RGBA8)//带alpha通道的
				{
					Debug.Log("格式不符合:" + assetPath + ",现在格式是:" + textureImporterFormat);
					imageList.Add(assetPath);
					errorList.Add("格式不符合: " + ",现在格式是:" + textureImporterFormat);
				}
			}

			//查看是否是2的次方
			Texture target = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as Texture;
			var type = System.Reflection.Assembly.Load ("UnityEditor.dll").GetType ("UnityEditor.TextureUtil");

//			var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");

			//MethodInfo[] methodInfos = type.GetMethods();
			//foreach (var obj in methodInfos)
			//    Debug.Log(obj);

			MethodInfo methodInfo = type.GetMethod("IsNonPowerOfTwo", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
			string isNon = methodInfo.Invoke(null, new object[] { target }).ToString();
			if (isNon.Contains("True"))
			{
				imageList.Add(assetPath);
				errorList.Add( "图片大小不是2的次幂");
				Debug.Log(assetPath + ",图片大小不是2的次幂:" + methodInfo.Invoke(null, new object[] { target }));
			}
		}
		TextureCheckEditorWindow.imageList = imageList;
		TextureCheckEditorWindow.errorList = errorList;
		TextureCheckEditorWindow.ShowWindow();

		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
		// Debug.Log(text.ToString());
	}

	[MenuItem("检查图片/转换文件夹内Android的图片为etc")]
	static void 转换Android的图片为etc()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;


			//处理压缩格式为etc
			int maxsize = 0;
			TextureImporterFormat textureImporterFormat;
			string androidPlatform = "Android";
			if (textureImporter.GetPlatformTextureSettings(androidPlatform, out maxsize, out textureImporterFormat))
			{
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.ETC_RGB4)//不带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(androidPlatform, maxsize, TextureImporterFormat.ETC_RGB4);
					textureImporter.SaveAndReimport();
					count++;
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.ETC2_RGBA8 )//带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(androidPlatform, maxsize, TextureImporterFormat.ETC2_RGBA8);
					textureImporter.SaveAndReimport();
					count++;
				}
			}
		}
		Debug.Log("压缩了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}

	[MenuItem("检查图片/转换文件夹内Android的图片为pvrtc4bits")]
	static void 转换Android的图片为etcs()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;


			//处理压缩格式为etc
			int maxsize = 0;
			TextureImporterFormat textureImporterFormat;
			string iosPlatform = "iPhone";
			if (textureImporter.GetPlatformTextureSettings(iosPlatform, out maxsize, out textureImporterFormat))
			{
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.PVRTC_RGB4)//不带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.PVRTC_RGB4);
					textureImporter.SaveAndReimport();
					count++;
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.PVRTC_RGBA4 )//带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.PVRTC_RGBA4);
					textureImporter.SaveAndReimport();
					count++;
				}
			} else {
				maxsize = 2048;
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.PVRTC_RGB4)//不带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.PVRTC_RGB4);
					textureImporter.SaveAndReimport();
					count++;
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.PVRTC_RGBA4 )//带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.PVRTC_RGBA4);
					textureImporter.SaveAndReimport();
					count++;
				}
			}
		}
		Debug.Log("压缩了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}

	[MenuItem("检查图片/转换文件夹内的图片为RGB16或者RGBA16")]
	static void 转换图片为RGB16()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;


			//处理压缩格式为etc
			int maxsize = 0;
			TextureImporterFormat textureImporterFormat;
			string iosPlatform = "iPhone";
			if (textureImporter.GetPlatformTextureSettings(iosPlatform, out maxsize, out textureImporterFormat))
			{
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.RGB16)//不带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.RGB16);
					textureImporter.SaveAndReimport();
					count++;
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.RGBA16 )//带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.RGBA16);
					textureImporter.SaveAndReimport();
					count++;
				}
			} else {
				maxsize = 2048;
				if (!textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.RGB16)//不带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.RGB16);
					textureImporter.SaveAndReimport();
					count++;
				}
				if (textureImporter.DoesSourceTextureHaveAlpha() && textureImporterFormat != TextureImporterFormat.RGBA16 )//带alpha通道的
				{

					textureImporter.SetPlatformTextureSettings(iosPlatform, maxsize, TextureImporterFormat.RGBA16);
					textureImporter.SaveAndReimport();
					count++;
				}
			}
		}
		Debug.Log("压缩了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}

	[MenuItem("检查图片/文件夹内图片改为Advanced")]
	static void 文件夹内图片改为Advanced()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;
			if(textureImporter.textureType==TextureImporterType.Sprite)
				continue;
			//去掉图片的mipmap
			if (textureImporter.textureType!=TextureImporterType.Default)
			{
				textureImporter.textureType = TextureImporterType.Default;
				textureImporter.SaveAndReimport();
				count++;
			}


		}
		Debug.Log("处理了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}
	[MenuItem("检查图片/去除文件夹内图片的read write")]
	static void 去除文件夹内图片的readWrite()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;
			if (textureImporter.isReadable)
			{
				textureImporter.isReadable = false;
				textureImporter.SaveAndReimport();
				count++;
			}


		}
		Debug.Log("处理了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}
	[MenuItem("检查图片/去除文件夹内图片的mipmap")]
	static void 去除文件夹内图片的mipmap()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;
			//去掉图片的mipmap
			if (textureImporter.mipmapEnabled)
			{
				textureImporter.mipmapEnabled = false;
				textureImporter.SaveAndReimport();
				count++;
			}


		}
		Debug.Log("处理了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}
	[MenuItem("检查图片/文件夹内图片大小改为2的次幂")]
	static void 文件夹内图片大小改为2的次幂()
	{
		if (Selection.objects.Length != 1)
		{
			Debug.LogError("选择一个文件夹");
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
		if (!Directory.Exists(path))
		{
			Debug.Log(path);
			return;
		}

		AssetDatabase.StartAssetEditing();
		var assets = new List<string>();
		int count = 0;
		string[] filePath = GetFilePath(Selection.objects);
		foreach (var guid in AssetDatabase.FindAssets("t:Texture", filePath))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null)
				continue;

			//查看是否是2的次方
			Texture target = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as Texture;
			var type = System.Reflection.Assembly.Load ("UnityEditor.dll").GetType ("UnityEditor.TextureUtil");
//			var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");

			//MethodInfo[] methodInfos = type.GetMethods();
			//foreach (var obj in methodInfos)
			//    Debug.Log(obj);

			MethodInfo methodInfo = type.GetMethod("IsNonPowerOfTwo", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
			string isNon = methodInfo.Invoke(null, new object[] { target }).ToString();
			if (isNon.Contains("True"))
			{
				textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
				textureImporter.SaveAndReimport();
				count++;
			}



		}
		Debug.Log("处理了:" + count + "个图片");
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}
	public static string[] GetFilePath(Object[] targets)
	{
		var folders = new List<string>();
		for (int i = 0; i < targets.Length; i++)
		{
			string assetPath = AssetDatabase.GetAssetPath(targets[i]);
			if (Directory.Exists(assetPath))
				folders.Add(assetPath);
		}
		return folders.ToArray();
	}
}