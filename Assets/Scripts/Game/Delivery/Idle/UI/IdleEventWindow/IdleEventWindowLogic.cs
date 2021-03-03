using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
namespace Delivery.Idle
{
    public class IdleEventWindowLogic : Singleton<IdleEventWindowLogic>
    {
        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }
        public void EndSpecialEvent()
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_SpecialEvent_ToEnd));
        }

        public void StartSpecialEvent()
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_SpecialEvent_Start));
        }

        public IdleSpecialEvent GetCurrentEvent()
        {
            return IdleEventCtrl.Instance.GetCurrentEvent();
        }

    }
}


