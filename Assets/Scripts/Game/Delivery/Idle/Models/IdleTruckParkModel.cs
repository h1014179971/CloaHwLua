using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle 
{
    public class IdleTruckParkModel : MonoBehaviour
    {
        [SerializeField]private int _itemId;
        public int ItemId { get { return _itemId; } }
    }
}


