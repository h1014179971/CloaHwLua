using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Spine;
using Spine.Unity;

namespace Delivery.Idle
{
    //快递员
    public class IdleStaffModel : MonoBehaviour
    {
        private enum StaffState
        {
            None, 
            Idle,//排队等待状态
            IdleRun, //排队向前移动状态
            Wait,//等待装货状态
            Run,
            BackRun
        }
        private StaffState _staffState;
        private MeshRenderer _meshRenderer;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        private SpriteRenderer _shadowSprite;
        private IdleSiteModel _idleSiteModel;
        private bool _isLoad;//是否可以装货
        private int _loadItemNum;//当前送货量
        private Vector3 _speed;//移动方向速度
        private int _speedTimes=1;//移动速度倍数(特殊事件开启时改变)
        private Vector3 _target;
        private List<Vector3> _goPoints = new List<Vector3>();
        private List<Vector3> _backPoints = new List<Vector3>();
        private int _pointIndex;
        private int _buildId;//送到建筑的id

        private float itemPriceTimes = 1;//计算单价的倍数（默认1倍，有特殊事件时可能更改）
        private float doubleIncomeTimes = 1;//双倍收益系数（玩家看广告开启双倍收益）

        private string _aniWalk = "walk";
        private string _aniWalks = "walks";
        private string _aniIdle = "idle";

        public void Init(IdleSiteModel model,Transform linePoint,int buildId)
        {
            _idleSiteModel = model;
            _buildId = buildId;
            if (_spineAnimationState == null)
            {
                SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
                _spineAnimationState = skeletonAnimation.AnimationState;
                skeletonAnimation.loop = true;  
                _skeleton = skeletonAnimation.Skeleton;  
            }
            _meshRenderer = GetComponent<MeshRenderer>();
            Transform goPoint = linePoint.Find("goPoint");
            for (int i = 0; i < goPoint?.childCount; i++)
            {
                Vector3 vec = goPoint.GetChild(i).position;
                vec.z = 0;
                _goPoints.Add(vec);
            }
            Transform backPoint = linePoint.Find("backPoint");
            for (int i = 0; i < backPoint?.childCount; i++)
            {
                Vector3 vec = backPoint.GetChild(i).position;
                vec.z = 0;
                _backPoints.Add(vec);
            }
            _shadowSprite = transform.Find("shadow").GetComponent<SpriteRenderer>();
            OrderLayer();
            Idle();
            //添加特殊事件相关监听
            //EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeItemPrice, OnItemPriceChange);
            //EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_DoubleIncome_Start, OnDoubleIncomeStart);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_DoubleIncome_End, OnDoubleIncomeEnd);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, SetSpeed);
        }

        ////当特殊事件触发货物价格变化时更改货物单价计算倍数
        //private void OnItemPriceChange(BaseEventArgs baseEventArgs)
        //{
        //    EventArgsTwo<int, float> argsTwo = (EventArgsTwo<int, float>)baseEventArgs;
        //    if (argsTwo.param1 != _idleSiteModel.SiteId) return;
        //    itemPriceTimes = argsTwo.param2;
        //}
        ////当特殊事件结束后恢复货物单价计算倍数
        //private void OnSpecialEventEnd(BaseEventArgs baseEventArgs)
        //{
        //    itemPriceTimes = 1;
        //}

        private void SetSpeed(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> args = baseEventArgs as EventArgsOne<int>;
            _speedTimes = args.param1;
        }

        //开启双倍收益
        private void OnDoubleIncomeStart(BaseEventArgs baseEventArgs)
        {
            doubleIncomeTimes = 2;
        }
        //结束双倍收益
        private void OnDoubleIncomeEnd(BaseEventArgs baseEventArgs)
        {
            doubleIncomeTimes = 1;
        }

        private void Idle()
        {
             if(_staffState != StaffState.Idle)
            {
                _staffState = StaffState.Idle;
                _spineAnimationState.SetAnimation(0, _aniIdle, true);
                _idleSiteModel.AddIdleStaff(this);
            } 
        }
        public void SetIdleTarget(Vector3 target)
        {
            if (_staffState == StaffState.Wait && _isLoad)
            {
                WaitLoadItem();
                return;
            }
                
            //if (_staffState != StaffState.Idle) return;
            //if (transform.position == target)
            //{
            //    if (transform.position == _idleSiteModel.WaitPoint())
            //        Wait();
            //    else
            //        Idle();
            //    return;
            //}
            float dis = (transform.position-target).magnitude;
            if(dis <= 0.05f)
            {
                if ((transform.position - _idleSiteModel.WaitPoint()).magnitude <=0.05f)
                    Wait();
                else
                    Idle();
                return;
            }
            SetTarget(target);
            IdleRun();
            //_staffState = StaffState.IdleRun;
            //_spineAnimationState.SetAnimation(0, "banyun_walk", true);
            //Vector3 direction = target - transform.position;
            //_speed = direction.normalized * _idleSiteModel.IdleSiteTime.speed;
            //_target = target;
        }
        private void IdleRun()
        {
            if(_staffState != StaffState.IdleRun)
            {
                _staffState = StaffState.IdleRun;
                _spineAnimationState.SetAnimation(0, _aniWalk, true);
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.magnitude >= (_target - transform.position).magnitude)
            {
                transform.position = _target;
                if (transform.position == _idleSiteModel.WaitPoint())
                    Wait();
                else
                    Idle();
            }   
            else
                transform.position += v3;
        }
        private void Wait()
        {
            if (_staffState != StaffState.Wait)
            {
                _staffState = StaffState.Wait;
                _spineAnimationState.SetAnimation(0, _aniIdle, true);
            }
                
            if (!_isLoad)
            {
                Timer.Instance.Register(_idleSiteModel.IdleSiteTime.qtime, 1, (pare) => {
                    _isLoad = true;
                    WaitLoadItem();
                }).AddTo(gameObject);
            }
            else
            {
                WaitLoadItem();
            }
            
        }
        public void WaitLoadItem()
        {
            if (_staffState != StaffState.Wait) return;
            int siteLoadNum = _idleSiteModel.GetItemNum();
            if(siteLoadNum <= 0)
            {
                LogUtility.LogError($"{_idleSiteModel.SiteId}驿站缺货，继续等待"); 
                return;
            }
            int loadNum = _idleSiteModel.IdleSiteBase.itemnum;
            _loadItemNum = loadNum < siteLoadNum ? loadNum : siteLoadNum;
        
            if(loadNum<siteLoadNum)
            {
                _loadItemNum = loadNum;
            }
            else
            {
                _loadItemNum = siteLoadNum;
            }

            _idleSiteModel.UnLoadItem(_loadItemNum);
            _isLoad = false;
            _idleSiteModel.RemoveIdleStaff(this);
            AudioCtrl.Instance.PlayMultipleSound(GameAudio.staffLoadItem);
            Run();
        }
        private void Run()
        {
            if (_staffState != StaffState.Run)
            {
                _staffState = StaffState.Run;
                _spineAnimationState.SetAnimation(0, _aniWalks, true);
                _pointIndex = 0;
                Vector3 target = _goPoints[_pointIndex];
                SetTarget(target);
                //_target = _idleSiteModel.GoPoints[_pointIndex];
                //_speed = (_target - transform.position).normalized * _idleSiteModel.IdleSiteTime.speed;
                
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.sqrMagnitude >= (_target - transform.position).sqrMagnitude)
            {
                transform.position = _target;
                _pointIndex++;
                if (_pointIndex >= _goPoints.Count)
                {
                    AddMoney();
                    BackRun();
                }
                    
                else
                {
                    Vector3 target = _goPoints[_pointIndex];
                    SetTarget(target);
                    //_target = _idleSiteModel.GoPoints[_pointIndex];
                    //_speed = (_target - transform.position).normalized * _idleSiteModel.IdleSiteTime.speed;
                }
                    
            }
            else
                transform.position += v3;
        }
        private void SetTarget(Vector3 target)
        {
            _target = target;
            _speed = (_target - transform.position).normalized * _idleSiteModel.IdleSiteTime.speed * _speedTimes;
            if (_target.x < transform.position.x)
                _skeleton.ScaleX = 1;
            else
                _skeleton.ScaleX = -1;
        }
        private void AddMoney()
        {
            _idleSiteModel.IdleSiteBuildLoadItem(_buildId);
            Long2 unitPrice = new Long2(_idleSiteModel.IdleSiteBase.value);
            Long2 price = unitPrice * _loadItemNum * itemPriceTimes * doubleIncomeTimes;
            Long2 totalmul = new Long2(_idleSiteModel.IdleSiteGrade.totalmultiple);
            price = price * totalmul;
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById();
            price *= new Long2(idleCity.totalmultiple);
            PlayerMgr.Instance.AddMoney(price);
            _loadItemNum = 0;
            FxCtrl.Instance.PlayFx(FxPrefabPath.idleAddMoney,transform.position+Vector3.up*2,null);
            //AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
            AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.vendingMoney); 
            string moneyStr = UnitConvertMgr.Instance.GetFloatValue(price,2);
            UIFxCtrl.Instance.PlayFx(FxPrefabPath.uiIdleAddMoney, transform.position + Vector3.up * 2, moneyStr);

            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, gameObject.name));
        }
        private void BackRun()
        {
            if (_staffState != StaffState.BackRun)
            {
                _staffState = StaffState.BackRun;
                _spineAnimationState.SetAnimation(0, _aniWalk, true);
                _pointIndex = 0;
                Vector3 target = _backPoints[_pointIndex];
                SetTarget(target);
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.magnitude >= (_target - transform.position).magnitude)
            {
                transform.position = _target;
                _pointIndex++;
                if (_pointIndex >= _backPoints.Count)
                {
                    Idle();
                }

                else
                {
                    Vector3 target = _backPoints[_pointIndex];
                    SetTarget(target);
                }

            }
            else
                transform.position += v3;

        }
        // Update is called once per frame
        void Update()
        {
            if (_staffState == StaffState.IdleRun)
            {
                IdleRun();
            }
            else if (_staffState == StaffState.Run)
            {
                Run();
            }
            else if (_staffState == StaffState.BackRun)
                BackRun();
        }
        private void OrderLayer()
        {
            Timer.Instance.Register(0.5f, -1, (pare) => {
                _meshRenderer.sortingOrder = -(int)(transform.position.y *10);
                _shadowSprite.sortingOrder = _meshRenderer.sortingOrder - 1;
            }).AddTo(gameObject);
        }

        private void OnDestroy()
        {
            _staffState = StaffState.None;
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeItemPrice, OnItemPriceChange);
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_DoubleIncome_Start, OnDoubleIncomeStart);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_DoubleIncome_End, OnDoubleIncomeEnd);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, SetSpeed);
        }

    }
}

