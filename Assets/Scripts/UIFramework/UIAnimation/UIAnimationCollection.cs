using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    //Move to anchor position animation
    public class MoveToAnchorPosAnimation : UIAnimation
    {
        public MoveToAnchorPosAnimation(){}
        private Vector2 _targetPos;

        public MoveToAnchorPosAnimation(RectTransform rt, Vector2 point, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            _targetPos = point;
        }

        protected override void DoAnimation()
        {
            // Move animation
            _UIRectTransform.DOAnchorPos(_targetPos, _animationDuration)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    //Move to loacl position animation
    public class MoveToLocalPosAnimation : UIAnimation
    {
        public MoveToLocalPosAnimation() { }
        private Vector2 _targetPos;

        public MoveToLocalPosAnimation(RectTransform rt, Vector2 point, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            _targetPos = point;
        }

        protected override void DoAnimation()
        {
            // Move animation
            _UIRectTransform.DOLocalMove(_targetPos, _animationDuration)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }
    
    // Move left animation
    public class MoveLeftFromRightAnimation : UIAnimation
    {
        public MoveLeftFromRightAnimation() { }
        public MoveLeftFromRightAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {

        }

        protected override void DoAnimation()
        {
            // Move rt right
            float pW = _UIRectTransform.rect.width;
            _UIRectTransform.anchoredPosition = new Vector2(_InitAnchoredPos.x + pW, _InitAnchoredPos.y);

            // Move left animation
            _UIRectTransform.DOAnchorPosX(_InitAnchoredPos.x, _animationDuration)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }

    }

    // Move right animation
    public class MoveRightFromLeftAnimation : UIAnimation
    {
        public MoveRightFromLeftAnimation() { }
        public MoveRightFromLeftAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            // Move rt right
            float pW = _UIRectTransform.rect.width;
            _UIRectTransform.anchoredPosition = new Vector2(_InitAnchoredPos.x - pW, _InitAnchoredPos.y);

            // Move left animation
            _UIRectTransform.DOAnchorPosX(_InitAnchoredPos.x, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                    .OnComplete(InvokeCallback);
        }
    }

    // Move up animation
    public class MoveUpFromDownAnimation : UIAnimation
    {
        public MoveUpFromDownAnimation() { }
        public MoveUpFromDownAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            
        }

        protected override void DoAnimation()
        {
            // Move rt down
            float pH = _UIRectTransform.rect.height;

            _UIRectTransform.anchoredPosition = new Vector2(_InitAnchoredPos.x, _InitAnchoredPos.y - pH);
            // Move up animation
            _UIRectTransform.DOAnchorPosY(_InitAnchoredPos.y, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Move down animation
    public class MoveDownFromUpAnimaiton : UIAnimation
    {
        public MoveDownFromUpAnimaiton() { }
        public MoveDownFromUpAnimaiton(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            // Move rt right
            float pH = _UIRectTransform.rect.height;
            _UIRectTransform.anchoredPosition = new Vector2(_InitAnchoredPos.x, _InitAnchoredPos.y + pH);

            // Move left animation
            _UIRectTransform.DOAnchorPosY(_InitAnchoredPos.y, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Hide Left animation
    public class MoveLeftAnimaion : UIAnimation
    {
        protected float _percent = 1f;
        public MoveLeftAnimaion() { }
        public MoveLeftAnimaion(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            if (percent > 1)
            {
                percent = 1f;
            }
            else if (percent < 0)
            {
                percent = 0f;
            }
            _percent = percent;
        }

        protected override void DoAnimation()
        {
            Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            float pW = _UIRectTransform.rect.width * _percent;

            // Move left animation
            _UIRectTransform.DOAnchorPosX(rtAnchoredPos.x - pW, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Hide Right animation
    public class MoveRightAnimaion : UIAnimation
    {
        protected float _percent = 1;

        public MoveRightAnimaion() { }
        public MoveRightAnimaion(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            if (percent > 1)
            {
                percent = 1f;
            }
            else if (percent < 0)
            {
                percent = 0f;
            }
            _percent = percent;
        }

        protected override void DoAnimation()
        {
            Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            float pW = _UIRectTransform.rect.width * _percent;

            // Move right animation
            _UIRectTransform.DOAnchorPosX(rtAnchoredPos.x + pW, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                    .OnComplete(InvokeCallback);
        }
    }

    // Move up animation
    public class MoveUpAnimation : UIAnimation
    {
        protected float _percent = 1f;

        public MoveUpAnimation() { }
        public MoveUpAnimation(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            if (percent > 1)
            {
                percent = 1f;
            }
            else if (percent < 0)
            {
                percent = 0f;
            }
            _percent = percent;
        }

        protected override void DoAnimation()
        {
            Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            float pH = _UIRectTransform.rect.height * _percent;

            _UIRectTransform.DOAnchorPosY(rtAnchoredPos.y + pH, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Move down animation
    public class MoveDownAnimaiton : UIAnimation
    {
        protected float _percent = 1f;

        public MoveDownAnimaiton() { }
        public MoveDownAnimaiton(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            if (percent > 1)
            {
                percent = 1f;
            }
            else if (percent < 0)
            {
                percent = 0f;
            }
            _percent = percent;
        }

        protected override void DoAnimation()
        {
            Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            float pH = _UIRectTransform.rect.height * _percent;

            _UIRectTransform.DOAnchorPosY(rtAnchoredPos.y - pH, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }


    // Hide Left animation
    public class MoveLeftFromCenterAnimaion : MoveLeftAnimaion
    {
        public MoveLeftFromCenterAnimaion() { }
        public MoveLeftFromCenterAnimaion(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, percent, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            //Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            //float pW = _UIRectTransform.rect.width;
            _UIRectTransform.anchoredPosition = UIVariable.ScreenCenterPos;
            base.DoAnimation();
            // Move left animation
            //_UIRectTransform.DOAnchorPosX(rtAnchoredPos.x - pW, UIAnimationSpeed.NORMAL)
                //.SetEase(_Ease)
                    //.OnComplete(InvokeCallback);
        }
    }

    // Hide Right animation
    public class MoveRightFromCenterAnimaion : MoveRightAnimaion
    {
        public MoveRightFromCenterAnimaion() { }
        public MoveRightFromCenterAnimaion(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, percent, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            //Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            //float pW = _UIRectTransform.rect.width;
            _UIRectTransform.anchoredPosition = UIVariable.ScreenCenterPos;
            base.DoAnimation();
            // Move right animation
            //_UIRectTransform.DOAnchorPosX(rtAnchoredPos.x + pW, UIAnimationSpeed.NORMAL)
                //.SetEase(_Ease)
                    //.OnComplete(InvokeCallback);
        }
    }

    // Move up animation
    public class MoveUpFromCenterAnimation : MoveUpAnimation
    {
        public MoveUpFromCenterAnimation() { }
        public MoveUpFromCenterAnimation(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, percent, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            float pH = _UIRectTransform.rect.height;
            _UIRectTransform.anchoredPosition = UIVariable.ScreenCenterPos;

            _UIRectTransform.DOAnchorPosY(rtAnchoredPos.y + pH, UIAnimationSpeed.NORMAL)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Move down animation
    public class MoveDownFromCenterAnimaiton : MoveDownAnimaiton
    {
        public MoveDownFromCenterAnimaiton() { }
        public MoveDownFromCenterAnimaiton(RectTransform rt, float percent = 1f, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, percent, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            //Vector2 rtAnchoredPos = _UIRectTransform.anchoredPosition;
            //float pH = _UIRectTransform.rect.height;
            _UIRectTransform.anchoredPosition = UIVariable.ScreenCenterPos;
            base.DoAnimation();
            //_UIRectTransform.DOAnchorPosY(rtAnchoredPos.y - pH, UIAnimationSpeed.NORMAL)
                //.SetEase(_Ease)
                //.OnComplete(InvokeCallback);
        }
    }

    public class MoveToCenterAnimation : UIAnimation
    {
        public MoveToCenterAnimation() {}
        public MoveToCenterAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            
        }

        protected override void DoAnimation()
        {
            Debug.Log(UIVariable.ScreenCenterPos);
            Vector2 sizeHalf = new Vector2(_UIRectTransform.rect.width / 2, _UIRectTransform.rect.height / 2);
            _UIRectTransform.DOAnchorPos(UIVariable.ScreenCenterPos - sizeHalf, _animationDuration)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    public class MoveToInitPosAnimation : UIAnimation
    {
        public MoveToInitPosAnimation() { }
        public MoveToInitPosAnimation(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {

        }

        protected override void DoAnimation()
        {
            _UIRectTransform.DOAnchorPos(_InitAnchoredPos, _animationDuration)
                .SetEase(_Ease)
                .OnComplete(InvokeCallback);
        }
    }

    // Stay
    public class StayAnimaiton : UIAnimation
    {
        public StayAnimaiton() { }
        public StayAnimaiton(RectTransform rt, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
        }

        protected override void DoAnimation()
        {
            DOVirtual.DelayedCall(_animationDuration, InvokeCallback);
        }
    }

    public class FloatUpAndDownAnimation : UIAnimation
    {
        public FloatUpAndDownAnimation() { }
        private float _amplitude;
        public FloatUpAndDownAnimation(RectTransform rt, float amplitude, Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) : base(rt, ease, duration)
        {
            _amplitude = amplitude;
        }

        protected override void DoAnimation()
        {
            Vector3 originPosition = _UIRectTransform.localPosition;
            Vector3[] path = { originPosition + new Vector3(0, _amplitude, 0), originPosition - new Vector3(0, _amplitude, 0), originPosition };
            _UIRectTransform.DOLocalPath(path, _animationDuration).SetEase(_Ease).SetLoops(-1);
        }
    }
    
    public class FadeInOutAnimation : UIAnimation
    {
        CanvasGroup _canvasGroup;
        float _startValue;
        float _endValue;

        public FadeInOutAnimation(CanvasGroup canvasGroup, float startValue, float endValue, float duration = UIAnimationSpeed.NORMAL)
        {
            _canvasGroup = canvasGroup;
            _startValue = startValue;
            _endValue = endValue;
            _animationDuration = duration;
        }

        protected override void DoAnimation()
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.alpha = _startValue;
            _canvasGroup.DOFade(_endValue, _animationDuration)
                                   .SetEase(Ease.Linear)
                                   .OnComplete(InvokeCallback);
        }
    }

    public class ScaleInOutAnimation : UIAnimation
    {
        float _startValue;
        float _endValue;

        public ScaleInOutAnimation(RectTransform rt, float startValue, float endValue, Ease ease = Ease.OutElastic, float duration = UIAnimationSpeed.NORMAL) : 
            base(rt, ease, duration)
        {
            _startValue = startValue;
            _endValue = endValue;
        }

        protected override void DoAnimation()
        {
            if (_UIRectTransform == null)
                return;

            _UIRectTransform.localScale = Vector3.one * _startValue;
            _UIRectTransform.DOScale(_endValue, _animationDuration)
                                        .SetEase(_Ease)
                                        .OnComplete(InvokeCallback);
        }
    }
}