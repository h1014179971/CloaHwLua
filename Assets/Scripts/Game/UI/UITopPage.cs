using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;

namespace Delivery
{
    public class UITopPage : UIPage
    {
        private RectTransform bg; 
        void Start()
        {
            InitPage();
        }

        protected override void InitPage(object args = null)
        {   
            bg = transform.Find("bg").GetComponent<RectTransform>();
            bg.offsetMax = new Vector2(0, ((Screen.safeArea.height + Screen.safeArea.y) - Screen.height) / RootCanvas.Instance.AdapterFit());
            
        }
    }
}

