using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UIFramework
{
    public class ScrollRectItem : MonoBehaviour
    {

        public ScrollRectEx _scroller;
        public int _index;

        public virtual int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                transform.localPosition = _scroller.GetPosition(_index);
            }
        }
        public virtual void Init() { }
        public virtual void CreateItem() { }
        public ScrollRectEx Scroller
        {
            set { _scroller = value; }
        }
    }
}


