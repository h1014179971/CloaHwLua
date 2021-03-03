//
// BuildScript.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2020 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Foundation;
using Debug = UnityEngine.Debug;

namespace libx
{
	public static class BuildScript
	{
		public static BuildTarget buildTarget= BuildTarget.NoTarget;
		public static string outputPath  = "DLC/" + GetPlatformName(); 

		public static void ClearAssetBundles ()
		{
			var allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames ();
			for (var i = 0; i < allAssetBundleNames.Length; i++) {
				var text = allAssetBundleNames [i];
				if (EditorUtility.DisplayCancelableProgressBar (
					                string.Format ("Clear AssetBundles {0}/{1}", i, allAssetBundleNames.Length), text,
					                i * 1f / allAssetBundleNames.Length))
					break;

				AssetDatabase.RemoveAssetBundleName (text, true);
			} 
			EditorUtility.ClearProgressBar ();
		}
		internal static void ApplyAppSetting()
		{
			var rules = GetAppSetting();
		}
		internal static void ApplyBuildRules ()
		{
			var rules = GetBuildRules ();
			rules.Apply ();
		}
		internal static EditorRuntimeInitializeOnLoad GetAppSetting()
		{
			return GetAsset<EditorRuntimeInitializeOnLoad>("Assets/Plugins/AppSetting.asset");
		}
		internal static BuildRules GetBuildRules ()
		{
			return GetAsset<BuildRules> ("Assets/Plugins/Rules.asset");
		} 

		public static void CopyAssetBundlesTo (string path)
		{
			var files = new[] {
				Versions.Dataname,
				Versions.Filename,
			};  
			if (!Directory.Exists (path)) {
				Directory.CreateDirectory (path);
			}
			foreach (var item in files) {
				var src = outputPath + "/" + item;
				var dest = Application.streamingAssetsPath + "/" + item;
				if (File.Exists (src)) {
					File.Copy (src, dest, true);
				}
			}
		}
		public static void CopyResAssetBundles()
        {
			string path = Path.Combine(Application.streamingAssetsPath, outputPath);
			if (Directory.Exists(path))
			{
				Directory.Delete(path,true);
			}
			CopyDirectory(outputPath, path);
			Refresh();
		}
		public static void CopyAssetBundlesTo()
        {
			var watch = new Stopwatch();
			watch.Start();
			string path = Path.Combine(Application.streamingAssetsPath, outputPath);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			CopyDirectory(outputPath, path);
			DeleteRes();
			Refresh();
			Debug.Log("CopyAssetBundlesTo " + watch.ElapsedMilliseconds + " ms.");
		}
		public static void CopyAssetBundlesToRemoteCDN(string host,string userName,string password,BuildTarget target)
        {
			var ftpUtil = new FTPUtil(host, userName, password);
			var upath = Path.Combine(outputPath,"ver");
			var watch = new Stopwatch();
			watch.Start();
			Upload(outputPath,ftpUtil);
			watch.Stop();
			Debug.Log("CopyAssetBundlesToRemoteCDN " + watch.ElapsedMilliseconds + " ms.");
		}
		public static void CreateFTPDirectory(DirectoryInfo source,FTPUtil ftpUtil)
        {
			DirectoryInfo parentSource = source.Parent;
			string parentDir = parentSource.FullName;
			int index = parentDir.IndexOf(GetPlatformName());
			if(index != -1)
            {
				parentDir = parentDir.Substring(index, parentDir.Length - index);
				
			}
            else
            {
				parentDir = string.Empty;
			}
			List<string> parentDirs = ftpUtil.GetDirectorys(parentDir);
			bool isCreate = true;
			for(int i = 0; i < parentDirs.Count; i++)
            {
				string dir = parentDirs[i];
				int idex = dir.IndexOf("<DIR>");
				dir = dir.Substring(idex + 5, dir.Length - idex - 5);
				dir = dir.Replace(" ", "");
				if (dir.Equals(source.Name))
                {
					isCreate = false;
					break;
                }
			}
			if (isCreate)
            {
				
				ftpUtil.MakeDirectory(Path.Combine(parentDir, source.Name));
			}
				

		}
		public static void Upload(string srcDir,FTPUtil ftpUtil)
        {
			//DirectoryInfo parentDir = new DirectoryInfo(Path.Combine(Application.dataPath, "DLC"));
			DirectoryInfo source = new DirectoryInfo(srcDir);
			CreateFTPDirectory(source, ftpUtil);
			if (!source.Exists)
			{
				Debug.LogError($"没有{source.FullName}文件夹");
				return;
			}
			FileInfo[] files = source.GetFiles();
			DirectoryInfo[] dirs = source.GetDirectories();
			if (files.Length == 0 && dirs.Length == 0)
			{
				return;
			}
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				string fileStr = fileInfo.FullName;
				int index = fileStr.IndexOf(GetPlatformName());
				if (index != -1)
				{
					fileStr = fileStr.Substring(index, fileStr.Length - index);

				}
				else
					fileStr = files[i].Name;
				ftpUtil.Upload(files[i].FullName, fileStr);
			}
            for (int j = 0; j < dirs.Length; j++)
            {
				Upload(dirs[j].FullName, ftpUtil);

			}
        }
		public static void CopyAssetBundlesToLocalCDN(string host, BuildTarget target)
        {
			var watch = new Stopwatch();
			watch.Start();
			//string path = Path.Combine("G:\\Moha\\UnityPro\\AliCodeup\\hw_game_unity_Delivery\\CDN", outputPath);
			string path = Path.Combine(host, "DLC/" + GetPlatformForAssetBundles(target));
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			CopyDirectory(outputPath, path);
			DeleteRes();
			Refresh();
			watch.Stop();
			Debug.Log("CopyAssetBundlesTo " + watch.ElapsedMilliseconds + " ms.");
		}
		public static void DeleteRes()
        {
			string respath = Path.Combine(Application.streamingAssetsPath, outputPath);
			respath = Path.Combine(respath,"res");
			if (File.Exists(respath))
				File.Delete(respath);
        }
		public static void DeleteDLC()
        {
			string path = Path.Combine(Application.streamingAssetsPath, "DLC");
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			Refresh();
		}
		static void Refresh()
        {
			AssetDatabase.Refresh();
		}
		/// <summary>
		/// 拷贝文件
		/// </summary>
		/// <param name="srcDir">起始文件夹</param>
		/// <param name="tgtDir">目标文件夹</param>
		public static void CopyDirectory(string srcDir, string tgtDir)
		{
			DirectoryInfo source = new DirectoryInfo(srcDir);
			DirectoryInfo target = new DirectoryInfo(tgtDir);

			if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new Exception("父目录不能拷贝到子目录！");
			}

			if (!source.Exists)
			{
				Debug.LogError($"没有{source.FullName}文件夹");
				return;
			}

			if (!target.Exists)
			{
				target.Create();
			}

			FileInfo[] files = source.GetFiles();
			DirectoryInfo[] dirs = source.GetDirectories();
			if (files.Length == 0 && dirs.Length == 0)
			{
				return;
			}
			for (int i = 0; i < files.Length; i++)
			{
				File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
			}
			for (int j = 0; j < dirs.Length; j++)
			{
				CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
			}
			
		}

		public static string GetPlatformName ()
		{
			if (buildTarget != BuildTarget.NoTarget)
				return GetPlatformForAssetBundles(buildTarget);
			return GetPlatformForAssetBundles (EditorUserBuildSettings.activeBuildTarget);
		}

		private static string GetPlatformForAssetBundles (BuildTarget target)
		{
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (target) {
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
#if UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSX:
				return "OSX";
#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
#endif
			default:
				return null;
			}
		}

		private static string[] GetLevelsFromBuildSettings ()
		{
			List<string> scenes = new List<string> ();
			foreach (var item in GetBuildRules().scenesInBuild) {
				var path = AssetDatabase.GetAssetPath (item);
				if (!string.IsNullOrEmpty (path)) {
					scenes.Add (path);
				}
			}

			return scenes.ToArray ();
		}

		private static string GetAssetBundleManifestFilePath ()
		{
			var relativeAssetBundlesOutputPathForPlatform = Path.Combine ("Asset", GetPlatformName ());
			return Path.Combine (relativeAssetBundlesOutputPathForPlatform, GetPlatformName ()) + ".manifest";
		}

		public static void BuildStandalonePlayer ()
		{
			var outputPath =
				Path.Combine (Environment.CurrentDirectory,
					"Build/" + GetPlatformName ()
                        .ToLower ()); //EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;

			var levels = GetLevelsFromBuildSettings ();
			if (levels.Length == 0) {
				Debug.Log ("Nothing to build.");
				return;
			}

			//var targetName = GetBuildTargetName (EditorUserBuildSettings.activeBuildTarget);
			var targetName = GetBuildTargetName(buildTarget);
			if (targetName == null)
				return;
#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			//BuildPipeline.BuildPlayer(levels, path + targetName, EditorUserBuildSettings.activeBuildTarget, option);
			BuildPipeline.BuildPlayer(levels, path + targetName, buildTarget, option);
#else
			var buildPlayerOptions = new BuildPlayerOptions {
				scenes = levels,
				locationPathName = outputPath + targetName,
				assetBundleManifestPath = GetAssetBundleManifestFilePath (),
				//target = EditorUserBuildSettings.activeBuildTarget,
				target = buildTarget,
				options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None
			};
			BuildPipeline.BuildPlayer (buildPlayerOptions);
#endif
		}

		public static string CreateAssetBundleDirectory ()
		{
			// Choose the output path according to the build target.
			if (!Directory.Exists (outputPath))
				Directory.CreateDirectory (outputPath);

			return outputPath;
		}

		public static void BuildAssetBundles (BuildTarget targetPlatform)
		{
			buildTarget = targetPlatform;
			BuildScript.outputPath = "DLC/" + GetPlatformName();
			// Choose the output path according to the build target.
			var outputPath = CreateAssetBundleDirectory ();
			const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
			//var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
			var rules = GetBuildRules ();
			var builds = rules.GetBuilds ();
			var assetBundleManifest = BuildPipeline.BuildAssetBundles (outputPath, builds, options, targetPlatform);
			if (assetBundleManifest == null) {
				return;
			}

			var manifest = GetManifest ();
			var dirs = new List<string> ();
			var assets = new List<AssetRef> ();
			var bundles = assetBundleManifest.GetAllAssetBundles ();
			var bundle2Ids = new Dictionary<string, int> ();
			for (var index = 0; index < bundles.Length; index++) {
				var bundle = bundles [index];
				bundle2Ids [bundle] = index;
			}

			var bundleRefs = new List<BundleRef> ();
			for (var index = 0; index < bundles.Length; index++) {
				var bundle = bundles [index];
				var deps = assetBundleManifest.GetAllDependencies (bundle);
				var path = string.Format ("{0}/{1}", outputPath, bundle);
				if (File.Exists (path)) {
					using (var stream = File.OpenRead (path)) {
						bundleRefs.Add (new BundleRef {
							name = bundle,  
							id = index,
							deps = Array.ConvertAll (deps, input => bundle2Ids [input]),
							len = stream.Length,
							hash = assetBundleManifest.GetAssetBundleHash (bundle).ToString (),
						});
					}
				} else {
					Debug.LogError (path + " file not exsit.");
				}
			}

			for (var i = 0; i < rules.ruleAssets.Length; i++) {
				var item = rules.ruleAssets [i];
				var path = item.path;
				var dir = Path.GetDirectoryName (path).Replace("\\", "/");
				var index = dirs.FindIndex (o => o.Equals (dir));
				if (index == -1) {
					index = dirs.Count;
					dirs.Add (dir);
				}

				var asset = new AssetRef { bundle = bundle2Ids [item.bundle], dir = index, name = Path.GetFileName (path) };
				assets.Add (asset);
			}

			manifest.dirs = dirs.ToArray ();
			manifest.assets = assets.ToArray ();
			manifest.bundles = bundleRefs.ToArray ();

			EditorUtility.SetDirty (manifest);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			var manifestBundleName = "manifest.unity3d";
			builds = new[] {
				new AssetBundleBuild {
					assetNames = new[] { AssetDatabase.GetAssetPath (manifest), },
					assetBundleName = manifestBundleName
				}
			};

			BuildPipeline.BuildAssetBundles (outputPath, builds, options, targetPlatform);
			ArrayUtility.Add (ref bundles, manifestBundleName);  

			Versions.BuildVersions (outputPath, bundles, GetBuildRules ().AddVersion ());
		}

		private static string GetBuildTargetName (BuildTarget target)
		{
			var time = DateTime.Now.ToString ("yyyyMMdd-HHmmss");
			var name = PlayerSettings.productName + "-v" + PlayerSettings.bundleVersion + ".";
			switch (target) {
			case BuildTarget.Android:
				return string.Format ("/{0}{1}-{2}.apk", name, GetBuildRules().version, time);

			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return string.Format ("/{0}{1}-{2}.exe", name, GetBuildRules().version, time);

#if UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSX:
				return "/" + name + ".app";

#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "/" + path + ".app";

#endif

			case BuildTarget.WebGL:
			case BuildTarget.iOS:
				return "";
			// Add more build targets for your own.
			default:
				Debug.Log ("Target not implemented.");
				return null;
			}
		}

		private static T GetAsset<T> (string path) where T : ScriptableObject
		{
			var asset = AssetDatabase.LoadAssetAtPath<T> (path);
			if (asset == null) {
				asset = ScriptableObject.CreateInstance<T> ();
				AssetDatabase.CreateAsset (asset, path);
				AssetDatabase.SaveAssets ();
			}

			return asset;
		} 

		public static Manifest GetManifest ()
		{
			return GetAsset<Manifest> (Assets.ManifestAsset);
		}
	}
}