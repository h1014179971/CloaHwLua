using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{
    public class IdleInvestorLogic : Singleton<IdleInvestorLogic>
    {
        public void CloseCurrentWindow()
        {
            UIController.Instance.CloseCurrentWindow();
        }

        public Long2 GetIncome()
        {
            return IdleInvestorCtrl.Instance.GetIncome();
        }

        public void RestartInvestor()
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Investor_Restart));
        }

        public void EndInvestor()
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Investor_End));
        }
    }
}

