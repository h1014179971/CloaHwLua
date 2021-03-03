using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Spine.Unity;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("待机")]
    //[TaskIcon("Assets/3rd/Behavior Designer/Editor/{SkinColor}SeekIcon.png")]
    public class Idle : Action
    {
        public bool _flipX;
        private MeshRenderer _meshRenderer;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        public override void OnStart()
        {
            SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
            _spineAnimationState = skeletonAnimation.AnimationState;
            skeletonAnimation.loop = true;
            _skeleton = skeletonAnimation.Skeleton;
            Spine.Animation animation = _skeleton.Data.FindAnimation("idle");
            if (animation == null)
            {
                Debug.LogError($"{Owner.transform.parent.name}没有idle动作");
            }
            else
            {
                _spineAnimationState.SetAnimation(0, "idle", true);
                if (!_flipX)
                    _skeleton.ScaleX = 1;
                else
                    _skeleton.ScaleX = -1;
                _meshRenderer = GetComponent<MeshRenderer>();
                OrderLayer();
            }
            
        }
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
        private void OrderLayer()
        {
            _meshRenderer.sortingOrder = -(int)(transform.position.y * 10);
        }
    }
}

