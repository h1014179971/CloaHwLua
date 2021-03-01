using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Foundation;
using libx;
using System.Text;

namespace Delivery
{
    public class UnitConvertMgr : Singleton<UnitConvertMgr>
    {
        private Dictionary<int, UnitConvert> _unitConvertDic = new Dictionary<int, UnitConvert>();
        public  void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            Assets.LoadAssetAsync(Files.unitConvert, typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<UnitConvert> units = FullSerializerAPI.Deserialize(typeof(List<UnitConvert>), jsonStr) as List<UnitConvert>;
                    _unitConvertDic = units.ToDictionary(key => key.x, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.unitConvert}读取失败,error={request.error}");
            };
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.unitConvert), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<UnitConvert> units = FullSerializerAPI.Deserialize(typeof(List<UnitConvert>), jsonStr) as List<UnitConvert>;
            //        _unitConvertDic = units.ToDictionary(key => key.x, value => value);
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
        }

        public string GetUnit(int key)
        {
            if (_unitConvertDic.ContainsKey(key))
            {
                if (String.IsNullOrEmpty(_unitConvertDic[key].y))
                    return "";
                else
                    return _unitConvertDic[key].y;
            }  
            return "";
        }

        public string GetValue(Long2 lhs,bool integer=false)
        {
            if (lhs.x == 0)
                return "0";
            int unit = (int)((lhs.y + Math.Log10(Mathf.Abs(lhs.x))) / 3);
            string uni = GetUnit(unit);
            int b = ((int)((lhs.y + Math.Log10(Mathf.Abs(lhs.x))) / 3)) * 3;
            double a = lhs.x * Math.Pow(10, lhs.y - b);
            if (!integer)
                return a + uni;
            string aa = Mathf.RoundToInt((float)a).ToString();
            return aa + uni;
        }

        public string GetValue(Long2 lhs,int length)
        {
            if (lhs.x == 0)
                return "0";
            int unit = (int)((lhs.y + Math.Log10(Mathf.Abs(lhs.x))) / 3);
            string uni = GetUnit(unit);
            int b = ((int)((lhs.y + Math.Log10(Mathf.Abs(lhs.x))) / 3)) * 3;
            double a = lhs.x * Math.Pow(10, lhs.y - b);
            string aa = a.ToString();
            length = Math.Min(length, aa.Length);
            //如果取值的最后一位是小数点，则向后多取一位
            int index = aa.IndexOf('.');
            if (index == length - 1)
                length++;
            aa = aa.Substring(0, length);  
            return aa + uni;
        }

        private string _uni;
        private string _aa;
        /// <summary>
        /// 取小数点后count位数值
        /// </summary>
        /// <returns></returns>
        public string GetFloatValue(Long2 lhs,int count)
        {
            if (lhs.x == 0)
                return "0";
            int unit = (int)((lhs.y + Math.Log10(Mathf.Abs(lhs.x))) / 3);
            _uni = GetUnit(unit);
            int b = ((int)((lhs.y +  Math.Log10(Mathf.Abs(lhs.x))) / 3)) * 3;
            double a = lhs.x * Math.Pow(10, lhs.y - b);
            a *= Math.Pow(10, count);
            a = Mathf.RoundToInt((float)a);
            a /= Math.Pow(10, count);
            //如果取值的最后一位是小数点，则向后多取一位
            _aa = a.ToString();
            string[] valueStr = _aa.Split('.');
            if (valueStr.Length <= 1)
                return _aa + _uni;
            int length = valueStr[0].Length + Math.Min(count, valueStr[1].Length) + 1;
            _aa = _aa.Substring(0, length);
            return _aa + _uni;
        }

    }
}

