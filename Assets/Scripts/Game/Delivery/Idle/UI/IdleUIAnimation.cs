using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UIFramework;
using UnityEngine.Events;
using Spine.Unity;

namespace Delivery.Idle
{
    public enum UIAnimationDir
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class IdleUIAnimation
    {
        private Sequence sequence;

        private RectTransform rectTrans;
        private CanvasGroup canvasGroup;
    
        private Vector2 startPos;
        private Vector2 endPos;

        public bool isActive;

        private SkeletonGraphic[] skeletons;

        private UnityEvent OnEnterFinish;
        private UnityEvent OnExitFinish;
        

        public IdleUIAnimation(RectTransform _rectTrans, Vector2 _startPos, Vector2 _endPos, UnityAction _enterCallback=null, UnityAction _exitCallback=null)
        {
            isActive = false;
            rectTrans = _rectTrans;
            canvasGroup = rectTrans.transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = rectTrans.gameObject.AddComponent<CanvasGroup>();
            skeletons = rectTrans.GetComponentsInChildren<SkeletonGraphic>();
            //if(skeletons!=null && skeletons.Length>0)
            //{
            //    for(int i=0;i<skeletons.Length;i++)
            //    {
            //        CanvasGroup skeletonCanvasGroup = skeletons[i].GetOrAddComponent<CanvasGroup>();
            //        skeletonCanvasGroup.ignoreParentGroups = true;
            //    }
            //}

            startPos = _startPos;
            endPos = _endPos;
            
           
            OnEnterFinish = new UnityEvent();
            if (_enterCallback != null)
                OnEnterFinish.AddListener(_enterCallback);
            OnExitFinish = new UnityEvent();
            if (_exitCallback != null)
                OnExitFinish.AddListener(_exitCallback);
        }

        public IdleUIAnimation(RectTransform _rectTrans,UIAnimationDir dir, UnityAction _enterCallback = null, UnityAction _exitCallback = null)
        {
            isActive = false;
            rectTrans = _rectTrans;
            canvasGroup = rectTrans.transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = rectTrans.gameObject.AddComponent<CanvasGroup>();

            skeletons = rectTrans.GetComponentsInChildren<SkeletonGraphic>();
            //if (skeletons != null && skeletons.Length > 0)
            //{
            //    for (int i = 0; i < skeletons.Length; i++)
            //    {
            //        CanvasGroup skeletonCanvasGroup = skeletons[i].GetOrAddComponent<CanvasGroup>();
            //        skeletonCanvasGroup.ignoreParentGroups = true;
            //    }
            //}

            startPos = endPos = rectTrans.anchoredPosition;
            switch (dir)
            {
                case UIAnimationDir.Left:
                    startPos.x -= 100;
                    break;
                case UIAnimationDir.Right:
                    startPos.x += 100;
                    break;
                case UIAnimationDir.Top:
                    startPos.y += 100;
                    break;
                case UIAnimationDir.Bottom:
                    startPos.y -= 100;
                    break;
            }

            OnEnterFinish = new UnityEvent();
            if (_enterCallback != null)
                OnEnterFinish.AddListener(_enterCallback);
            OnExitFinish = new UnityEvent();
            if (_exitCallback != null)
                OnExitFinish.AddListener(_exitCallback);
        }



        public void SetStartPos(Vector2 newPos)
        {
            startPos = newPos;
        }
        public void SetEndPos(Vector2 newPos)
        {
            endPos = newPos;
        }

        public void SetExitFinishEvent(UnityAction newEvent)
        {
            if (newEvent == null)
            {
                OnExitFinish.RemoveAllListeners();
                return;
            }
            OnExitFinish.RemoveAllListeners();
            OnExitFinish.AddListener(newEvent);
        }
        public void SetEnterFinishEvent(UnityAction newEvent)
        {
            if(newEvent==null)
            {
                OnEnterFinish.RemoveAllListeners();
                return;
            }

            OnEnterFinish.RemoveAllListeners();
            OnEnterFinish.AddListener(newEvent);
        }


        public void Enter(bool isFade=true)
        {
            sequence.Kill(true);
            isActive = true;
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            rectTrans.anchoredPosition = startPos;
            canvasGroup.blocksRaycasts = true;
            if (isFade)
            {
                canvasGroup.alpha = 0.0f;
                sequence.Append(canvasGroup.DOFade(1, 0.3f));
            }

            for(int i=0;i<skeletons.Length;i++)
            {
                //skeletons[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                skeletons[i].DOColor(Color.white, 0.3f);
            }

            sequence.Join(rectTrans.DOAnchorPos(endPos, 0.3f).SetEase(Ease.OutBack)).OnComplete(() =>
            {
                OnEnterFinish.Invoke();
           
            });
            
        }

        public void Exit()
        {
            sequence.Kill(true);
            isActive = false;
          
            canvasGroup.alpha = 1.0f;
            rectTrans.anchoredPosition = endPos;
            
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.Append(canvasGroup.DOFade(0, 0.3f));
            for (int i = 0; i < skeletons.Length; i++)
            {
                //skeletons[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                Color tempColor = Color.white;
                tempColor.a = 0;
                skeletons[i].DOColor(tempColor, 0.3f);
            }

            sequence.Join(rectTrans.DOAnchorPos(startPos, 0.3f).SetEase(Ease.InBack));
      
            sequence.OnComplete(()=> {
                canvasGroup.blocksRaycasts = false;
                OnExitFinish.Invoke();
            });
        }

        public void ExitImmediatelty()
        {
            isActive = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            rectTrans.anchoredPosition = startPos;
            OnExitFinish.Invoke();
        }

        
    }
}


