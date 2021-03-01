using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Spine.Unity;

namespace Delivery.Idle 
{
    public class IdleCityStaffModel : MonoBehaviour
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
        private bool _isLoad;//是否可以装货
        private Vector3 _speed;//移动方向速度
        private int _speedTimes = 1;//移动速度倍数
        private Vector3 _target;
        private List<Vector3> _goPoints = new List<Vector3>();
        private int _pointIndex;
        private LoadModel _loadModel;
        private int _itemId;//运送货物的id
        private Transform _stopTran;
        private IdleTruckStopModel _loadTran;//驿站卡车装货路点
        private MeshRenderer _meshRenderer;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        private SpriteRenderer _shadowSprite;
        private string _aniWalk = "walk";
        private string _aniWalks = "walks";
        private string _aniIdle = "idle";
        public void Init(int itemId, Transform stopTran)
        {
            _itemId = itemId;
            _stopTran = stopTran;
            _loadTran = _stopTran.Find("loadPoint").GetOrAddComponent<IdleTruckStopModel>();
            _loadTran.Init();
            if (_spineAnimationState == null)
            {
                SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
                _spineAnimationState = skeletonAnimation.AnimationState;
                skeletonAnimation.loop = true;
                _skeleton = skeletonAnimation.Skeleton;
            }
            _meshRenderer = GetComponent<MeshRenderer>();
            _shadowSprite = transform.Find("shadow").GetComponent<SpriteRenderer>();
            OrderLayer();
            Idle();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeSiteStaffSpeed, SetSpeed);
        }

        private void SetSpeed(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> args = baseEventArgs as EventArgsOne<int>;
            _speedTimes = args.param1;
        }

        public int ItemId { get { return _itemId; } }
        private void Idle()
        {
            if (_staffState != StaffState.Idle)
            {
                _staffState = StaffState.Idle;
                _spineAnimationState.SetAnimation(0, _aniIdle, true);
                IdleCityStaffCtrl.Instance.AddIdleStaff(this);
            }
        }
        public void SetIdleTarget(Vector3 target)
        {
            if (_staffState == StaffState.Wait && _isLoad)
            {
                WaitLoadItem();
                return;
            }
            float dis = (transform.position - target).magnitude;
            if (dis <= 0.05f)
            {
                if ((transform.position - IdleCityStaffCtrl.Instance.WaitPoint(ItemId)).magnitude <= 0.05f)
                    Wait();
                else
                    Idle();
                return;
            }
            SetTarget(target);
            IdleRun();
        }
        private void IdleRun()
        {
            if (_staffState != StaffState.IdleRun)
            {
                _staffState = StaffState.IdleRun;
                _spineAnimationState.SetAnimation(0, _aniWalk, true);
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.magnitude >= (_target - transform.position).magnitude)
            {
                transform.position = _target;
                if (transform.position == IdleCityStaffCtrl.Instance.WaitPoint(ItemId))
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
                Timer.Instance.Register(0.5f, 1, (pare) => {
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
            _loadModel = IdleCityStaffCtrl.Instance.GetLoadModel(ItemId);
            if (_loadModel == null )
            {
                LogUtility.LogError($"{ItemId}接收区没有等待的卡车,继续等待");
                return;
            }
            if (PlayerMgr.Instance.GetCurrentStoreVolume(ItemId) <= 0)
            {
                LogUtility.LogError($"快递中心没有此类{ItemId}货物，继续等待");
                return;
            }
            _loadModel.isWaitForLoad = true;
            IdleCityCtrl.Instance.UnLoadItem(ItemId, _loadModel.loadVolume);
            _isLoad = false;
            IdleCityStaffCtrl.Instance.RemoveIdleStaff(this);
            Run();
        }
        private void Run()
        {
            if (_staffState != StaffState.Run)
            {
                _staffState = StaffState.Run;
                _spineAnimationState.SetAnimation(0, _aniWalks, true);
                Vector3 p1 = _loadTran.GetPoint();
                Vector3 v1 = _loadTran.GetVector();
                StartPath(p1, v1, _loadModel.p2, _loadModel.v2);
                _pointIndex = 0;
                Vector3 target = _goPoints[_pointIndex];
                SetTarget(target);
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.sqrMagnitude >= (_target - transform.position).sqrMagnitude)
            {
                transform.position = _target;
                _pointIndex++;
                if (_pointIndex >= _goPoints.Count)
                {
                    BackRun();
                }

                else
                {
                    Vector3 target = _goPoints[_pointIndex];
                    SetTarget(target);
                }

            }
            else
                transform.position += v3;
        }
        private void SetTarget(Vector3 target)
        {
            _target = target;
            _speed = (_target - transform.position).normalized * 2 * _speedTimes;
            if (_target.x < transform.position.x)
                _skeleton.ScaleX = 1;
            else
                _skeleton.ScaleX = -1;
        }
        private void BackRun()
        {
            if (_staffState != StaffState.BackRun)
            {
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, gameObject.name));
                IdleCityStaffCtrl.Instance.RemoveIdleTruck(ItemId, _loadModel);
                _staffState = StaffState.BackRun;
                _spineAnimationState.SetAnimation(0, _aniWalk, true);
                _goPoints.Insert(0,IdleCityStaffCtrl.Instance.GetWaitPoint(ItemId));
                _pointIndex = _goPoints.Count-2;
                Vector3 target = _goPoints[_pointIndex];
                SetTarget(target);
            }
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if (v3.magnitude >= (_target - transform.position).magnitude)
            {
                transform.position = _target;
                _pointIndex--;
                if (_pointIndex < 0)
                {
                    Idle();
                }

                else
                {
                    Vector3 target = _goPoints[_pointIndex];
                    SetTarget(target);
                }

            }
            else
                transform.position += v3;

        }
        private void StartPath(Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2)
        {
            _goPoints.Clear();
            _goPoints.Add(p1);
            Vector3 midVec = Vector3.zero;
            if (Utils.IsLineCross(out midVec, p1, v1, p2, v2))
            {
                _goPoints.Add(midVec);
                _goPoints.Add(midVec + v2.normalized * 0.2f);
            }
            else
                LogUtility.LogError($"叉车{ItemId}IsLineCross返回false");
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
            Timer.Instance.Register(0.1f, -1, (pare) => {
                _meshRenderer.sortingOrder = -(int)(transform.position.y * 10);
                _shadowSprite.sortingOrder = _meshRenderer.sortingOrder - 1;
            }).AddTo(gameObject);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeSiteStaffSpeed, SetSpeed);
        }

    }
}


