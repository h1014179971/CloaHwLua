using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;
using libx;
namespace UIFramework
{
    public class UIPanel : MonoBehaviour
    {
        public AssetRequest request;

        private Coroutine destroyCoroutine = null;//销毁自身的协程
        public bool IsActive { get; set; }

        public List<UIAnimation> OpenAnimation = new List<UIAnimation>();
        public List<UIAnimation> CloseAnimation = new List<UIAnimation>();
        public List<UIAnimation> ShowAnimation = new List<UIAnimation>();
        public List<UIAnimation> HideAnimation = new List<UIAnimation>();

        [NonSerialized]
        public UnityEvent OnOpen = new UnityEvent();
        [NonSerialized]
        public UnityEvent AfterOpen = new UnityEvent();

        [NonSerialized]
        public UnityEvent OnClose = new UnityEvent();
        [NonSerialized]
        public UnityEvent AfterClose = new UnityEvent();

        [NonSerialized]
        public UnityEvent OnShow = new UnityEvent();
        [NonSerialized]
        public UnityEvent AfterShow = new UnityEvent();

        [NonSerialized]
        public UnityEvent OnHide = new UnityEvent();
        [NonSerialized]
        public UnityEvent AfterHide = new UnityEvent();

        [NonSerialized]
        public UnityEvent AfterDestroy = new UnityEvent();

        private RectTransform _UIRectTransform;
        public RectTransform UIRectTransform
        {
            get
            {
                if (_UIRectTransform == null)
                {
                    _UIRectTransform = this.GetComponent<RectTransform>();
                    _InitAnchoredPos = _UIRectTransform.anchoredPosition;
                }
                return _UIRectTransform;
            }
        }

        private Vector2 _InitAnchoredPos;
        public Vector2 InitAnchoredPos
        {
            get
            {
                return _InitAnchoredPos;

            }
        }

        

        public virtual void Open()
        {
            OnOpen.Invoke();
            IsActive = true;

            //打断协程
            StopCoroutine("DelayDestroy");
            
            transform.localPosition = Vector3.zero;
            if (OpenAnimation.Count == 0)
            {
                AfterOpen.Invoke();
                return;
            }

            int last = OpenAnimation.Count - 1;
            for (int i = 0; i < OpenAnimation.Count; i++)
            {
                if (i == last)
                {
                    OpenAnimation[i].Play(AfterOpen);
                }
                else
                {
                    OpenAnimation[i].Play();
                }
            }
        }
      
        private IEnumerator DelayDestroy()
        {
            
            transform.localPosition = new Vector3(10000, 10000, 0);
            yield return new WaitForSeconds(10.0f);
            ImmediateDestroy();
        }

        private void ImmediateDestroy()
        {
            AfterDestroy.Invoke();

            if (request != null)
                request.Release();
            Destroy(gameObject);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        /// <summary>
        /// 播放关闭动画结束后同时销毁对象
        /// </summary>
        public void PlayCloseAniAndDestory(bool isImmediately=true)
        {
            OnClose.Invoke();
            AfterClose.AddListener(() =>
            {
                IsActive = false;
                if (isImmediately)
                    ImmediateDestroy();
                else
                {
                    StartCoroutine("DelayDestroy");
                }
                //Destroy(gameObject);
                //Resources.UnloadUnusedAssets();
                //System.GC.Collect();
            });

            if (CloseAnimation.Count == 0)
            {
                AfterClose.Invoke();
                return;
            }

            int last = CloseAnimation.Count - 1;
            for (int i = 0; i < CloseAnimation.Count; i++)
            {
                if (i == last)
                {
                    CloseAnimation[i].Play(AfterClose);
                }
                else
                {
                    CloseAnimation[i].Play();
                }
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow.Invoke();

            if (ShowAnimation.Count == 0)
            {
                AfterShow.Invoke();
                return;
            }

            int last = ShowAnimation.Count - 1;
            for (int i = 0; i < ShowAnimation.Count; i++)
            {
                if (i == last)
                {
                    ShowAnimation[i].Play(AfterShow);
                }
                else
                {
                    ShowAnimation[i].Play();
                }
            }
        }

        public virtual void Hide()
        {
            OnHide.Invoke();

            if (HideAnimation.Count == 0)
            {
                AfterHide.Invoke();
                return;
            }

            int last = HideAnimation.Count - 1;
            for (int i = 0; i < HideAnimation.Count; i++)
            {
                if (i == last)
                {
                    HideAnimation[i].Play(AfterHide);
                }
                else
                {
                    HideAnimation[i].Play();
                }
            }
        }

        protected void AddOpenAnimation<T>(Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) where T : UIAnimation, new()
        {
            T t = new T();
            t.SetParams(UIRectTransform, ease, duration);
            OpenAnimation.Add(t);
        }

        protected void AddCloseAnimation<T>(Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) where T : UIAnimation, new()
        {
            T t = new T();
            t.SetParams(UIRectTransform, ease, duration);
            CloseAnimation.Add(t);
        }

        protected void AddShowAnimation<T>(Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) where T : UIAnimation, new()
        {
            T t = new T();
            t.SetParams(UIRectTransform, ease, duration);
            ShowAnimation.Add(t);
        }

        protected void AddHideAnimation<T>(Ease ease = Ease.Linear, float duration = UIAnimationSpeed.NORMAL) where T : UIAnimation, new()
        {
            T t = new T();
            t.SetParams(UIRectTransform, ease, duration);
            HideAnimation.Add(t);
        }

    }

    
}