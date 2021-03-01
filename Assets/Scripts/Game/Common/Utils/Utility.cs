using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    public class Utility
    {                         
        public static float mulWidth = FixScreen.width / Screen.width;
        public static float mulHeight = FixScreen.height / Screen.height;
        public static Vector2 vecMul2 = new Vector2(mulWidth, mulHeight);
        public static Vector3 vecMul3 = new Vector3(mulWidth, mulHeight, 1);
        private static float minAngle = 45;
        private static float roundAngle = 360;
        //返回值为[0,360)
        public static float Angle_360(Vector2 v1, Vector2 v2)
        {
            float angle = Vector2.Angle(Vector2.right, v2 - v1);
            if (v2.y >= v1.y)
                return angle % roundAngle;
            else
                return (roundAngle - angle) % roundAngle;
        }
        //两点间是否是45的倍数
        public static bool IsAngle_45(Vector2 v1, Vector2 v2)
        {
            float angle = Angle_360(v1, v2);
            float mul = angle / minAngle;
            mul = mul % 1;
            if (mul > 0)
                return false;
            return true;
        }
        //计算出v1的45度倍数角
        public static float Angle_45(Vector2 v1, Vector2 v2, bool isMin)
        {
            float angle = Angle_360(v1, v2);
            int mul = Mathf.FloorToInt(angle / minAngle);
            if (isMin)
            {
                //如果isMin=true 大于22.5度进1
                if (angle % minAngle  >= minAngle * 0.5f)
                    mul += 1;
            }
            else
            {
                //如果isMin=true 小于22.5度进1
                if (angle % minAngle < minAngle * 0.5f) 
                    mul += 1;
            }
            angle = mul * minAngle;
            angle -= Mathf.FloorToInt(angle / roundAngle) * roundAngle;
            return angle;
        }
        //计算出v1的45度倍数角
        public static int AngleMul45(Vector2 v1, Vector2 v2, bool isMin)
        {
            float angle = Angle_360(v1, v2);
            int mul = Mathf.FloorToInt(angle / minAngle);
            if (isMin)
            {
                //如果isMin=true 大于22.5度进1
                if (angle % minAngle   >= minAngle *0.5 ) 
                    mul += 1;
            }
            else
            {
                //如果isMin=true 小于22.5度进1
                if (angle % minAngle  < minAngle *0.5) 
                    mul += 1;
            }
            return mul;
        }
        public static float Angle45(int mul)
        {
            float angle = mul * minAngle;
            angle -= Mathf.FloorToInt(angle / roundAngle) * roundAngle;
            return angle;
        }
        //计算出直线a和b
        public static object LineAB(Vector2 v1, Vector2 v2, bool isMin = true)
        {

            float angle = Angle_45(v1, v2, isMin);
            Debug.Log($"angle======{angle}");
            if (angle == 90)
                return 1f;
            else if (angle == 270)
                return -1f;
            else
            {
                //float a = Mathf.Atan(angle / roundAngle * Mathf.PI);
                float a = Mathf.Tan(angle * Mathf.Deg2Rad);
                float b = v1.y - a * v1.x;
                return new Vector2(a, b);
            }

        }
        public static object LineAB(Vector2 v1, float angle)
        {

            //float angle = Angle_45(v1, v2, isMin);
            if (angle == 90)
                return 1f;
            else if (angle == 270)
                return -1f;
            else
            {
                //float a = Mathf.Atan(angle / roundAngle * Mathf.PI);
                float a = Mathf.Tan(angle * Mathf.Deg2Rad);
                float b = v1.y - a * v1.x;
                return new Vector2(a, b);
            }

        }
        //计算出v1的45度倍数角
        public static bool AngleBe(Vector2 v1, Vector2 v2)
        {
            float angle = Angle_360(v1, v2);
            //如果isMin=true 大于22.5度进1
            if (angle % minAngle >= minAngle *0.5f)
                return true;
            return false;
        }

        //颜色转换
        public static Color ConvertColor(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red:
                    return Utils.HexToColor("EF325A");
                case ColorType.Yellow:
                    return Utils.HexToColor("FFA200");
                case ColorType.Blue:
                    return Utils.HexToColor("00BAFF");
                case ColorType.Green:
                    return Utils.HexToColor("23B838");
                case ColorType.Purple:
                    return Utils.HexToColor("AC3BF3");
                default:
                    return Color.white;
            }
            
        }
    }
}

