
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery 
{
    public class BackgroundAdapter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            CanvasScaler _scaler = RootCanvas.Instance.UICanvas.GetComponent<CanvasScaler>();
            if (_scaler == null) _scaler = GetComponentInParent<CanvasScaler>();

            var resolution = _scaler.referenceResolution;
            var rt = _scaler.transform as RectTransform;
            if (rt == null) return;
            var screenSize = rt.sizeDelta;
            var factor = Mathf.Max(screenSize.x / resolution.x, screenSize.y / resolution.y);
            var scale = Vector3.one * factor;
            transform.localScale = scale;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


