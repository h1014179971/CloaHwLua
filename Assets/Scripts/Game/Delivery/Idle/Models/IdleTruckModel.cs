using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Pathfinding;

namespace Delivery.Idle
{
    public class TruckItem
    {
        public int itemId;                  
        public int num;
        public IdleSiteModel siteModel;
    }

    public class IdleTruckModel : MonoBehaviour
    {
        private IdleTruck _idleTruck;
        private Sprite _spriteUp;//朝上图片
        private Sprite _spriteDown;//朝下图片
        private bool _isEmpty; //是否是空车状态
        private bool _isUp;//是否是向上状态
        private List<TruckItem> _truckItems = new List<TruckItem>();

        private SpriteRenderer _sprite;
        private Animation _animation;
        private Seeker _seeker;
        private List<Vector3> _vectorPath = new List<Vector3>();
        private Vector3 _nextPoint;
        private Vector3 _nextTargetPoint;
        private Vector3 _target;
        private IdleTruckStopModel _stopModel;
        private IdleTruckStopModel _stopTargetModel;
        private Vector3 _offset;
        private float _speed = 2;
        private float _speedTimes = 1.0f;//速度倍数(记录正常速度和倒车速度)
        private int _specialSpeedTimes = 1;//速度倍数（特殊事件触发时的速度倍数）
        private int _pathIndex;

        private float _rightOffset = 0.3f;

    
        private SpriteRenderer _itemSprite;//货物图标

        //private bool _isMove; //是否正在移动
        private enum TruckState
        {
            Idle,
            CityMove,//向接收区移动
            Move,
            CityBack,//接收区有空位直接返回接收区
            Back,
            Wait //倒车等待
        }
        private TruckState _truckState;
        private TruckItem _targetTruckItem;
        public SpriteRenderer Sprite
        {
            get
            {
                if (_sprite == null)
                    _sprite = this.GetComponentByPath<SpriteRenderer>("truck");
                return _sprite;
            }
        }
        public void Init(IdleTruck truck)
        {
            _idleTruck = truck;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Truck_ChangeVolumeLv, TruckGrade);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_TruckMove, TruckMove);
            _animation = this.GetComponentByPath<Animation>("truck");

           
            _itemSprite = this.GetComponentByPath<SpriteRenderer>("item");

            _seeker = GetComponent<Seeker>();
            _seeker.pathCallback += OnPathComplete;
            _speed = _idleTruck.speed;
            _isEmpty = true;
            _isUp = true;
            ChangeSprite();
            Sprite.sprite = _spriteUp;
            OrderLayer();
            Idle();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeTruckSpeed, SetTruckSpeed);
        } 

        private void SetTruckSpeed(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> args = baseEventArgs as EventArgsOne<int>;
            _specialSpeedTimes = args.param1;
        }


        public int GetTruckState()
        {
            return (int)_truckState;
        }
        //设置出发位置
        public void SetStopModel(IdleTruckStopModel model)
        {
            _stopModel = model;
        }
        private void TruckGrade(BaseEventArgs args)
        {
            IdleTruck idleTruck = PlayerMgr.Instance.GetIdleTruck();
            _idleTruck = idleTruck;
            _speed = _idleTruck.speed;

            Sprite up = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, true);
            if (up != _spriteUp)
            {
                //if(Sprite.sprite == _spriteUp)
                //    Sprite.sprite = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, true);
                //else
                //    Sprite.sprite = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, false);
                Sprite.sprite = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, _isUp);
                _spriteUp = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, true);
                _spriteDown = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, false);
            }

        }
        private void ChangeSprite()
        {
            _spriteUp = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, true);
            _spriteDown = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, false);

            _itemSprite.gameObject.SetActive(!_isEmpty);
        }
        //初始化货物图片
        private void InitItemIcon(int itemId)
        {
            _itemSprite.sprite = AssetHelper.GetItemSprite(itemId);
        }
        //设置货物层级
        private void SetItemOrder()
        {
            _itemSprite.sortingOrder = _sprite.sortingOrder + 1;
        }
        
        public int GetVolume()
        {
            int volume = 0;
            for(int i=0;i< _truckItems.Count; i++)
            {
                volume += _truckItems[i].num;
            }
            volume = _idleTruck.volume - volume;
            LogUtility.LogInfo($"volume==={volume}");
            return volume;
        } 
        void Update()
        {
            if (_truckState != TruckState.Idle && _pathIndex < _vectorPath.Count)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed * _speedTimes, Time.deltaTime);
                if (v3.sqrMagnitude < (_nextTargetPoint - transform.position).sqrMagnitude)
                    transform.position += v3;
                else
                {
                    transform.position = _nextTargetPoint;
                    Vector3 vec = _vectorPath[_pathIndex];
                    _pathIndex++;
                    ///如果相邻两点距离很近，直接下一个目标点
                    if (_pathIndex < _vectorPath.Count && (vec - _vectorPath[_pathIndex]).magnitude < 0.01f)
                        _pathIndex++;
                    ///如果相邻两点距离很近，直接下一个目标点
                    if (_pathIndex < _vectorPath.Count)
                    {
                        NextPoint();
                    }
                    else
                        Next();
                }
                SetItemOrder();
            }
        }
        public void OnPathComplete(Path p)
        {
            Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
            if (!p.error)
            {
                _vectorPath.AddRange(p.vectorPath);
                //_vectorPath = p.vectorPath;
                if (_truckState == TruckState.Back)
                    BackParkStartPath();
                _vectorPath.Add(_target);
                _pathIndex = 0;
                isBackTruck = false;
                startBackTruck = false;
                _speedTimes = 1.0f * _specialSpeedTimes;
                NextPoint();
            }
        }
        private Queue<Vector3> backPosQue = new Queue<Vector3>();
        private bool isBackTruck;//是否开始倒车
        private bool startBackTruck;//是否开始倒车
        private void NextPoint()
        {
            if(backPosQue.Count<=0)
            {
                _nextPoint = _vectorPath[_pathIndex];
                if (_pathIndex >= _vectorPath.Count - 1)
                {
                    _nextTargetPoint = _nextPoint;
                    if (isBackTruck)
                        isBackTruck = false;
                    //if(_truckState==TruckState.Back)
                    //startBack = true;
                }
                else
                {
                    Vector3 dir;
                    Vector3 p1;
                    Vector3 v1;
                    Vector3 v2;
                    Vector3 p2;
                    if (_pathIndex == 0)
                    {
                        if (_stopModel == null)
                        {
                            dir = _nextPoint - transform.position;
                            v1 = VectorRight(dir);
                            p1 = _nextPoint + v1.normalized * _rightOffset;
                            v1 = dir;
                        }
                        else
                        {
                            dir = _stopModel.GetVector();
                            v1 = dir;
                            p1 = _stopModel.GetChildPoint();
                        }
                        v2 = VectorRight(_vectorPath[_pathIndex + 1] - _nextPoint);
                        p2 = _nextPoint + v2.normalized * _rightOffset;
                        v2 = _vectorPath[_pathIndex + 1] - _nextPoint;
                    }
                    else if (_pathIndex == _vectorPath.Count - 2)
                    {
                        dir = _nextPoint - _vectorPath[_pathIndex - 1];
                        v1 = VectorRight(dir);
                        p1 = _nextPoint + v1.normalized * _rightOffset;
                        v1 = dir;
                        v2 = _stopTargetModel.GetVector();
                        p2 = _stopTargetModel.GetChildPoint();
                    }
                    else 
                    {
                        dir = _nextPoint - _vectorPath[_pathIndex - 1];
                        v1 = VectorRight(dir);
                        p1 = _nextPoint + v1.normalized * _rightOffset;
                        v1 = dir;
                        v2 = VectorRight(_vectorPath[_pathIndex + 1] - _nextPoint);
                        p2 = _nextPoint + v2.normalized * _rightOffset;
                        v2 = _vectorPath[_pathIndex + 1] - _nextPoint;
                    }
                    if(!IsLineCross(out _nextTargetPoint, p1, v1, p2, v2))
                        _nextTargetPoint = p1;

                    //对出发点和终点进行位置修正
                    if (_pathIndex == _vectorPath.Count - 2)
                    {
                        if (backPosQue.Count <= 0)
                        {
                            Vector3 moveDir = _nextTargetPoint - transform.position;
                            Vector3 addPoint = _nextTargetPoint + moveDir.normalized * 0.2f;
                            backPosQue.Enqueue(addPoint);
                            backPosQue.Enqueue(_nextTargetPoint);
                            backPosQue.Enqueue(_target);
                        }
                    }
                }
            }
            else
            {
                if (backPosQue.Count == 2)
                {
                    if (!startBackTruck)
                        StartBackTruck();
                    _speedTimes = 0.5f * _specialSpeedTimes;
                }
                _nextTargetPoint = backPosQue.Dequeue();
                if (backPosQue.Count > 0)
                {
                    _pathIndex--;
                }
            }
           
            _offset = _nextTargetPoint - transform.position;
            _nextTargetPoint.z = 0;

            _nextPoint.z = 0;
            _offset.z = 0;
            if (_offset.y <= 0)
            {
                
                if (!isBackTruck && Sprite.sprite != _spriteDown)
                {
                    _isUp = false;
                    Sprite.sprite = _spriteDown;
                }
                else if (isBackTruck && Sprite.sprite != _spriteUp)
                {
                    _isUp = true;
                    Sprite.sprite = _spriteUp;
                }
                if (_offset.x > 0)
                    Sprite.flipX = true;
                else
                    Sprite.flipX = false;
            }  
            else
            {
                
                if (!isBackTruck && Sprite.sprite != _spriteUp)
                {
                    _isUp = true;
                    Sprite.sprite = _spriteUp;
                }
                   
                else if (isBackTruck && Sprite.sprite != _spriteDown)
                {
                    _isUp = false;
                    Sprite.sprite = _spriteDown;
                }
                if (_offset.x > 0)
                    Sprite.flipX = false;
                else
                    Sprite.flipX = true;
            }
        }
        
        //开始倒车
        private void StartBackTruck()
        {
            startBackTruck = true;
            TruckState state = _truckState;
            _truckState = TruckState.Wait;
            isBackTruck = true;
            //yield return new WaitForEndOfFrame();
            _truckState = state;
        }
        
        //private void NextPoint()
        //{
        //    _nextPoint = _vectorPath[_pathIndex];
        //    _offset = _nextPoint - transform.position;
        //    if (_pathIndex >= _vectorPath.Count - 1)
        //        _nextTargetPoint = _nextPoint;
        //    else
        //    {
        //        Vector3 v1 = VectorRight(_offset);
        //        Vector3 p1 = _nextPoint + v1.normalized * _rightOffset;
        //        v1 = _offset;
        //        Vector3 v2 = VectorRight(_vectorPath[_pathIndex + 1] - _nextPoint);
        //        Vector3 p2 = _nextPoint + v2.normalized * _rightOffset;
        //        v2 = _vectorPath[_pathIndex + 1] - _nextPoint;
        //        IsLineCross(out _nextTargetPoint, p1, v1, p2, v2);
        //        _offset = _nextTargetPoint - transform.position;
        //        _nextTargetPoint.z = 0;
        //    }
        //    _nextPoint.z = 0;
        //    _offset.z = 0;
        //    if (_offset.y <= 0)
        //    {
        //        if (Sprite.sprite != _spriteDown)
        //            Sprite.sprite = _spriteDown;
        //        if (_offset.x > 0)
        //            Sprite.flipX = true;
        //        else
        //            Sprite.flipX = false;
        //    }
        //    else
        //    {
        //        if (Sprite.sprite != _spriteUp)
        //            Sprite.sprite = _spriteUp;
        //        if (_offset.x > 0)
        //            Sprite.flipX = false;
        //        else
        //            Sprite.flipX = true;
        //    }
        //}
        private void Next()
        {
            if (_truckState == TruckState.Move)
                //StartCoroutine(NextTarget());
                NextTarget();
            else if(_truckState == TruckState.CityMove)
            {
                _animation.Stop();
                _truckState = TruckState.Idle;
                IdleCityCtrl.Instance.TruckLoadItem();
            }
            else
                Idle();
            AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.siteTruckStop);
        }
        //下一个目标点 
        private void NextTarget()
        {
            if (_truckItems.Count <= 0) return;
            _animation.Stop();
            _targetTruckItem.siteModel.CreateRemover(this);
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, gameObject.name));
        }            
        //IdleRemoverModel脚本中调用
        public void SetNextTarget(IdleRemoverModel removerModel)
        {
            //_targetTruckItem.siteModel.LoadItem(_targetTruckItem.itemId, _targetTruckItem.num);
            removerModel.SetItem(_targetTruckItem.itemId, _targetTruckItem.num);
            _truckItems.RemoveAt(0);
            if (_truckItems.Count > 0)
            {
                _targetTruckItem = _truckItems[0];
                _stopModel = null;
                _stopTargetModel = _targetTruckItem.siteModel.GetUnLoadItemStopModel();
                _target = _stopTargetModel.GetPoint();
            }
            else
            {
                
                _stopModel = _stopTargetModel;
                _isEmpty = true;
                IdleCityCtrl.Instance.ChoiceParkPoint(new List<int>(), this);
                if(_truckState != TruckState.CityMove)
                {
                    _truckState = TruckState.Back;
                    _stopTargetModel = IdleCityCtrl.Instance.UseParkPoint(this);
                    _target = _stopTargetModel.GetPoint();
                }
                
                
            }
            if(_truckState != TruckState.CityMove)
            {
                ChangeSprite();
                Sprite.sprite = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, _isUp);
                TimerEvent timer = Timer.Instance.Register(0.5f, 1, (pare) =>
                {
                    _vectorPath.Clear();
                    _seeker.StartPath(transform.position, _target);
                    _animation.Play();
                }).AddTo(gameObject);
            }
        }
        public IdleTruck IdleTruck { get { return _idleTruck; } } 
        private void Idle()
        {
            _truckState = TruckState.Idle;
            _truckItems.Clear();
            _animation.Stop();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleTruckModel>(EnumEventType.Event_Idle_TruckBack,this));
        
        }                                                           
        //装货
        public void LoadItem(IdleSiteModel siteModel,int loadNum)
        {    
            TruckItem truckItem = new TruckItem();
            truckItem.itemId = siteModel.IdleSite.itemId;
            truckItem.siteModel = siteModel;
            truckItem.num = loadNum;
            _truckItems.Add(truckItem);

            InitItemIcon(truckItem.itemId);//初始化货物图片
        }
        //装了多少货
        public int LoadItemNum()
        {
            int num = 0;
            for(int i=0;i< _truckItems.Count; i++)
            {
                TruckItem truckItem = _truckItems[i];
                num += truckItem.num;
            }
            return num;
        }
        public bool IsMove { get { return _truckState == TruckState.Move; } }
        public void Move()
        {
            _animation.Stop();
            if (_truckItems.Count > 0)
            {
                Vector3 pos = transform.position;
                for (int i = 0; i < _truckItems.Count - 1; i++)
                {
                    for (int j = i + 1; j < _truckItems.Count; j++)
                    {
                        IdleSiteModel siteModel1 = _truckItems[i].siteModel;
                        float dis1 = Vector3.Distance(pos, siteModel1.transform.position);
                        IdleSiteModel siteModel2 = _truckItems[j].siteModel;
                        float dis2 = Vector3.Distance(pos, siteModel2.transform.position);
                        if (dis2 < dis1)
                        {
                            TruckItem truckItem = _truckItems[i];
                            _truckItems[i] = _truckItems[j];
                            _truckItems[j] = truckItem;
                        }
                    }
                }

                _truckState = TruckState.Move;
                if (_stopTargetModel != null)
                    _stopModel = _stopTargetModel;
                _targetTruckItem = _truckItems[0];
                ChangeSprite();
                //_target = _targetTruckItem.siteModel.transform.position;
                _stopTargetModel = _targetTruckItem.siteModel.GetUnLoadItemStopModel();
                _target = _stopTargetModel.GetPoint();
                
                //Timer.Instance.Register(2, 1, (pare) => {
                    
                //    _animation.Play();
                //    _seeker.StartPath(transform.position, _target);
                //    IdleCityCtrl.Instance.RemoveParkPoint(this);
                //});
                
            }
            else
                _truckState = TruckState.Idle;
        }
        public void CityMove(IdleTruckStopModel stopModel)
        {
            //TimerEvent timer = Timer.Instance.Register(0.5f, 1, (pare) =>
            //{
                
            //}).AddTo(gameObject);
            _truckState = TruckState.CityMove;
            ChangeSprite();
            if(_stopTargetModel != null)
                _stopModel = _stopTargetModel;
            _stopTargetModel = stopModel;
            _target = _stopTargetModel.GetPoint();
            _animation.Play();
            if (IdleCityCtrl.Instance.IsWaitParkPoint(_stopModel))
            {
                Vector3 outVec = Vector3.zero;
                List<Vector3> points = OutParkStartPath(out outVec);
                if (points != null)
                {
                    //for (int i = points.Count - 1; i >= 0; i--)
                    //{
                    //    _vectorPath.Insert(0, points[i]);
                    //}
                    _vectorPath.Clear();
                    _vectorPath.AddRange(points);
                    _seeker.StartPath(outVec, _target);
                    
                }
                else
                {
                    _vectorPath.Clear();
                    _seeker.StartPath(transform.position, _target);
                }
            }
            else
            {
                _vectorPath.Clear();
                _seeker.StartPath(transform.position, _target);
            }
                
            IdleCityCtrl.Instance.RemoveWaitPoint(this);


        }
        private void TruckMove(BaseEventArgs args)
        {
            EventArgsThree<IdleTruckModel, int, int> arg = args as EventArgsThree<IdleTruckModel,int, int>;
            if (arg.param1 != this) return;
            _isEmpty = false;
            ChangeSprite();
            Sprite.sprite = AssetHelper.GetTruckSprite(PlayerMgr.Instance.GetTruckLv(), _isEmpty, _isUp);
            TimerEvent timer =Timer.Instance.Register("timer", 0.5f, 1,true, (pare) =>
            {
                _animation.Play();
                _vectorPath.Clear();
                _seeker.StartPath(transform.position, _target);
                IdleCityCtrl.Instance.RemoveParkPoint(this);
            }).AddTo(gameObject);
            
        }
        private List<Vector3> OutParkStartPath(out Vector3 vector)
        {
            if (_stopModel.RowId == 0)
            {
                vector = Vector3.zero;
                return null;
            }
            List<Vector3> points = null;
            if (_stopTargetModel.GetPoint().x < _stopModel.GetPoint().x)
            {
                //左边出
                points = IdleCityCtrl.Instance.OutPark(_stopModel, true);
                vector = IdleCityCtrl.Instance.GetLeftEnterAreaPoint();
            }
            else
            {
                //右边出
                points = IdleCityCtrl.Instance.OutPark(_stopModel, false);
                vector = IdleCityCtrl.Instance.GetRightEnterAreaPoint();
            }
                
            return points;
        }
        /// <summary>
        /// 返回停车场规划停车场路线
        /// </summary>
        private void BackParkStartPath()
        {
            if (_stopTargetModel.RowId == 0) return;
            Vector3 endVec = _vectorPath[_vectorPath.Count - 1];
            Vector3 nextEndVec = _vectorPath[_vectorPath.Count - 2];
            Vector3 offset = endVec - nextEndVec;
            if(endVec.x < IdleCityCtrl.Instance.GetLeftEnterAreaPoint().x)
            {
                if(offset.x < 0)
                {
                    for(int i = _vectorPath.Count - 1; i >= 0; i--)
                    {
                        Vector3 vec = _vectorPath[i];
                        if (vec.x < IdleCityCtrl.Instance.GetLeftEnterAreaPoint().x)
                            _vectorPath.RemoveAt(i);
                    }
                }
                //从左边点进
                List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
                _vectorPath.AddRange(points);
            }
            else if(endVec.x > IdleCityCtrl.Instance.GetRightEnterAreaPoint().x)
            {
                if(offset.x >0)
                {
                    for (int i = _vectorPath.Count - 1; i >= 0; i--)
                    {
                        Vector3 vec = _vectorPath[i];
                        if (vec.x > IdleCityCtrl.Instance.GetRightEnterAreaPoint().x)
                            _vectorPath.RemoveAt(i);
                    }
                }
                //从右边点进
                List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
                _vectorPath.AddRange(points);
            }
            else
            {
                Vector3 leftPoint = IdleCityCtrl.Instance.GetLeftEnterAreaPoint();
                Vector3 rightPoint = IdleCityCtrl.Instance.GetRightEnterAreaPoint();
                Vector3 midPoint = (rightPoint + leftPoint) * 0.5f;
                if (offset.x >= 0)
                {
                    if (endVec.x >= midPoint.x)
                    {
                        //从右边点进
                        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
                        _vectorPath.AddRange(points);
                    }
                    else
                    {
                        if(_vectorPath[0].x > leftPoint.x)
                        {
                            //起始点就在左点和中点之间，直接向右行驶
                            //从右边点进
                            List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
                            _vectorPath.AddRange(points);
                        }
                        else
                        {
                            for (int i = _vectorPath.Count - 1; i >= 0; i--)
                            {
                                Vector3 vec = _vectorPath[i];
                                if (vec.x > leftPoint.x)
                                    _vectorPath.RemoveAt(i);
                            }
                            //从左边点进
                            List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
                            _vectorPath.AddRange(points);
                        }
                        
                    }
                }
                else
                {
                    if (endVec.x <= midPoint.x)
                    {
                        //从左边点进
                        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
                        _vectorPath.AddRange(points);
                    }
                    else
                    {
                        if (_vectorPath[0].x < rightPoint.x)
                        {
                            //起始点就在右点和中点之间，直接向左行驶
                            //从左边点进
                            List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
                            _vectorPath.AddRange(points);
                            
                        }
                        else
                        {
                            for (int i = _vectorPath.Count - 1; i >= 0; i--)
                            {
                                Vector3 vec = _vectorPath[i];
                                if (vec.x < rightPoint.x)
                                    _vectorPath.RemoveAt(i);
                            }
                            //从右边点进
                            List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
                            _vectorPath.AddRange(points);
                        }
                    }
                }
            }
            //if(offset.x >0 )
            //{
            //    Vector3 enterPoint = IdleCityCtrl.Instance.GetLeftEnterAreaPoint();
            //    if(endVec.x < enterPoint.x)
            //    {
            //        //从左边点进
            //        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
            //        _vectorPath.AddRange(points);
            //    }
            //    else
            //    {
            //        //从右边点进
            //        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
            //        _vectorPath.AddRange(points);
            //    }
            //}
            //else
            //{
            //    Vector3 enterPoint = IdleCityCtrl.Instance.GetRightEnterAreaPoint();
            //    if (endVec.x < enterPoint.x)
            //    {
            //        //从左边点进
            //        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, true);
            //        _vectorPath.AddRange(points);
            //    }
            //    else
            //    {
            //        //从左边点进
            //        List<Vector3> points = IdleCityCtrl.Instance.EnterPark(_stopTargetModel, false);
            //        _vectorPath.AddRange(points);
            //    }
            //}
        }
        private Vector3 VectorRight(Vector3 v3)
        {
            return Quaternion.AngleAxis(-90, Vector3.forward) * v3;
        }
        /// <summary>
        /// 判断两直线是否相交
        /// </summary>
        /// <param name="result">交点</param>
        /// <param name="p1">线段1上的点</param>
        /// <param name="v1">向量1</param>
        /// <param name="p2">线段2上的点</param>
        /// <param name="v2">向量2</param>
        /// <returns></returns>
        private bool IsLineCross(out Vector3 result, Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2)
        {
            result = Vector3.zero;
            float dot = Vector3.Dot(v1.normalized, v2.normalized);
            if (dot==1 || dot==-1)
            {
                // 两线平行
                return false;
            }
            Vector3 startPointSeg = p2 - p1;
            Vector3 vecS1 = Vector3.Cross(v1, v2);            // 有向面积1
            Vector3 vecS2 = Vector3.Cross(startPointSeg, v2); // 有向面积2
            float num = Vector3.Dot(startPointSeg, vecS1);
            // 判断两这直线是否共面
            if (num >= 1E-05f || num <= -1E-05f)
            {
                return false;
            }
            // 有向面积比值，利用点乘是因为结果可能是正数或者负数
            if (vecS1 == Vector3.zero) return false ;
            float num2 = Vector3.Dot(vecS2, vecS1) / vecS1.sqrMagnitude; 
            result = p1 + v1 * num2;
            return true;
        }
        private void OrderLayer()
        {
            Timer.Instance.Register(0.5f, -1, (pare) => {
                _sprite.sortingOrder = -(int)(_sprite.transform.position.y * 10);
            }).AddTo(gameObject);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            SG.ResourceManager.Instance.ReturnObjectToPool(gameObject);
        }
        private void OnDestroy()
        {
            if(_seeker != null)
                _seeker.pathCallback -= OnPathComplete;
            StopAllCoroutines();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Truck_ChangeVolumeLv, TruckGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_TruckMove, TruckMove);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeTruckSpeed, SetTruckSpeed);
        }
    }
}

