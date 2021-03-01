using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Spine.Unity;
using BehaviorDesigner.Runtime;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("等待")]
    [TaskIcon("{SkinColor}WaitIcon.png")]
    public class Wait : Action
    {
        public bool _flipX;
        private MeshRenderer _meshRenderer;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The amount of time to wait")]
        public SharedFloat waitTime = 1;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the wait be randomized?")]
        public SharedBool randomWait = false;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum wait time if random wait is enabled")]
        public SharedFloat randomWaitMin = 1;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum wait time if random wait is enabled")]
        public SharedFloat randomWaitMax = 1;

        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;

        public override void OnStart()
        {
            // Remember the start time.
            startTime = Time.time;
            if (randomWait.Value)
            {
                waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }
            else
            {
                waitDuration = waitTime.Value;
            }
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
            // The task is done waiting if the time waitDuration has elapsed since the task was started.
            if (startTime + waitDuration < Time.time)
            {
                return TaskStatus.Success;
            }
            // Otherwise we are still waiting.
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                // Remember the time that the behavior was paused.
                pauseTime = Time.time;
            }
            else
            {
                // Add the difference between Time.time and pauseTime to figure out a new start time.
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            waitTime = 1;
            randomWait = false;
            randomWaitMin = 1;
            randomWaitMax = 1;
        }
        private void OrderLayer()
        {
            _meshRenderer.sortingOrder = -(int)(transform.position.y * 10);
        }
    }
}


