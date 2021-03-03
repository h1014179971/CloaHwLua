using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdlePostSiteItemModel : MonoBehaviour
    {
        public int index;
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}


