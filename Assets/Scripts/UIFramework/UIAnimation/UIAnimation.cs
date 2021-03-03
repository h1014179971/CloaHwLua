using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFramework
{
    public struct UIAnimationSpeed
    {
        public const float SUPER_FAST = 0.2f;
        public const float FAST = 0.325f;
        public const float NORMAL = 0.375f;
        public const float SLOW = 0.435f;
    }

    public abstract class UIAnimation
    {
        protected RectTransform _UIRectTransform;
        protected Vector2 _InitAnchoredPos;
        protected float _animationDuration;
        protected Ease _Ease;
        protected UnityEvent _AnimationCallback = new UnityEvent();

        public UIAnimation()
        {
        }

        public UIAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL)
        {
            _UIRectTransform = rt;
            if (_UIRectTransform != null)
            {
                _InitAnchoredPos = _UIRectTransform.anchoredPosition;
            }
            SetDuration(duration);
            SetEase(ease);

            //_AnimationCallback.AddListener(UIController.Instance.HideBlocker);
        }

        public void SetParams(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL)
        {
            _UIRectTransform = rt;

            SetDuration(duration);
            SetEase(ease);

            
        }

        public Ease SetEase(Ease ease)
        {
            _Ease = ease;
            return _Ease;
        }

        public float SetDuration(float d)
        {
            _animationDuration = d;
            return _animationDuration;
        }

        public void Play(UnityEvent callback = null)
        {
            if (callback != null)
            {
                _AnimationCallback.RemoveAllListeners();
                _AnimationCallback.AddListener(callback.Invoke);
            }
            UIController.Instance.ShowBlocker();
            _AnimationCallback.AddListener(UIController.Instance.HideBlocker);
            DoAnimation();
        }

        protected void InvokeCallback()
        {
            _AnimationCallback.Invoke();
        }

        protected abstract void DoAnimation();
    }
}