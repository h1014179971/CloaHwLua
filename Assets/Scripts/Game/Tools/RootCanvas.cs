using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
namespace Delivery
{
    public class RootCanvas : MonoBehaviour
    {
        protected static RootCanvas _Instance = null;
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        public static RootCanvas Instance
        {
            get
            {   
                return _Instance;
            }
        }
        private void Awake()
        {
            if (null == _Instance)
                _Instance = this;
            DontDestroyOnLoad(gameObject);
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<CanvasScaler>();
            RectTrans = GetComponent<RectTransform>();

            float currentAspect = (float)Screen.width / Screen.height;
            float settingAspect = FixScreen.width / FixScreen.height;
            if (currentAspect > settingAspect)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0;
            }

        }
        public Canvas UICanvas { get { return canvas; } }
        public RectTransform RectTrans { get; set; }
        public Camera UICamera { get { return canvas.worldCamera; } }
        public float Match
        {
            get { return canvasScaler.matchWidthOrHeight; }
        } 
        public float AdapterFit()
        {
            if (Match < 0.5f)
                return Screen.width / FixScreen.width;
            return Screen.height / FixScreen.height;
        }
    }
}

