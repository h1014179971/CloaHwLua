using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Foundation
{
    public class TextureScale 
    {
        /// <summary>
        /// 图片缩放（双线性Bilinear算法,用Unity GetPixelBilinear 接口）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            //needs to be ARGB32, RGBA32, RGB24, R8, Alpha8 or one of float formats
            Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }
        /// <summary>
        /// 图片缩放（双线性Bilinear算法）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>

        Texture2D ScaleTextureBilinear(Texture2D source, int targetWidth, int targetHeight)
        {
            Vector2 scaleFactor = new Vector2(targetWidth / source.width, targetHeight / source.height);
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(source.width * scaleFactor.x), Mathf.CeilToInt(source.height * scaleFactor.y));
            float scaleW = 1.0f / scaleFactor.x;
            float scaleH = 1.0f / scaleFactor.y;
            int maxX = source.width - 1;
            int maxY = source.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    // Bilinear Interpolation
                    float targetX = x * scaleW;
                    float targetY = y * scaleH;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = source.GetPixel(x1, y1);
                    Color color2 = source.GetPixel(x2, y1);
                    Color color3 = source.GetPixel(x1, y2);
                    Color color4 = source.GetPixel(x2, y2);
                    Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                        );
                    newTexture.SetPixel(x, y, color);
                }
            }

            return newTexture;
        }

        public static void SaveTexture(Texture2D t, string path)
        {
            byte[] byt = DeCompress(t).EncodeToPNG();
            // 该函数会直接覆盖已存在的同名文件
            File.WriteAllBytes(path, byt);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif 
        }
        /// <summary>
        /// 更改图片格式
        /// </summary>
        /// <param name="oldTexture"></param>
        /// <param name="newFormat"></param>
        /// <returns></returns>
        public static Texture2D ChangeFormat(Texture2D source, TextureFormat newFormat)
        {
            //Create new empty Texture
            Texture2D newTex = new Texture2D(source.width, source.height, newFormat, false);
            newTex.SetPixels(source.GetPixels());
            newTex.Apply();

            return newTex;
        }
        /// <summary>
        ///  图片合成
        /// </summary>
        /// <param name="BigTexture"></param>
        /// <param name="SmallTexture"></param>
        /// <param name="alpha">是否保留small图片通道</param>
        /// <returns></returns>
        public static Texture2D ComplexTwoTextures(Texture2D BigTexture, Texture2D SmallTexture,bool alpha = false)
        {
            /////needs to be ARGB32, RGBA32, RGB24, R8, Alpha8 or one of float formats
            Texture2D tex = new Texture2D(BigTexture.width, BigTexture.height, TextureFormat.RGBA32, false);
            tex.SetPixels(BigTexture.GetPixels());
            int startWidth = BigTexture.width/2 - SmallTexture.height /2;
            int startHeight = BigTexture.height / 2 - SmallTexture.height / 2;
            if(alpha)
                tex.SetPixels32(startWidth, startHeight, SmallTexture.width, SmallTexture.height, SmallTexture.GetPixels32());
            else
            {
                for(int i = 0; i < SmallTexture.width; i++)
                {
                    for(int j = 0; j < SmallTexture.height; j++)
                    {
                        //Color color = SmallTexture.GetPixelBilinear((float)i /(float)SmallTexture.width,(float)j / (float)SmallTexture.height);
                        Color color = SmallTexture.GetPixel(i,j);
                        if (color.a == 0) continue;
                        tex.SetPixel(i+startWidth, j+startHeight, color);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// 图片解压
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Texture2D DeCompress(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}

