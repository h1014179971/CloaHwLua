using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public class ItemLittleModel : MonoBehaviour
    {
        private RectTransform rectTrans;
        public int Id { get; set; }
        public RectTransform RectTrans
        {
            get
            {
                if (rectTrans == null)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
            }
        }
    }
}


