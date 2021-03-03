using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
namespace Delivery.Idle
{
    public class IdleCityInfoLogic : Singleton<IdleCityInfoLogic>
    {
        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }
    }
}


