using UnityEngine;
using System;
using System.Collections;
using System.IO;

public static class ScreenShot
{
    public static Vector2 ImageScale = new Vector2(1f, 1f);

    private static string GenerateFilename(string suffix)
    {
        return "LastScreenShot." + suffix;
        //DateTime time = DateTime.Now;
        //return string.Format("SC_{0}_{1}_{2}_{3}_{4}_{5}.{6}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, suffix);
    }

    /// <summary>  
    /// 使用ReadPixels截取屏幕  
    /// 左下角为(0,0)  
    /// </summary>   
    /// <param name="callback">回调接口</param>  
    public static IEnumerator CaptureByReadPixels(Action<string, Texture2D> callback)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();
        string filename = GenerateFilename("jpg");
        string filepath = Utils.GetDefaultFilePath() + filename;

        //初始化Texture2D  
        Rect rc = new Rect(0, 0, Screen.width, Screen.height);
        Texture2D mTexture = new Texture2D((int)rc.width, (int)rc.height, TextureFormat.ARGB32, false);
        //读取屏幕像素信息并存储为纹理数据  
        mTexture.ReadPixels(rc, 0, 0);
        //应用  
        mTexture.Apply();

        //将图片信息编码为字节信息  
        byte[] bytes = mTexture.EncodeToJPG(80);
        //保存  
        File.WriteAllBytes(filepath, bytes);

        if (callback != null)
        {
            callback(filepath, mTexture);
        }

        //如果需要可以返回截图  
        //return mTexture;  
    }

    public static IEnumerator CaptureByCamera(Camera camera, Action<Texture2D> callback, string savePath = null)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();
        string filename = GenerateFilename("png");
        string filepath = Utils.GetDefaultFilePath() + filename;

        // 创建一个RenderTexture对象
        Rect rect = new Rect(0, 0, Screen.width * ImageScale.x, Screen.height * ImageScale.y);
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 16);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.targetTexture = rt;
        camera.Render();
        yield return new WaitForEndOfFrame();

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();
        // 重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        yield return new WaitForEndOfFrame();

        //将图片信息编码为字节信息  
        if (!string.IsNullOrEmpty(savePath))
        {
            byte[] bytes = screenShot.EncodeToJPG(80);
            //byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(filepath, bytes);
        }

        if (callback != null)
        {
            callback(screenShot);
        }
    }

    public static IEnumerator CaptureByCamera(Camera camera, Action<string, Texture2D> callback)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();
        string filename = GenerateFilename("png");
        string filepath = Utils.GetDefaultFilePath() + filename;

        // 创建一个RenderTexture对象
        Rect rect = new Rect(0, 0, Screen.width * ImageScale.x, Screen.height * ImageScale.y);
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 16);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.targetTexture = rt;
        camera.Render();
        yield return new WaitForEndOfFrame();

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();
        // 重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        yield return new WaitForEndOfFrame();

        //将图片信息编码为字节信息  
        byte[] bytes = screenShot.EncodeToJPG(80);
        //byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filepath, bytes);

        if (callback != null)
        {
            callback(filepath, screenShot);
        }
    }

    /// <summary>调用Unity已封装好的截屏方法，仅支持png格式。
    /// fullPath - 截屏保存的完整路径，需包含文件名和后缀名。
    /// callback - 截屏完成回调，会把fullPath返回。
    /// </summary>
    public static IEnumerator CaptureByUnity(string fullPath = null,Action<string> callback = null)
    {
        if(string.IsNullOrEmpty(fullPath))
        {
            string systemTime = System.DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH-mm-ss");
            fullPath = Utils.GetDefaultFilePath() + systemTime + ".png";
        }

        float startTime = Time.realtimeSinceStartup;
        ScreenCapture.CaptureScreenshot(fullPath);

        //因为截屏保存需要时间，所以加等待判断文件已存在再执行后续操作
        while(!Utils.IsFileExistByPath(fullPath))
        {
            if(Time.realtimeSinceStartup - startTime >= 5f)//超时
            {
                string error = "ScreenCapture timeout!!!Path===" + fullPath;
                Debug.LogError(error);
                if(callback != null)
                {
                    callback(error);
                }
                yield break;
            }
            yield return new WaitForSeconds(0.05f);
        }

        float deltaTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("ScreenCapture done，total time：" + deltaTime + "s  Path===" + fullPath);

        if(callback != null)
        {
            callback(fullPath);
        }
    }
}