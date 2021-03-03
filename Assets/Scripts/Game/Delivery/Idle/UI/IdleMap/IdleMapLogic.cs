using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{
    public class IdleMapLogic : Singleton<IdleMapLogic>
    {
        private PlayerMgr playerMgr;
        private IdleCityMgr cityMgr;
        private List<IdleCity> allCities;
        public IdleMapLogic()
        {
            allCities = new List<IdleCity>();
            playerMgr = PlayerMgr.Instance;
            cityMgr = IdleCityMgr.Instance;
        }

        public void InitData()
        {
            allCities.Clear();
            Dictionary<int, IdleCity> allCitiesDic = IdleCityMgr.Instance.GetAllIdleCity();
            var enumerator = allCitiesDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                allCities.Add(enumerator.Current.Value);
            }
        }

        //private int Compare(IdleCity city1,IdleCity city2)
        //{
        //    return city1.Id < city2.Id ? -1 : 1;
        //}
      

        public IdleCity GetNextCity()
        {
            int nextCityId = playerMgr.CityId + 1;
            return cityMgr.GetIdleCityById(nextCityId);
        }

        public IdleCity GetIdleCityByIndex(int index)
        {
            if (index < 0 || index >= allCities.Count) return null;
            return allCities[index];
        }

        public int GetAllCityCount()
        {
            return allCities.Count;
        }

        public bool IsCityUnlock(IdleCity idleCity)
        {
            PlayerCity playerCity = playerMgr.GetPlayerCity(idleCity.Id);
            return playerCity != null;
        }
        /// <summary>
        /// 解锁城市
        /// </summary>
        public void UnlockCity()
        {
            IdleCity nextCity = GetNextCity();
            if (nextCity == null) return;
            Long2 cost = new Long2(nextCity.unlockPrice);
            if (playerMgr.PlayerCity.money < cost) return;
            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            //GameObject.FindObjectOfType<GameLauncher>().NextScene(nextCity.Id);
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_UnlockCity));
        }
        
        public bool PlayerCanPay()
        {
            Long2 unlockCost = new Long2(GetNextCity().unlockPrice);
            return playerMgr.PlayerCity.money >= unlockCost;
        }

        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }

        public void OnUnlockBtnClick()
        {
            UnlockCity();
        }
    }
}

