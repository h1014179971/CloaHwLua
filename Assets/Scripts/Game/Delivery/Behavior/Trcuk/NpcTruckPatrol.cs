using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using BehaviorDesigner.Runtime;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior/NPCTruck")]
    [TaskDescription("Npc巡逻")]
    public class NpcTruckPatrol : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走目标点")]
        public List<Transform> _targets;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("行走速度")]
        public float _speed;
        public SharedObject _upSprite;
        public SharedObject _downSprite;
        private Sprite _spriteUp;//朝上图片
        private Sprite _spriteDown;//朝下图片
        private Vector3 _speedV3;
        private int _index = 0;
        private Vector3 _targetPos;
        private bool _isBack;
        private SpriteRenderer _sprite;
        private Animation _animation;
        public override void OnStart()
        {
            _isBack = false;
            _index = 0;
            _targetPos = _targets[_index].position;
            _sprite = Owner.transform.Find("truck").GetComponent<SpriteRenderer>();
            Debug.Log($"_sprite==={_sprite}");
            _spriteUp = _upSprite.Value as Sprite;
            _spriteDown = _downSprite.Value as Sprite;
            if (_spriteUp == null || _spriteDown == null)
                LogUtility.LogError($"NPC 车辆图片为空up{_spriteUp},down{_spriteDown}");
            _sprite.sprite = _spriteUp;
            _animation = Owner.transform.Find("truck").GetComponent<Animation>();
            _animation.Play();
            SetTarget(_targetPos);
            OrderLayer();
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
                if (_index >= _targets.Count && !_isBack)
                {
                    _isBack = true;
                    _index = _targets.Count - 1;
                }
                else if (_index <= 0 && _isBack)
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
                _sprite.sortingOrder = -(int)(transform.position.y * 10);
            }).AddTo(Owner.gameObject);
        }
    }
}

