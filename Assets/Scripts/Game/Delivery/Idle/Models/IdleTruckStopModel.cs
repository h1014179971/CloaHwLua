using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    //卡车停车点（包括站点停车位和驿站停车位）
    public class IdleTruckStopModel : MonoBehaviour
    {
        [SerializeField]private int _rowId; //行id，主要用了等待区停车场
        private Transform _child;
        public int RowId { get { return _rowId; } }
        public void Init()
        {
            _child = transform.Find("child");
        }
        public Vector3 GetPoint()
        {
            return transform.position;
        }
        public Vector3 GetChildPoint()
        {
            return _child.position;
        }
        public Vector3 GetVector()
        {
            return _child.position - transform.position;
        }
    }
}

