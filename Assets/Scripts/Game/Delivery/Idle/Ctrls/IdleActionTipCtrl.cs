using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{

    public class IdleActionTipCtrl : MonoSingleton<IdleActionTipCtrl>
    {
        private bool _toLevelUpSite;//是否需要升级快递中心
        private bool _toLevelUpTruckStation;//是否需要升级车站
        private bool _toLevelUpPostSite;//是否需要升级驿站
        private int _postSiteId;//需要升级的驿站id
        private int _actionType;
        public void Init()
        {
            _toLevelUpPostSite = false;
            _toLevelUpSite = false;
            _toLevelUpTruckStation = false;
            _actionType = -1;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_NeedToLevelUpSite, NeedToLevelUpSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_CancelToLevelUpSite, CancelToLevelUpSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_NeedToLevelUpPostSite, NeedToLevelUpPostSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_CancelToLevelUpPostSite, CancelToLevelUpPostSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_NeedToLevelUpTruck, NeedToLevelUpTruckStation);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_CancelToLevelUpTruck, CancelToLevelUpTruckStation);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_NeedToLevelUpSite, NeedToLevelUpSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_CancelToLevelUpSite, CancelToLevelUpSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_NeedToLevelUpPostSite, NeedToLevelUpPostSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_CancelToLevelUpPostSite, CancelToLevelUpPostSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_NeedToLevelUpTruck, NeedToLevelUpTruckStation);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_CancelToLevelUpTruck, CancelToLevelUpTruckStation);
        }

        #region 获取属性
        public bool ToLevelUpSite
        {
            get
            {
                return _toLevelUpSite;
            }
        }
        public bool ToLevelUpTruckStation
        {
            get
            {
                return _toLevelUpTruckStation;
            }
        }
        public bool ToLevelUpPostSite
        {
            get
            {
                return _toLevelUpPostSite;
            }
        }
        //是否存在任何一个操作提示
        public bool IsAnyActionTip
        {
            get
            {
                return _toLevelUpPostSite || _toLevelUpSite || _toLevelUpTruckStation;
            }
        }
        #endregion

        private void NeedToLevelUpSite(BaseEventArgs baseEventArgs)
        {
            bool lastValue = _toLevelUpSite;
            _toLevelUpSite = true;

            if (!lastValue)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_ActionTip_Trigger, 2));
        }

        private void CancelToLevelUpSite(BaseEventArgs baseEventArgs)
        {
            _toLevelUpSite = false;
        }


        private void NeedToLevelUpTruckStation(BaseEventArgs baseEventArgs)
        {
            bool lastValue = _toLevelUpTruckStation;
            _toLevelUpTruckStation = true;
            if (!lastValue)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_ActionTip_Trigger, 3));
        }


        private void CancelToLevelUpTruckStation(BaseEventArgs baseEventArgs)
        {
            _toLevelUpTruckStation = false;
        }


        private void NeedToLevelUpPostSite(BaseEventArgs baseEventArgs)
        {
            if (_toLevelUpPostSite) return;
            EventArgsOne<int> args = (EventArgsOne<int>)baseEventArgs;
            bool lastValue = _toLevelUpPostSite;
            _toLevelUpPostSite = true;
            _postSiteId = args.param1;
            if (!lastValue)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_ActionTip_Trigger, 1));
        }


        private void CancelToLevelUpPostSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> args = (EventArgsOne<int>)baseEventArgs;
            if (_postSiteId != args.param1) return;
            _toLevelUpPostSite = false;
            _postSiteId = -1;
        }


        public void OpenTargetWindow(int type)
        {
            _actionType = type;

            if (_actionType < 0) return;
            switch (_actionType)
            {
                case 1:
                    IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(_postSiteId);
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
                    _toLevelUpPostSite = false;
                    _postSiteId = -1;
                    break;
                case 2:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
                    _toLevelUpSite = false;
                    break;
                case 3:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
                    _toLevelUpTruckStation = false;
                    break;
            }
            _actionType = -1;
            
        }

        //private void OnCameraArrive(BaseEventArgs baseEventArgs)
        //{
        //    if (_actionType < 0) return;
        //    switch (_actionType)
        //    {
        //        case 1:
        //            IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(_postSiteId);
        //            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
        //            _postSiteId = -1;
        //            break;
        //        case 2:
        //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
        //            break;
        //        case 3:
        //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
        //            break;
        //    }
        //    _actionType = -1;
        //}

    }

}


