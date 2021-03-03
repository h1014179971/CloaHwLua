using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Foundation;
using Spine.Unity;
using DG.Tweening;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("淡出淡入")]
    public class Fade : Action
    {
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        private SpriteRenderer _shadow;
        private Color _shadowColor;
        public float _startA;
        public float _endA;
        public float _t;
        private float _offset;
        public override void OnStart()
        {
            SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeleton = skeletonAnimation.Skeleton;
            _shadow = transform.GetComponentByPath<SpriteRenderer>("shadow");
            if (_shadow != null)
                _shadowColor = _shadow.color;
            _skeleton.A = _startA;
            _offset = Mathf.Abs(_endA - _startA);
        }

        public override TaskStatus OnUpdate()
        {
            if (_startA <= _endA)
            {
                return FadeOut();
            }
            else
                return FadeIn();
        }
        private TaskStatus FadeOut()
        {
            if(_skeleton.A >= _endA)
            {
                _skeleton.A = _endA;

                _shadowColor.a = _skeleton.A;
                if (_shadow != null)
                    _shadow.color = _shadowColor;
                return TaskStatus.Success;
            }
            _skeleton.A += Mathf.Lerp(0, Mathf.Min(_offset/ _t * Time.deltaTime, _endA - _skeleton.A), 1f);
            _shadowColor.a = _skeleton.A;
            if (_shadow != null)
                _shadow.color = _shadowColor;
            return TaskStatus.Running;
        }
        private TaskStatus FadeIn()
        {
            if (_skeleton.A <= _endA)
            {
                _skeleton.A = _endA;
                _shadowColor.a = _skeleton.A;
                if (_shadow != null)
                    _shadow.color = _shadowColor;
                return TaskStatus.Success;
            }
            _skeleton.A -= Mathf.Lerp(0, Mathf.Min(_offset / _t * Time.deltaTime, _skeleton.A - _endA), 1f);
            _shadowColor.a = _skeleton.A;
            if (_shadow != null)
                _shadow.color = _shadowColor;
            return TaskStatus.Running;
        }
    }
}

