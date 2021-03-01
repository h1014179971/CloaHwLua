using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Pathfinding;

namespace Delivery.Idle
{
    public class IdleForkModel : MonoBehaviour
    {
        //private Sprite _spriteUp;//朝上图片
        //private Sprite _emptySpriteUp;
        //private Sprite _spriteDown;//朝下图片
        //private Sprite _emptySpriteDown;
        private GameObject _fork;
        private GameObject _forkItemParent;
        private SpriteRenderer[] _forkSprites;
        
        private Animation _animation;
        private Vector3 _target;
        private List<Vector3> _vectorPath = new List<Vector3>();
        private Vector3 _nextPoint;
        private Vector3 _offset;
        private int _pathIndex;
        private float _speed = 2.5f;
        private int _speedTimes = 1;//速度倍数(触发特殊事件时改变)
        private int _itemId;//运送货物的id
        private int _maxVolume = 6;// 叉车货物上限
        private int _forkVolumeTimes = 1;//叉车容量上限倍数
        private int _truckVolumeTimes = 1;//货车容量倍数(触发特殊事件时改变)
        private int _volume;//叉车当前装货量
        private Transform _stopTran;
        private IdleTruckStopModel _unloadTran;//城市卡车卸货路点
        private class UnLoadModel 
        {
            public int itemId;
            public int totalVolume;//城市卡车运送总量
            public Vector3 p2;
            public Vector3 v2;
        }
        private List<UnLoadModel> _unLoadModels = new List<UnLoadModel>();

        private enum ForkState
        {
            Idle,
            UnLoadMove,
            Wait,
            UnLoadBack, //从大卡车上卸货返回
            UnLoadWait,//等待卸货到货架上
        }
        private ForkState _forkState;
        public int ItemId { get { return _itemId; } }
        public void Init(int itemId,Transform stopTran)
        {
            _itemId = itemId;
            _stopTran = stopTran;
            _unloadTran = _stopTran.Find("unloadPoint").GetOrAddComponent<IdleTruckStopModel>();
            _unloadTran.Init();
            transform.position = _stopTran.position;
            
            ChangeFork(1);
            SetItemActive(false);
            _animation = _fork.GetComponentByPath<Animation>("fork");
            OrderLayer();
            Idle();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_CityTruck_Arrive, CityTruckUnLoad);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeForkSpeed, SetForkSpeed);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeCityTruckVolume, SetCityTruckTotalVolume);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeForkVolume, SetForkVolume);
        }
        public void CityTruckUnLoad(BaseEventArgs args)
        {
            EventArgsThree<int, Vector3, Vector3> arg = args as EventArgsThree<int, Vector3, Vector3>;
            if (arg.param1 != ItemId) return;
            UnLoadModel unLoadModel = new UnLoadModel();
            unLoadModel.itemId = arg.param1;
            unLoadModel.totalVolume = IdleCityMgr.Instance.GetVolume(PlayerMgr.Instance.PlayerCity.cityTruckVolumeLv) * _truckVolumeTimes;
            unLoadModel.p2 = arg.param2;
            unLoadModel.v2 = arg.param3;
            _unLoadModels.Add(unLoadModel);
            if (_forkState == ForkState.Idle)
                StartCityPath();
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<int, int>(EnumEventType.Event_CityTruck_SetTotalVolume, unLoadModel.itemId, unLoadModel.totalVolume));//设置大货车容量
        }

        private void SetForkVolume(BaseEventArgs args)
        {
            EventArgsOne<int> arg = args as EventArgsOne<int>;
            _forkVolumeTimes = arg.param1;
        }

        private void SetCityTruckTotalVolume(BaseEventArgs args)
        {
            EventArgsOne< int> arg = args as EventArgsOne< int>;
            _truckVolumeTimes = arg.param1;
        }

        private void SetForkSpeed(BaseEventArgs args)
        {
            EventArgsOne<int> arg = args as EventArgsOne<int>;
            _speedTimes = arg.param1;
        }

        //城市卡车卸货寻路点计算
        private void StartCityPath()
        {
            if(_unLoadModels.Count <= 0)
            {
                LogUtility.LogError($"{ItemId}叉车卸货，城市卡车信息为空");
                return;
            }
            UnLoadModel unLoadModel = _unLoadModels[0];
            Vector3 p1 = _unloadTran.GetPoint();
            Vector3 v1 = _unloadTran.GetVector();
            StartPath(p1,v1, unLoadModel.p2, unLoadModel.v2);
            _forkState = ForkState.UnLoadMove;
            _animation.Play();
            _pathIndex = 1;
            NextPoint();
        }
        
        private void StartPath(Vector3 p1,Vector3 v1, Vector3 p2,Vector3 v2)
        {
            _vectorPath.Clear();
            Vector3 startPos = _stopTran.position;
            _vectorPath.Add(startPos);
            _vectorPath.Add(p1);
            Vector3 midVec = Vector3.zero;
            if (Utils.IsLineCross(out midVec, p1, v1, p2, v2))
            {
                _vectorPath.Add(midVec);
                _vectorPath.Add(midVec + v2.normalized * 0.2f);
            }
            else
                LogUtility.LogError($"叉车{ItemId}IsLineCross返回false");
        }

        private void NextPoint()
        {
            _nextPoint = _vectorPath[_pathIndex];
            _offset = _nextPoint - transform.position;
            if (_offset.y <= 0)
            {
                if (_offset.x > 0)
                    ChangeFork(2);
                else
                    ChangeFork(1);
                if (_forkState == ForkState.UnLoadBack)
                {
                    SetItemActive(true, _volume);
                }
                else if (_forkState == ForkState.UnLoadMove)
                {
                    SetItemActive(false);
                }
            }
            else
            {
                
                if (_offset.x > 0)
                    ChangeFork(4);
                else
                    ChangeFork(3);
                if (_forkState == ForkState.UnLoadBack)
                {
                    SetItemActive(true, _volume);
                }
                else if (_forkState == ForkState.UnLoadMove)
                    SetItemActive(false);
            }
        }
        private void ChangeFork(int index)
        {
            Vector3 pos = Vector3.zero;
            if (_fork != null)
            {
                pos = _fork.transform.localPosition;
                SG.ResourceManager.Instance.ReturnObjectToPool(_fork);
            } 
            _fork = IdleForkCtrl.Instance.GetObjectFromPool(index);
            _fork.transform.SetParent(transform);
            _fork.transform.localPosition = pos;
            _forkItemParent = _fork.transform.Find("fork/items").gameObject;
            _forkSprites = transform.GetComponentsInChildren<SpriteRenderer>(true);
        }
        private void SetItemActive(bool isActive,int activeCount = -1)
        {
            if (isActive)
            {
                if (!_forkItemParent.activeSelf)
                    _forkItemParent.SetActive(true);
                for (int i = 0; i < _forkItemParent.transform.childCount; i++)
                {
                    GameObject item = _forkItemParent.transform.GetChild(i)?.gameObject;
                    if (item == null) continue;
                    if(activeCount != -1 && i >= activeCount)
                    {
                        if (item.activeSelf)
                            item.SetActive(false);
                        continue;
                    }
                    if (!item.activeSelf)
                        item.SetActive(true);
                }
            }
            else
            {
                if (_forkItemParent.activeSelf)
                    _forkItemParent.SetActive(false);
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(_forkState == ForkState.UnLoadMove )
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed * _speedTimes, Time.deltaTime);
               
                if (v3.sqrMagnitude >= (_nextPoint - transform.position).sqrMagnitude)
                {
                    transform.position = _nextPoint;
                    _pathIndex++;
                    if (_pathIndex < _vectorPath.Count)
                    {
                        NextPoint();
                    }
                    else
                    {
                        //发消息卸（装）货完成
                        if (_forkState == ForkState.UnLoadMove)
                        {
                            StartCoroutine(UnLoadWait());
                        }
                        _forkState = ForkState.Wait;
                    }
                }
                else
                    transform.position += v3;
            }
            else if(_forkState == ForkState.UnLoadBack)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed * _speedTimes, Time.deltaTime);
                if (v3.sqrMagnitude >= (_nextPoint - transform.position).sqrMagnitude)
                {
                    transform.position = _nextPoint;
                    _pathIndex--;
                    if (_pathIndex >= 0)
                    {
                        NextPoint();
                    }
                    else
                    {
                        Idle();
                    }
                }
                else
                    transform.position += v3;
            }
        }
        IEnumerator UnLoadWait()
        {
            
            yield return new WaitForSeconds(0.5f);
            AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.forkUnloadItem);
            UnLoadModel unLoadModel = _unLoadModels[0];
            _volume = unLoadModel.totalVolume < _maxVolume * _forkVolumeTimes ? unLoadModel.totalVolume : _maxVolume * _forkVolumeTimes;
            unLoadModel.totalVolume -= _volume;
            //if (unLoadModel.totalVolume <= 0)
            //    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_CityTruck_UnLoad, ItemId));
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<int, int>(EnumEventType.Event_CityTruck_UnLoad, ItemId, unLoadModel.totalVolume));
            _pathIndex = _vectorPath.Count - 2;
            NextPoint();
            _forkState = ForkState.UnLoadBack;
            
        }
        void Idle()
        {
            
            if (_forkState == ForkState.UnLoadBack && _unLoadModels.Count >0)
            {
                _forkState = ForkState.UnLoadWait;
                _animation.Stop();
                UnLoadModel unLoadModel = _unLoadModels[0];
                AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.forkUnloadItem);
                ForkIdleUnLoad(unLoadModel,_volume, () =>
                {
                    if (unLoadModel.totalVolume <= 0)
                    {
                        if (_unLoadModels.Contains(unLoadModel))
                            _unLoadModels.Remove(unLoadModel);
                        Timer.Instance.Register(0.5f, (parm) =>
                        {
                            IdleCityCtrl.Instance.TruckLoadItem();
                        }).AddTo(gameObject);
                        ForkIdle();
                    }
                    else
                        StartCityPath();
                });
            }
            else
            {
                ForkIdle();
            }
            
        }
        private void ForkIdle()
        {
            _forkState = ForkState.Idle;
            _animation.Stop();
            Timer.Instance.Register(0.5f, (parm) => {
                if (_unLoadModels.Count > 0)
                {
                    StartCityPath();
                    return;
                }
            }).AddTo(gameObject);

            
        }
        private void ForkIdleUnLoad(UnLoadModel unLoadModel,int count,System.Action callback)
        {
            Timer.Instance.Register(0.2f, (pare) =>
            {
                count--;
                if(count >=0 && count < _forkItemParent.transform.childCount)
                {
                    int num = 1;
                    IdleCityCtrl.Instance.TruckUnloadItem(ItemId, num);
                    if (_forkItemParent.transform.GetChild(count) != null)
                        _forkItemParent.transform.GetChild(count).gameObject.SetActive(false);
                    if (count <= 0)
                        callback();
                    else
                        ForkIdleUnLoad(unLoadModel, count, callback);
                }
                else
                {
                    Debug.LogError($"ForkIdleUnLoad 越界 count={count}");
                    callback();
                }
                
            }).AddTo(gameObject);
        }
        
        private void OrderLayer()
        {
            Timer.Instance.Register(0.1f, -1, (pare) =>
            {
                for(int i =0; i< _forkSprites.Length;i++)
                {
                    SpriteRenderer sr = _forkSprites[i];
                    sr.sortingOrder = -(int)(transform.position.y * 10);
                }
            }).AddTo(gameObject);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_CityTruck_Arrive, CityTruckUnLoad);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeForkSpeed, SetForkSpeed);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeCityTruckVolume, SetCityTruckTotalVolume);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeForkVolume, SetForkVolume);
        }
    }
}

