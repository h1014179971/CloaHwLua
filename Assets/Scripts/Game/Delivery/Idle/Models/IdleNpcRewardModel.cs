using DG.Tweening;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;
namespace Delivery.Idle
{

    public class IdleNpcRewardModel : MonoBehaviour
    {
        public int siteId=0;
        public int minTime=30;
        public int maxTime = 45;
        public float minIncome = 0.1f;//最小的倍数
        public float maxIncome = 0.2f;//最大的倍数

        private int triggerTime;
        private float _rewardTimes;
        private GameObject moneyIcon;
        private TimerEvent showTimer;
        private RectTransform fxNode;
        private Vector3 fxEndPoint;
        private IdleTopUI topUI;
        private bool canGetMoney;

        private PlayerSite playerSite;
        private Tweener moneyAnim;
        
        private Vector3 FxEndPoint
        {
            get
            {
                if(fxEndPoint == Vector3.zero)
                {
                    topUI = FindObjectOfType<IdleTopUI>();
                    fxEndPoint = topUI.GetMoneyIconPos();
                }
                return fxEndPoint;
            }
        }
       
        private void Awake()
        {
            fxNode = RootCanvas.Instance.GetComponentByPath<Transform>("UIFxRoot").GetComponent<RectTransform>();
            moneyIcon = this.GetComponentByPath<Transform>("moneyIcon").gameObject;
            SetMoneyIcon();
            fxEndPoint = Vector3.zero;
            moneyIcon.SetActive(false);
            _rewardTimes = 0;
            canGetMoney = false;
            

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Game_Began, OnGameStart);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Game_Began, OnGameStart);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }

        private void OnGameStart(BaseEventArgs baseEventArgs)
        {
            playerSite = PlayerMgr.Instance.GetPlayerSite(siteId);

            if (playerSite != null && !playerSite.isLock)
            {
                triggerTime = Random.Range(minTime, maxTime);
                showTimer = Timer.Instance.Register(triggerTime, 1, ShowMoney).AddTo(gameObject);//开始计时
            }
        }

        private void OnUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> arg = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            if (arg.param1 == null || arg.param1.SiteId != siteId) return;
            triggerTime = Random.Range(minTime, maxTime);
            showTimer = Timer.Instance.Register(triggerTime, 1, ShowMoney).AddTo(gameObject);//开始计时
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }


        private void ShowMoney(params object[]objs)
        {
            moneyIcon.SetActive(true);
            moneyIcon.transform.localScale = Vector3.one;
            if (moneyAnim == null)
                moneyAnim = moneyIcon.transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);

            Timer.Instance.DestroyTimer(showTimer);
            showTimer = null;
            canGetMoney = true;
        }

        public void GetReward()
        {
            if (!canGetMoney) return;
            canGetMoney = false;
            moneyAnim.Kill(true);
            moneyAnim = null;
            moneyIcon.SetActive(false);
            _rewardTimes = Random.Range(minIncome, maxIncome);
          
            FxCtrl.Instance.PlayFx(FxPrefabPath.idleAddMoney, transform.position + Vector3.up * 2, null);

            IdleFxHelper.PlayGetRewardFxWorld(transform, () =>
            {
                Long2 income = PlayerMgr.Instance.GetCityIncome() * _rewardTimes;
                PlayerMgr.Instance.AddMoney(income);
                triggerTime = Random.Range(minTime, maxTime);
                showTimer = Timer.Instance.Register(triggerTime, 1, ShowMoney).AddTo(gameObject);//开始计时
            },true);
            
        }


        private void SetMoneyIcon()
        {
            SpriteRenderer moneySr = moneyIcon.GetComponent<SpriteRenderer>();
            moneySr.sortingOrder = -(int)(transform.position.y * 10);
            moneySr.sortingOrder = moneySr.sortingOrder - 1;
        }
        

    }


}

