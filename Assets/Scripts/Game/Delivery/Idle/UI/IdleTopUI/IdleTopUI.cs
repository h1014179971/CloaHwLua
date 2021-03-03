using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using DG.Tweening;
using System;

namespace Delivery.Idle
{
    public class IdleTopUI : UIBlockerBase
    {
        private Player player;
        private MyText money;
        private Transform moneyIcon;
        private Animation moneyAnim;

        private MyText income;
        private Animation incomeAnim;
        private Long2 _lastIncome;
        private Long2 _toIncome;
        private TimerEvent _incomeTimer;
        private Sequence _incomeSeq;
        private Long2 _uil;//收益单次滚动大小

        private void Awake()
        {
            RectTransform bg = this.GetComponentByPath<RectTransform>("bg");
            bg.offsetMin = new Vector2(0, ((Screen.safeArea.height + Screen.safeArea.y) - Screen.height) * FixScreen.height / Screen.height);
            money = this.GetComponentByPath<MyText>("bg/money");
            moneyAnim = money.GetComponent<Animation>();
            moneyIcon = this.GetComponentByPath<Transform>("bg/moneyIcon");
            income = this.GetComponentByPath<MyText>("bg/incomeIcon/income");
            incomeAnim = income.GetComponent<Animation>();
            player = PlayerMgr.Instance.Player;
            _lastIncome = PlayerMgr.Instance.GetCityIncome();
            _toIncome = _lastIncome;
            income.text = UnitConvertMgr.Instance.GetFloatValue(_lastIncome, 2) + "/分钟";
            UpdateMoney(null);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateMoney);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_TotalIncomeChange, UpdateIncome);
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateMoney);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_TotalIncomeChange, UpdateIncome);
        }

        private void UpdateMoney(BaseEventArgs baseEventArgs)
        {
            money.text = UnitConvertMgr.Instance.GetFloatValue(PlayerMgr.Instance.PlayerCity.money, 2);
        }
        
        public Vector3 GetMoneyIconPos()
        {
            return moneyIcon.position;
        }

        public Vector3 GetMoneyTextPos()
        {
            return money.transform.position;
        }

        public void PlayMoneyAnim()
        {
            if (!moneyAnim.isPlaying)
                moneyAnim.Play();
        }

        private void UpdateIncome(BaseEventArgs baseEventArgs)
        {
            //income.text = UnitConvertMgr.Instance.GetFloatValue(PlayerMgr.Instance.GetCityIncome(), 2) + "/分钟";
            //PlayIncomeAnim();
            IncomeRoll();
        }

        private void PlayIncomeAnim()
        {
            if (!incomeAnim.isPlaying)
                incomeAnim.Play();
        }
        void IncomeRoll()
        {
            //if (_incomeTimer != null)
            //{
            //    Timer.Instance.DestroyTimer(_incomeTimer);
            //    _incomeTimer = null;
            //    _lastIncome = _toIncome;
            //    income.text = UnitConvertMgr.Instance.GetFloatValue(_lastIncome, 2) + "/分钟";
            //}
            if (_lastIncome != _toIncome)
                _lastIncome = _toIncome;
            _toIncome = PlayerMgr.Instance.GetCityIncome();
            _uil = _toIncome - _lastIncome;
            _uil /= Application.targetFrameRate;
            int y = ((int)((_lastIncome.y + Math.Log10(Mathf.Abs(_lastIncome.x))) / 3)) * 3;
            if (y > 2)
                y -= 2;
            Long2 unit = new Long2(1, y);
            unit = (_toIncome - _lastIncome) < unit ? (_toIncome - _lastIncome) : unit;
            if (_uil < unit)
                _uil = unit;
            if(_incomeTimer == null && _incomeSeq == null)
            {
                _incomeSeq = DOTween.Sequence();
                _incomeSeq.SetUpdate(true);
                _incomeSeq.Append(income.transform.DOScale(Vector3.one * 1.5f, 0.2f));
                _incomeSeq.Join(income.DOColor(Utils.HexToColor("06F823"), 0.2f));
                _incomeSeq.OnComplete(() =>
                {
                    _incomeSeq = null;
                    _incomeTimer = Timer.Instance.Register(Time.deltaTime, -1, (complete) => {
                        _lastIncome += _uil;
                        if (_lastIncome >= _toIncome)
                        {
                            _lastIncome = _toIncome;
                            if (_incomeTimer != null)
                                Timer.Instance.DestroyTimer(_incomeTimer);
                            _incomeTimer = null;
                            income.text = UnitConvertMgr.Instance.GetFloatValue(_lastIncome, 2) + "/分钟";
                            //PlayIncomeAnim();
                            Sequence seq = DOTween.Sequence();
                            seq.SetUpdate(true);
                            seq.Append(income.transform.DOScale(Vector3.one, 0.1f));
                            seq.Join(income.DOColor(Utils.HexToColor("FFFFFF"), 0.1f));
                        }
                        else
                            income.text = UnitConvertMgr.Instance.GetFloatValue(_lastIncome, 2) + "/分钟";
                    }).AddTo(gameObject);
                });
            }
            
        }
    }
}


