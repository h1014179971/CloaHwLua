using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public class Tool 
    {
        /// <summary>
        /// 转换成相对于左下角（0，0）的坐标
        /// </summary>
        /// <param name="rectTrans"></param>
        /// <returns></returns>
        public static Vector3 ConvertGLPosition(RectTransform rectTrans)
        {
            Vector3 glPos = rectTrans.anchoredPosition3D;
            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;
            Vector2 offsetMin = rectTrans.offsetMin;
            Vector2 size = rectTrans.sizeDelta;
            Vector2 pivot = rectTrans.pivot;
            Vector2 offsetMax = rectTrans.offsetMax;
            glPos.x = FixScreen.width * anchorMin.x + offsetMin.x + (size.x - FixScreen.width * anchorMin.x)* pivot.x;
            glPos.y = FixScreen.height * anchorMin.y + offsetMin.y + (size.y - FixScreen.height * anchorMin.y) * pivot.y;
            return glPos;
        }
        public static Vector3 ConvertGLPosition(RectTransform rectTrans,Vector3 pos)
        {
            Vector3 glPos = pos;
            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;
            Vector2 offsetMin = rectTrans.offsetMin;
            Vector2 size = rectTrans.sizeDelta;
            Vector2 pivot = rectTrans.pivot;
            Vector2 offsetMax = rectTrans.offsetMax;
            glPos.x = FixScreen.width * anchorMin.x + offsetMin.x + (size.x - FixScreen.width * anchorMin.x) * pivot.x;
            glPos.y = FixScreen.height * anchorMin.y + offsetMin.y + (size.y - FixScreen.height * anchorMin.y) * pivot.y;
            return glPos;
        }
    }
}

