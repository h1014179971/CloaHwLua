using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Delivery.Test
{
    public class IdleForkModel : MonoBehaviour
    {
        public Transform target;
        private Animation _animation;
        private Seeker _seeker;
        private Vector3 _target;
        private List<Vector3> _vectorPath = new List<Vector3>();
        private Vector3 _nextPoint;
        private Vector3 _offset;
        private int _pathIndex;
        private float _speed = 2;
        private float _speedTimes = 1.0f;//速度倍数
        private int _itemId;//运送货物的id
        private enum ForkState
        {
            Idle,
            Move,
            Back
        }
        private ForkState _forkState;
        public int ItemId { get { return _itemId; } }
        private void Start()
        {
            Init(1, Vector3.one);
        }
        public void Init(int itemId, Vector3 stopPoint)
        {
            _itemId = itemId;
            //transform.position = stopPoint;
            _seeker = GetComponent<Seeker>();
            _seeker.pathCallback += OnPathComplete;
            _forkState = ForkState.Idle;
            _target = target.position;
            _seeker.StartPath(transform.position, _target);
            PointGraph pointGraph = AstarPath.active.data.pointGraph;
        }
        public void OnPathComplete(Path p)
        {
            Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
            if (!p.error)
            {
                _vectorPath = p.vectorPath;
                _vectorPath.Add(_target);
                _pathIndex = 0;
                NextPoint();
                _forkState = ForkState.Move;
            }
        }

        private void NextPoint()
        {
            _nextPoint = _vectorPath[_pathIndex];
            _offset = _nextPoint - transform.position;
        }
        // Update is called once per frame
        void Update()
        {
            if (_forkState == ForkState.Move)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed * _speedTimes, Time.deltaTime);
                if (v3.magnitude > (_nextPoint - transform.position).magnitude)
                    transform.position = _nextPoint;
                else
                    transform.position += v3;
                //if ((_nextPoint - transform.position).sqrMagnitude < _speed * Time.deltaTime * _speed * Time.deltaTime *2/* _moveNextDist*/)//长度平方小于一定值判断到达目标点
                if (_nextPoint == transform.position)
                {
                    _pathIndex++;
                    if (_pathIndex < _vectorPath.Count)
                    {
                        NextPoint();
                    }
                    else
                        _forkState = ForkState.Idle;
                }
            }
        }
        private void OnDestroy()
        {
            if (_seeker != null)
                _seeker.pathCallback -= OnPathComplete;
        }
    }
}

