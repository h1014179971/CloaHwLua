using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class UIFx : MonoBehaviour
    {

        public virtual void Init(object args = null)
        {

        }
        public virtual void Open(object args = null)
        {
            
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
            
        }  
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public virtual void Close()
        {
            Destroy(gameObject);
        }
    }
}

