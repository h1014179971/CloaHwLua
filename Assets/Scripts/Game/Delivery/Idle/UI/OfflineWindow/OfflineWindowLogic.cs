using System;
using UnityEngine;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{
    public class OfflineWindowLogic : Singleton<OfflineWindowLogic>
    {
        /// <summary>
        /// 获取离开时间
        /// </summary>
        public string GetLeaveTime()
        {
            //DateTime time = DateTime.Now;
            //long nowTime = TimeUtils.ConvertLongDateTime(time);
            //PlayerMgr playerMgr = PlayerMgr.Instance;
            //PlayerCity playerCity = playerMgr.GetPlayerCity(playerMgr.CityId);
            //long leaveTime = playerCity.leaveTime;
            //long t = nowTime - leaveTime;
            long t = PlayerMgr.Instance.GetLeaveCityTime();
            return GetLeaveTime((int)t);
        }

        private string GetLeaveTime(int totalSecond)
        {
            int day = totalSecond / (24 * 3600);
            int hour = Mathf.CeilToInt((totalSecond % (24 * 3600)) / 3600);
            int minute = Mathf.CeilToInt((totalSecond % (24 * 3600)) % 3600 / 60);
            if(day>0)
            {
                return $"{day}天{hour}小时";
            }
            if(hour>0)
            {
                return $"{hour}小时{minute}分钟";
            }
            return $"{minute}分钟";
        }
        /// <summary>
        /// 获取离线奖励
        /// </summary>
        public Long2 GetOfflineIncome()
        {
            return PlayerMgr.Instance.OffLineIncome();
        }


    }
}


