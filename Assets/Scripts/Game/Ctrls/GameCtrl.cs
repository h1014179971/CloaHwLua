using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

namespace Delivery
{
    public class GameCtrl : MonoBehaviour
    {
        private void Awake()
        {
            UIController.Instance.OpenPage<UIStagePage>(UIPrefabPath.UI_StagePage);
        }
    }
}

