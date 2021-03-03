using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdlePlaneModel : MonoBehaviour
    {
        Animation _animation;
        // Start is called before the first frame update
        void Start()
        {
            _animation = GetComponent<Animation>();
            
        }
        public void Play()
        {
            _animation?.Play("plane");
        }
        public void FeijiDown()
        {
            //IdleCityCtrl.Instance.PlaneUnLoadItem();
        }
    }
}

