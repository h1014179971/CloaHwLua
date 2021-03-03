using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    public delegate void TouchEventHandler(Vector3 touchPos);
    public abstract class BaseTouch : MonoBehaviour
    {
        public event TouchEventHandler onTouchBegan;
        public event TouchEventHandler onTouchMoved;
        public event TouchEventHandler onTouchEnded;    
        // Start is called before the first frame update
        public virtual void Start()
        {
            onTouchBegan = new TouchEventHandler(OnTouchBegan);
            onTouchMoved = new TouchEventHandler(OnTouchMoved);
            onTouchEnded = new TouchEventHandler(OnTouchEnded);       
        }

        // Update is called once per frame
        public virtual void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
#else
            if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
            {
                OnTouchBegan(Input.mousePosition);
            }
#if UNITY_EDITOR
            else if (Input.GetMouseButton(0))
#else
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved )
#endif
            {
                OnTouchMoved(Input.mousePosition);
            }
#if UNITY_EDITOR
            else if (Input.GetMouseButtonUp(0))
#else
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
#endif
            {
                OnTouchEnded(Input.mousePosition);
            }                      
        }
        public virtual void OnTouchBegan(Vector3 touchPos) { }
        public virtual void OnTouchMoved(Vector3 touchPos) { }
        public virtual void OnTouchEnded(Vector3 touchPos) { }    
    }
}

