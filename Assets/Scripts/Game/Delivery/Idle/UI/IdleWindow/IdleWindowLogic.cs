using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
namespace Delivery.Idle
{
    public class IdleWindowLogic : Singleton<IdleWindowLogic>
    {

        #region UI响应事件
        /// <summary>
        /// 统计按钮点击事件
        /// </summary>
        public void OnStatisticsBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleStatisticsWindow>( "IdleStatisticsWindow.prefab");
        }
      
        /// <summary>
        /// 地图按钮点击事件
        /// </summary>
        public void OnMapBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleMapWindow>( "IdleMapWindow.prefab");
        }

        /// <summary>
        /// 建造按钮点击事件
        /// </summary>
        public void OnBuildBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleBuildingWindow>("IdleBuildingWindow.prefab");
        }
        /// <summary>
        /// 任务按钮点击事件
        /// </summary>
        public void OnTaskBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleTaskWindow>( "IdleTaskWindow.prefab");
        }
        /// <summary>
        /// 特殊时间按钮点击事件
        /// </summary>
        public void OnEventBtnClick()
        {
            IdleEventCtrl.Instance.IsEventTrigger = false;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleEventWindow>( "IdleEventWindow.prefab");
        }
        /// <summary>
        /// 投资人按钮点击事件
        /// </summary>
        public void OnInvestorBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleInvestorWindow>("IdleInvestorWindow.prefab");
        }
        /// <summary>
        /// 双倍收益按钮点击事件
        /// </summary>
        public void OnDoubleIncomeBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleDoubleIncomeWindow>("IdleDoubleIncomeWindow.prefab");
        }


        /// <summary>
        /// 站点点击事件
        /// </summary>
        public void OnSiteBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleSiteInfoWindow>( "IdleSiteInfoWindow.prefab");
        }

        /// <summary>
        /// 驿站点击事件
        /// </summary>
        public void OnPostBtnClick(BaseEventArgs baseEventArgs)
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            IdlePostSiteWindow postSiteWindow = UIController.Instance.OpenWindowFromAsset<IdlePostSiteWindow>( "IdlePostSiteWindow.prefab");
            EventArgsOne<IdleSiteModel> argsOne = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            postSiteWindow.InitData(argsOne.param1);
        }

     
        /// <summary>
        /// 车站点击事件
        /// </summary>
        public void OnTruckStationBtnClick()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));
            UIController.Instance.OpenWindowFromAsset<IdleTruckInfoWindow>("IdleTruckInfoWindow.prefab");
        }

       

        #endregion



    }
}


