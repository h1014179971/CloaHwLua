using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.TrackEditor
{                                                    
    public delegate void TouchEventHandler(Vector3 touchPos);
    public abstract class TrackEditorBaseTouch : MonoBehaviour
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
            if (Input.GetMouseButtonDown(0))    
            {
                OnTouchBegan(Input.mousePosition);
            }    
            else if (Input.GetMouseButton(0)) 
            {
                OnTouchMoved(Input.mousePosition);
            }    
            else if (Input.GetMouseButtonUp(0)) 
            {
                OnTouchEnded(Input.mousePosition);
            }
        }
        public virtual void OnTouchBegan(Vector3 touchPos) { }
        public virtual void OnTouchMoved(Vector3 touchPos) { }
        public virtual void OnTouchEnded(Vector3 touchPos) { }
    }
}

