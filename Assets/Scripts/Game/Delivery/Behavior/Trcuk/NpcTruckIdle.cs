using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Foundation;
using BehaviorDesigner.Runtime;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior/NPCTruck")]
    [TaskDescription("Npc待机")]
    public class NpcTruckIdle : Action
    {
        public bool _isUp;
        public SharedObject _upSprite;
        public SharedObject _downSprite;
        private Sprite _spriteUp;//朝上图片
        private Sprite _spriteDown;//朝下图片
        private SpriteRenderer _sprite;
        public override void OnStart()
        {
            
            _sprite = Owner.transform.Find("truck").GetComponent<SpriteRenderer>();
            _spriteUp = _upSprite.Value as Sprite;
            _spriteDown = _downSprite.Value as Sprite;
            if (_spriteUp == null || _spriteDown == null)
                LogUtility.LogError($"NPC 车辆图片为空up{_spriteUp},down{_spriteDown}");
            if (_isUp)
            {
                _sprite.sprite = _spriteUp;
            }
            else
                _sprite.sprite = _spriteDown;
            OrderLayer();
        }
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
        private void OrderLayer()
        {
            _sprite.sortingOrder = -(int)(transform.position.y * 10);
        }
    }
}

