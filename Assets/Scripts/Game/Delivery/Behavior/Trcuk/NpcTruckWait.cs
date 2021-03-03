using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using Foundation;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior/NPCTruck")]
    [TaskDescription("Npc等待")]
    public class NpcTruckWait : Action
    {
        public bool _isUp;
        public SharedObject _upSprite;
        public SharedObject _downSprite;
        private Sprite _spriteUp;//朝上图片
        private Sprite _spriteDown;//朝下图片
        private SpriteRenderer _sprite;
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
            
            _sprite = Owner.transform.Find("truck").GetComponent<SpriteRenderer>();
            _spriteUp = _upSprite.Value as Sprite;
            _spriteDown = _downSprite.Value as Sprite;
            if (_spriteUp == null || _spriteDown == null)
                LogUtility.LogError($"NPC 车辆图片为空up{_spriteUp},down{_spriteDown}");
            if (_isUp)
                _sprite.sprite = _spriteUp;
            else
                _sprite.sprite = _spriteDown;
            _sprite.sortingOrder = -(int)(transform.position.y * 10);
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
    }
}

