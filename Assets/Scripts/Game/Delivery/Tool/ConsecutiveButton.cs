using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Foundation;
namespace Delivery
{
    public class ConsecutiveButton : Selectable,IPointerClickHandler
    {
        private float m_MinIntervalTime = 0.01f;
        private float m_MaxIntervalTime = 0.3f;
        [SerializeField] private UnityEvent onLongpress;
        [SerializeField] private ParticleSystem _particleSystem;
        public UnityEvent OnClick => onLongpress;
        public UnityEvent PointerUp;
        private int m_FadeNum = 10;
        private float m_IntervalTime;
        private int m_DownNum;
        private float m_LastInvokeTime = 0.0f;
        private bool m_IsDown = false;
        private Graphic[] m_Graphics;

        private bool m_hasBreak;

        private ButtonInfo _buttonInfo;
        private ButtonInfo buttonInfo
        {
            get
            {
                if (_buttonInfo == null)
                    _buttonInfo = GetComponent<ButtonInfo>();
                return _buttonInfo;
            }
        }


        protected override void Awake()
        {
            m_Graphics = GetComponentsInChildren<Graphic>(true);
            base.Awake();
            
        }

        public void RemoveAllClickListeners()
        {
            onLongpress?.RemoveAllListeners();
        }

        public void AddClickListener(UnityAction action)
        {
            if (onLongpress == null)
                onLongpress = new UnityEvent();
            onLongpress?.AddListener(action);
            onLongpress?.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            if (string.IsNullOrEmpty(buttonInfo.clipName))
                LogUtility.LogError($"{name}没有音效");
            else
                AudioCtrl.Instance.PlaySingleSound2D(buttonInfo.clipName);
        }
        public bool isGuiding = false;//是否处在引导状态
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (isGuiding) return;//如果是引导的时候调用

            if (!interactable)
                return;
            m_hasBreak = false;
            m_IsDown = true;
            m_DownNum = 0;
            m_LastInvokeTime = Time.time;
            if (_particleSystem != null) _particleSystem.Play();
           
            m_IntervalTime = m_MaxIntervalTime;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_IsDown = false;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            m_IsDown = false;
            isGuiding = false;
            PointerUp?.Invoke();

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            m_IsDown = false;
            isGuiding = false;
            PointerUp?.Invoke();
        }
        
        protected virtual void Update()
        {
            if (!m_IsDown)
                return;
            if (Time.time - m_LastInvokeTime > m_IntervalTime)
            {
                //触发点击;
                m_DownNum++;
                m_IntervalTime = Mathf.Lerp(m_MaxIntervalTime, m_MinIntervalTime, m_DownNum * 1.0f / m_FadeNum);
                if (interactable)
                {
                    if (_particleSystem != null) _particleSystem.Play();
                    onLongpress?.Invoke();
                }
                m_LastInvokeTime = Time.time;
            }
        }

        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (!this.gameObject.activeInHierarchy)
                return;
            Color color;
            switch (state)
            {
                case Selectable.SelectionState.Normal:
                    color = this.colors.normalColor;
                    break;
                case Selectable.SelectionState.Highlighted:
                    color = this.colors.highlightedColor;
                    
                    break;
                case Selectable.SelectionState.Pressed:
                    color = this.colors.pressedColor;
                    
                    break;
                case Selectable.SelectionState.Disabled:
                    color = this.colors.disabledColor;
                    
                    break;
                default:
                    color = Color.black;
                    break;
            }
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    foreach (var graphic in m_Graphics)
                    {
                        graphic.CrossFadeColor(color * this.colors.colorMultiplier, 0.0f , true, true);
                    }
                    break;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_hasBreak) return;
            onLongpress.Invoke();
        }


        public void ForbiddenBtn()
        {
            m_hasBreak = true;
            m_IsDown = false;
            DoStateTransition(SelectionState.Normal, false);
            isGuiding = false;
        }
        
    }
}
