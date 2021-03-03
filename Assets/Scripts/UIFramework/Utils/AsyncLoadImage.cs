using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

public class AsyncLoadImage : Singleton<AsyncLoadImage>
{
    string _imageCachePath;
    string _blurCachePath;

	public string ImageCachePath
	{
		get
		{
            if (string.IsNullOrEmpty(_imageCachePath))
            {
                _imageCachePath = Application.persistentDataPath + "/ImageCache/";
            }
            return _imageCachePath;

		}
	}

    public override void Init()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
		}
	}

	public IEnumerator Load(MonoBehaviour mono, string url, Image image, Action callback = null)
	{
		if (string.IsNullOrEmpty(url) || image == null)
			yield break;

		Texture2D tex2d = new Texture2D(1, 1);
		yield return mono.StartCoroutine(LoadTexture2D(mono, url, tex2d, callback));
		Sprite s = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
		image.sprite = s;

        yield return null;
		if (callback != null)
		{
			callback();
		}
	}

    public IEnumerator LoadTexture2D(MonoBehaviour mono, string url, Texture2D tex2d, Action callback = null)
	{
        if (string.IsNullOrEmpty(url) || tex2d == null)
			yield break;

        //判断是否是第一次加载这张图片
        string filepath = ImageCachePath + url.GetHashCode();
        bool bExist = File.Exists(filepath);
        if (!bExist)
        {
            filepath = url;
		}

        if (!filepath.Contains("file://") && !filepath.Contains("http://") && !filepath.Contains("https://"))
        {
            while (filepath[0] == '/')
            {
                filepath = filepath.Substring(1);
            }
            filepath = "file:///" + filepath;
        }

        DebugUtil.Info("filePath:" + filepath);
        using (WWW www = new WWW(filepath))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                tex2d.LoadImage(www.bytes);
                if (!bExist)
                {
                    byte[] pngData = tex2d.EncodeToPNG();
                    File.WriteAllBytes(ImageCachePath + url.GetHashCode(), pngData);
                }
            }
            else
            {
                DebugUtil.Error("LoadFileError:" + www.error + ", URL:" + filepath);
            }
        }

        yield return null;
		if (callback != null)
		{
			callback();
		}
    }

    public IEnumerator LoadTexture2D(MonoBehaviour mono, byte[] data, Texture2D tex2d, Action callback = null)
    {
        if (data == null || tex2d == null)
            yield break;

        //判断是否是第一次加载这张图片
        //string filepath = ImageCachePath + url.GetHashCode();
        //bool bExist = File.Exists(filepath);
        //if (bExist)
        //{
        //    filepath = "file:///" + filepath;
        //}
        //else
        //{
        //    filepath = url;
        //}
        //Debug.Log("path:" + filepath);
        //WWW www = new WWW(filepath);
        //yield return www;
        //if (string.IsNullOrEmpty(www.error))
        //{
        tex2d.LoadImage(data);
        //if (!bExist)
        //{
        //    byte[] pngData = tex2d.EncodeToPNG();
        //    File.WriteAllBytes(ImageCachePath + url.GetHashCode(), pngData);
        //}
        ////}
        //else
        //{
        //    Debug.Log("LoadFileError:" + www.error);
        //}

        yield return null;
        if (callback != null)
        {
            callback();
        }
    }
}