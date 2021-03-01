using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle 
{
    public class IdleNpcModel : MonoBehaviour
    {
        [SerializeField] private List<string> _talks = new List<string>();
        public List<string> Talks { get { return _talks; } }
    }
}


