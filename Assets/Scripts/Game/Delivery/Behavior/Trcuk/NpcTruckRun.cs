using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Foundation;
using BehaviorDesigner.Runtime;

namespace Delivery.Behavior 
{
    [TaskCategory("Behavior/NPCTruck")]
    [TaskDescription("Npc行走")]
    public class NpcTruckRun : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走目标点")]
        public Transform _target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走速度")]
        public float _speed = 2;
        public SharedObject _upSprite;
        public SharedObject _downSprite;
        private Sprite _spriteUp;//朝上图片
        private Sprite _spriteDown;//朝下图片
        private Vector3 _speedV3;
        private SpriteRenderer _sprite;
        private Animation _animation;
        public override void OnStart()
        {
            
            _sprite = Owner.transform.Find("truck").GetComponent<SpriteRenderer>();
            _spriteUp = _upSprite.Value as Sprite;
            _spriteDown = _downSprite.Value as Sprite;
            if (_spriteUp == null || _spriteDown == null)
                LogUtility.LogError($"NPC 车辆图片为空up{_spriteUp},down{_spriteDown}");
            _sprite.sprite = _spriteUp;
            _animation = Owner.transform.Find("truck").GetComponent<Animation>();
            _animation.Play();
            SetTarget(_target.position);
            OrderLayer();
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speedV3, Time.deltaTime);
            if (v3.magnitude >= (_target.position - transform.position).magnitude)
            {
                transform.position = _target.position;
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
            if (_speedV3.y <= 0)
            {
                if (_sprite.sprite != _spriteDown)
                    _sprite.sprite = _spriteDown;
                if (_speedV3.x > 0)
                    _sprite.flipX = true;
                else
                    _sprite.flipX = false;
            }
            else
            {
                if (_sprite.sprite != _spriteUp)
                    _sprite.sprite = _spriteUp;
                if (_speedV3.x > 0)
                    _sprite.flipX = false;
                else
                    _sprite.flipX = true;
            }
        }
        private void OrderLayer()
        {
            Timer.Instance.Register(0.2f, -1, (pare) => {
                //if (_sprite == null) return;
                _sprite.sortingOrder = -(int)(transform.position.y * 10);
            }).AddTo(Owner.gameObject);
        }
    }
}


