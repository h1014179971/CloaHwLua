using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorDesigner.Runtime.Tasks;
using Foundation;
using Spine.Unity;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("巡逻")]
    public class Patrol : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走目标点")]
        public List<Transform> _targets;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走速度")]
        public float _speed;
        private Vector3 _speedV3;
        private int _index = 0;
        private Vector3 _targetPos;
        private bool _isBack;
        private MeshRenderer _meshRenderer;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        public override void OnStart()
        {
            SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
            _spineAnimationState = skeletonAnimation.AnimationState;
            skeletonAnimation.loop = true;
            _skeleton = skeletonAnimation.Skeleton;
            Spine.Animation animation = _skeleton.Data.FindAnimation("walk");
            if (animation == null)
            {
                Debug.LogError($"{Owner.transform.parent.name}没有walk动作");
            }
            else
            {
                _spineAnimationState.SetAnimation(0, "walk", true);
                _isBack = false;
                _index = 0;
                _targetPos = _targets[_index].position;
                SetTarget(_targetPos);
                _meshRenderer = GetComponent<MeshRenderer>();
                OrderLayer();
            }
            
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speedV3, Time.deltaTime);
            if (v3.magnitude >= (_targetPos - transform.position).magnitude)
            {
                transform.position = _targetPos;
                if (_isBack)
                    _index--;
                else
                    _index++;
                if(_index >= _targets.Count && !_isBack)
                {
                    _isBack = true;
                    _index = _targets.Count - 1;
                }  
                else if(_index <=0 && _isBack)
                {
                    _isBack = false;
                    _index = 0;
                }
                _targetPos = _targets[_index].position;
                SetTarget(_targetPos);
            }
            else
                transform.position += v3;
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {

        }
        private void SetTarget(Vector3 target)
        {
            _speedV3 = (target - transform.position).normalized * _speed;
            if (target.x < transform.position.x)
                _skeleton.ScaleX = 1;
            else
                _skeleton.ScaleX = -1;
        }
        private void OrderLayer()
        {
            Timer.Instance.Register(1, -1, (pare) => {
                _meshRenderer.sortingOrder = -(int)(transform.position.y * 10);
            }).AddTo(Owner.gameObject);
        }
    }
}

