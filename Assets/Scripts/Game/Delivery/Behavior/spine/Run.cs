using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Foundation;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("行走")]
    public class Run : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走目标点")]
        public Transform _target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走速度")]
        public float _speed;
        private Vector3 _speedV3;
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
            if(animation == null)
            {
                Debug.LogError($"{Owner.transform.parent.name}没有walk动作");
            }
            else
            {
                _spineAnimationState.SetAnimation(0, "walk", true);
                SetTarget(_target.position);
                _meshRenderer = GetComponent<MeshRenderer>();
                OrderLayer();
            }
            
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speedV3, Time.deltaTime);
            if (v3.magnitude >= (_target.position - transform.position).magnitude)
            {
                transform.position = _target.position;
                _spineAnimationState.SetAnimation(0, "idle", true);
                return TaskStatus.Success;
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

