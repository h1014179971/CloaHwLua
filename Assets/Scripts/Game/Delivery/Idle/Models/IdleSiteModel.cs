using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using UnityEditor;
using libx;

namespace Delivery.Idle
{
    public class IdleSiteModel : MonoBehaviour
    {
        [SerializeField] private int _siteId;
        [SerializeField] private List<IdleSiteBuildModel> _idleSiteBuildModels;
        public Transform siteArea;//驿站所在区域的根节点(用于设置该驿站所处区域的所有图片颜色)
        private SpriteRenderer _spriteRenderer = null;
        private MyText _viewMoney;
        private GameObject _weijianObj;
        private IdleTruckStopModel _unloadItemStopModel;
        private List<Vector3> _staffWaitPoints = new List<Vector3>();
        private Vector3 _offset;//单位偏移量
        private float _staffDis = 0.5f;//等待快递员间隙
        private Transform _staffLine;
        private int _lineCount;//路线的数量
        private List<IdleStaffModel> _idleStaffModels = new List<IdleStaffModel>(); //总的快递员
        private List<IdleStaffModel> _waitStaffModels = new List<IdleStaffModel>();//等待快递员
        private PlayerSite _playerSite;
        private IdleSite _idleSite;
        private IdleSiteBase _idleSiteBase;
        private IdleSiteGrade _idleSiteGrade;
        private IdleSiteTime _idleSiteTime;
        private IdleSiteVolume _idleSiteVolume;

        private Transform removerPointRoot;
        private float lastPlayFxTime;//上次播放升级特效的时间
        private float dPlayFxTime=0.2f;//播放特效的时间间隔
        private float lastPlayVolumeFxTime;//上次播放升级容量特效的时间
        private float lastPlayTimeFxTime;//上次播放升级快递员特效时间
        private Transform levelUpFxNode;//升级特效节点
        private Animation levelUpAnim;//升级动画
        private Transform shelfNode;//货架父节点
        //private GameObject _shelfShadowNode;//货架蒙版节点
        //private int shelfMaxCount;//驿站货架总数
        //private List<GameObject> shelfStoreys;//所有货架层
        private List<IdleShelfModel> _allShelfModel = new List<IdleShelfModel>();//驿站所有货架

        private Transform _unlockNode;//解锁时显示的所有物体的节点
        private GameObject _greenVolumeScreen;//绿色显示屏
        private GameObject _redVolumeScreen;//红色显示屏

        private IdlePostSiteBuidingModel _buildingModel;//驿站建筑

        //private IdleShelfModel shelfModel;

        private Transform _lock;//解锁按钮
        private SpriteRenderer _lockSprite;//解锁按钮背景图
        private MyText _lockPrice;//解锁花费

        private TimerEvent _truckTipTimer;//升级车站操作提示计时器
        private TimerEvent _postSiteTipTimer;//升级驿站操作提示计时器
        private float _tipTime=30f;//操作提示计时器时间

        public int SiteId { get { return _siteId; } }
        public PlayerSite PlayerSite { get { return _playerSite; } }
        public IdleSite IdleSite { get { return _idleSite; } }
        public IdleSiteBase IdleSiteBase { get { return _idleSiteBase; } }
        public IdleSiteGrade IdleSiteGrade { get { return _idleSiteGrade; } }
        public IdleSiteTime IdleSiteTime { get { return _idleSiteTime; } }
        public IdleSiteVolume IdleSiteVolume { get { return _idleSiteVolume; } }
        public bool CanUnlock
        {
            get;
            private set;
        }
        public void Init(PlayerSite playerSite)
        {
            CanUnlock = false;
            _playerSite = playerSite;
            _idleSite =  IdleSiteMgr.Instance.GetIdleSite(_playerSite.Id,_playerSite.cityId);
            _idleSiteBase = IdleSiteMgr.Instance.GetSiteBase(SiteId, _playerSite.siteBaseLv);
            _idleSiteGrade = IdleSiteMgr.Instance.GetSiteGradeBySiteId(SiteId, _playerSite.siteBaseLv);
            _idleSiteTime = IdleSiteMgr.Instance.GetSiteTime(SiteId, _playerSite.siteTimeLv);
            _idleSiteVolume = IdleSiteMgr.Instance.GetSiteVolume(SiteId, _playerSite.siteVolumeLv);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_BaseGrade, BaseGrade);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_TimeGrade, TimeGrade);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_VolumeGrade, VolumeGrade);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _unlockNode = this.GetComponentByPath<Transform>("building/unLockNode");
            //_viewMoney = this.GetComponentByPath<MyText>("building/Fx_UI_SiteMoney/money_text");
            _viewMoney = _unlockNode.GetComponentByPath<MyText>("Fx_UI_SiteMoney/money_text");
            //_viewMoney.text = _playerSite.loadItemNum.ToString();
            _viewMoney.text = (_idleSiteVolume.volume - _playerSite.loadItemNum).ToString();
            _weijianObj = transform.Find("weijian").gameObject;
            _weijianObj.SetActive(false);
            //_shelfShadowNode = transform.Find("building/shelfShadowNode").gameObject;
            //_shelfShadowNode.SetActive(false);
            Transform unloadItemTrans = transform.Find("unloadItemPoint");
            _unloadItemStopModel = unloadItemTrans.GetOrAddComponent<IdleTruckStopModel>();
            _unloadItemStopModel.Init();
            if (_playerSite.isLock)
                SpriteHide();
            LoadPoints();

            _lock = transform.Find("lock");
            if (_lock != null)
            {
                _lockSprite = _lock.GetComponent<SpriteRenderer>();
                _lockPrice = _lock.GetComponentByPath<MyText>("Canvas/cost");
            }
            
            if (!_playerSite.isLock)
            {
                CreateStaff(_idleSiteGrade.staffnum);
                InitBuilding();
            }
            else
            {
                SetAreaColor(true);
                _unlockNode.gameObject.SetActive(false);
            }

            //InitBuilding();//初始化驿站建筑

            removerPointRoot = transform.Find("removerPoints");
            lastPlayFxTime = Time.time;
            lastPlayVolumeFxTime = Time.time;
            lastPlayTimeFxTime = Time.time;
            string fxNodeName = "map/postSiteFxNodes/postSite" + SiteId.ToString();
            levelUpFxNode = GameObject.Find(fxNodeName).transform;
            levelUpAnim = this.GetComponentByPath<Animation>("building");

            //_greenVolumeScreen = this.GetComponentByPath<Transform>("building/jijiaqi/green").gameObject;
            //_redVolumeScreen = this.GetComponentByPath<Transform>("building/jijiaqi/red").gameObject;
            _greenVolumeScreen = _unlockNode.GetComponentByPath<Transform>("jijiaqi/green").gameObject;
            _redVolumeScreen = _unlockNode.GetComponentByPath<Transform>("jijiaqi/red").gameObject;

            InitShelf(_playerSite.isLock);//初始化货架


            
          
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateLockSprite);
            //EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnPostSiteUnlock);
        }

        private void InitBuilding()
        {
            CreateBuildingModel(IdleSiteGrade.prefabName);
            _buildingModel.ShowAllItems(IdleSiteGrade.itemIndex);
            _unlockNode.gameObject.SetActive(true);
            SetAreaColor(false);
        }

        private void SetAreaColor(bool isLock)
        {
            if(siteArea!=null)
            {
                Color color = Color.white;
                if(isLock)
                {
                    ColorUtility.TryParseHtmlString("#B4B4B4", out color);
                }
                SpriteRenderer[] sprites = siteArea.GetComponentsInChildren<SpriteRenderer>();
                for(int i=0;i<sprites.Length;i++)
                {
                    sprites[i].color = color;
                }
                //解锁按钮不置灰
                if(_lock!=null && isLock)
                {
                    SpriteRenderer[] lockSprites = _lock.GetComponentsInChildren<SpriteRenderer>();
                    for(int i=0;i<lockSprites.Length;i++)
                    {
                        lockSprites[i].color = Color.white;
                    }
                }
            }
        }

        //创建驿站建筑
        private void CreateBuildingModel(string prefabName)
        {
            if(_buildingModel!=null)
            {
                Destroy(_buildingModel.gameObject);
            }
            GameObject buildingPrefab = AssetLoader.Load<GameObject>(prefabName + ".prefab");
            if (buildingPrefab == null) return;

            GameObject building = GameObject.Instantiate(buildingPrefab);
            Transform buidingParent = this.GetComponentByPath<Transform>("building");
            building.transform.SetParent(buidingParent,false);
            _buildingModel = building.GetOrAddComponent<IdlePostSiteBuidingModel>();
            _buildingModel.Init(prefabName);
        }
        //更新建筑（升级时调用）
        private void UpdateBuilding()
        {
            if (_buildingModel == null) return;
            if(_buildingModel.PrefabName==IdleSiteGrade.prefabName)
            {
                _buildingModel.ShowItems(IdleSiteGrade.itemIndex);
            }
            else
            {
                CreateBuildingModel(IdleSiteGrade.prefabName);
            }
        }
        


        public void ShowLock()
        {
            if (_lock == null) return;
            _lock.gameObject.SetActive(true);
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(SiteId);
            Long2 unlockCost = new Long2(idleSite.unlockPrice);
            Long2 playerMoney = PlayerMgr.Instance.PlayerCity.money;
            CanUnlock = playerMoney >= unlockCost;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_Site_CanUnlock, CanUnlock));
            _lockPrice.text = UnitConvertMgr.Instance.GetFloatValue(unlockCost, 2);
        }
        public void HideLock()
        {
            if (_lock == null) return;
            _lock.gameObject.SetActive(false);
        }

        private void UpdateLockSprite(BaseEventArgs baseEventArgs)
        {
            if (_lock==null || !_lock.gameObject.activeSelf) return;
            ShowLock();
        }
        //private void OnPostSiteUnlock(BaseEventArgs baseEventArgs)
        //{
        //    EventArgsOne<IdleSiteModel> args = (EventArgsOne<IdleSiteModel>)baseEventArgs;
        //    if (args.param1.SiteId != _playerSite.Id) return;
        //    InitBuilding();//初始化驿站建筑
        //    InitShelf(_playerSite.isLock);
        //    UpdateScreen();
        //}
        //更新显示屏
        private void UpdateScreen()
        {
            if (_playerSite.isLock)
            {
                _redVolumeScreen.SetActive(false);
                _greenVolumeScreen.SetActive(false);
                return;
            }
            int currentStore = _playerSite.loadItemNum;
            if (currentStore >= IdleSiteVolume.volume)
            {
                _redVolumeScreen.SetActive(true);
                _greenVolumeScreen.SetActive(false);
            }
            else
            {
                _redVolumeScreen.SetActive(false);
                _greenVolumeScreen.SetActive(true);
            }
        }
        //初始化货架
        private void InitShelf(bool isLock)
        {
            shelfNode = this.GetComponentByPath<Transform>("building/shelfNode");
            if (shelfNode == null || isLock) return;

            string shelfRes = IdleSiteVolume.shelfRes;
            if (string.IsNullOrEmpty(shelfRes)) return;
            string[] shelfIdArr = shelfRes.Split(',');
            
            for(int i=0;i<shelfIdArr.Length;i++)
            {
                Transform shelfParent = shelfNode.Find("shelf" + i.ToString());
                int shelfId = int.Parse(shelfIdArr[i]);
                IdleShelfModel shelfModel = new IdleShelfModel(shelfId,IdleSite.itemId ,shelfParent);
                _allShelfModel.Add(shelfModel);
            }
            UpdateShelfStorey(false);
        }

        //更新货架货物资源
        private void UpdateShelfStorey(bool showFx=true)
        {
            int currentVolume = _playerSite.loadItemNum;
            for (int i=0;i<_allShelfModel.Count;i++)
            {
                int shelfMaxItemCount = _allShelfModel[i].MaxItemCount;
                int itemCount = shelfMaxItemCount < currentVolume ? shelfMaxItemCount : currentVolume;
                _allShelfModel[i].SetItemCount(itemCount, showFx);
                currentVolume -= shelfMaxItemCount;
                if (currentVolume < 0) currentVolume = 0;
            }
            UpdateScreen();
            
        }
        //更新货架资源
        private void UpdateShelfRes()
        {
            string shelfRes = IdleSiteVolume.shelfRes;
            if (string.IsNullOrEmpty(shelfRes)) return;
            string[] shelfIdArr = shelfRes.Split(',');
            
            int currentShelfCount = _allShelfModel.Count;
            for (int i = 0; i < shelfIdArr.Length; i++)
            {
                int shelfId = int.Parse(shelfIdArr[i]);
                if (i < currentShelfCount)
                {
                    _allShelfModel[i].UpdateShelfRes(shelfId);
                }
                else
                {
                    Transform shelfParent = shelfNode.Find("shelf" + i.ToString());
                    IdleShelfModel shelfModel = new IdleShelfModel(shelfId, IdleSite.itemId, shelfParent);
                    _allShelfModel.Add(shelfModel);
                    shelfModel.PlayShelfAni(true);
                }
            }
        }


        public void SpriteHide() {
            _viewMoney.enabled = false;
            _weijianObj.SetActive(true);
        }
        public void SpriteShow()
        {
            //_spriteRenderer.enabled = true;
            _viewMoney.enabled = true;
            _weijianObj.SetActive(false);

            InitBuilding();//初始化驿站建筑
            InitShelf(_playerSite.isLock);
            UpdateScreen();
        }
        private void BaseGrade(BaseEventArgs args)
        {
            EventArgsOne<IdleSiteModel> arg = args as EventArgsOne<IdleSiteModel>;
            if (arg.param1 != this) return;
            int lastGradeId = _idleSiteGrade.Id;
            _idleSiteBase = IdleSiteMgr.Instance.GetSiteBase(SiteId, _playerSite.siteBaseLv);
            _idleSiteGrade = IdleSiteMgr.Instance.GetSiteGradeBySiteId(SiteId, _playerSite.siteBaseLv);
            if(lastGradeId!=_idleSiteGrade.Id)
            {
                UpdateBuilding();
            }

            if(_idleSiteGrade.staffnum > _idleStaffModels.Count)
            {
                CreateStaff(_idleSiteGrade.staffnum - _idleStaffModels.Count);
            }

            if(Time.time-lastPlayFxTime>dPlayFxTime)
            {
                lastPlayFxTime = Time.time;
                //Transform fxNode = GameObject.Find("map/postSiteFxNodes/postSite1").transform;
                FxCtrl.Instance.PlayFx(FxPrefabPath.idleSiteGrade, levelUpFxNode, false, 0.35f);
                if (levelUpAnim != null)
                    levelUpAnim.Play();
            }
           
        }
        private void TimeGrade(BaseEventArgs args)
        {
            EventArgsOne<IdleSiteModel> arg = args as EventArgsOne<IdleSiteModel>;
            if (arg.param1 != this) return;    
            _idleSiteTime = IdleSiteMgr.Instance.GetSiteTime(SiteId, _playerSite.siteTimeLv);
            //FxCtrl.Instance.PlayFx(FxPrefabPath.idleSiteGrade, transform,true, 0.5f);
            PlayTimeGradeFx();
        }

        private void PlayTimeGradeFx()
        {
            if (Time.time - lastPlayTimeFxTime <= dPlayFxTime) return;
            lastPlayTimeFxTime = Time.time;
            for(int i=0;i<_idleStaffModels.Count;i++)
            {
                Transform staffTrans = _idleStaffModels[i].transform;
                FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, staffTrans, false, 0.35f);
                model.transform.localScale = Vector3.one*2;
                Vector3 localPos = Vector3.zero;
                //localPos.y -= 0.2f;
                model.transform.localPosition = localPos;
            }
        }

        private void VolumeGrade(BaseEventArgs args)
        {
            EventArgsOne<IdleSiteModel> arg = args as EventArgsOne<IdleSiteModel>;
            if (arg.param1 != this) return;
            _idleSiteVolume = IdleSiteMgr.Instance.GetSiteVolume(SiteId,_playerSite.siteVolumeLv);
            _viewMoney.text = (_idleSiteVolume.volume - _playerSite.loadItemNum).ToString();
            UpdateShelfRes();
            UpdateShelfStorey(false);

            //shelfModel.PlayShelfAni();
            //for (int i=0;i<_allShelfModel.Count;i++)
            //{
            //    _allShelfModel[i].PlayShelfAni();
            //}
        }
        //播放容量升级特效
        private void PlayVolumeGradeFx()
        {
            if (Time.time - lastPlayVolumeFxTime <= dPlayFxTime) return;
            lastPlayVolumeFxTime = Time.time;
            for(int i=0;i<shelfNode.childCount;i++)
            {
                Vector3 pos = shelfNode.GetChild(i).position;
                pos.y -= 0.36f;
                FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, pos,0.35f);
                model.transform.localScale = Vector3.one;
            }
        }

        public IdleTruckStopModel GetUnLoadItemStopModel()
        {
            return _unloadItemStopModel;
        }
        private void LoadPoints()
        {
            Transform staffWaitPoint = transform.Find("staffWaitPoint");
            for (int i = 0; i < staffWaitPoint?.childCount; i++)
            {
                Vector3 vec = staffWaitPoint.GetChild(i).position;
                vec.z = 0;
                _staffWaitPoints.Add(vec);
            }
            if(_staffWaitPoints.Count > 0)
            {
                _offset = _staffWaitPoints[_staffWaitPoints.Count - 1] - _staffWaitPoints[0];
                _offset = _offset.normalized;
            }

            _staffLine = transform.Find("staffLine");
            _lineCount = _staffLine.childCount;
        } 
        //解锁后创建快递员
        public void CreateStaff()
        {
            if (!_playerSite.isLock && _playerSite.Id == SiteId)
                CreateStaff(_idleSiteGrade.staffnum);
        }
        //创建快递员
        private void CreateStaff(int count)
        {
            if (_staffWaitPoints.Count <= 0) return;
            for(int i = 0; i < count; i++)
            {
                string prefabPath = PrefabPath.idlePath + PrefabName.staff;
                GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.staff, true, 1);
                obj.transform.SetParent(transform);
                obj.transform.position = IdlePoint();
                obj.name = prefabPath.Replace('/', '_');
                IdleStaffModel model = obj.GetOrAddComponent<IdleStaffModel>();
                int lineIndex = _idleStaffModels.Count % _lineCount;
                model.Init(this, _staffLine?.Find("line" + lineIndex),lineIndex);
                //model.Init(this, _staffLine?.Find("line" + _idleStaffModels.Count));
                _idleStaffModels.Add(model);
                
            }
            
        }
        private Vector3 IdlePoint()
        {    
            return _staffWaitPoints[0] + _offset * _staffDis * _waitStaffModels.Count;
        }  
        public Vector3 WaitPoint()
        {
            return _staffWaitPoints[0];
        }
        //添加等到快递员
        public void AddIdleStaff(IdleStaffModel model)
        {
            if (!_waitStaffModels.Contains(model))
            {
                _waitStaffModels.Add(model);
                StaffEndLine();
            }
                
        }     
        public void RemoveIdleStaff(IdleStaffModel model)
        {
            if (_waitStaffModels.Contains(model))
            {
                _waitStaffModels.Remove(model);
                StaffEndLine();
            }
                
        }
        //重新排队 
        private void StaffEndLine()
        {
            for(int i = 0; i < _waitStaffModels.Count; i++)
            {
                Vector3 target = _staffWaitPoints[0] + _offset * _staffDis * i;
                _waitStaffModels[i].SetIdleTarget(target);
            }
        }                                            
        //创建搬运工
        public  void CreateRemover(IdleTruckModel idleTruckModel)
        {
            string prefabPath = PrefabPath.idlePath + PrefabName.remover;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.remover, true, 1);
            obj.transform.SetParent(transform,true);
            obj.transform.position = removerPointRoot.Find("startPos").position;
            obj.name = prefabPath.Replace('/', '_');
            IdleRemoverModel model = obj.GetOrAddComponent<IdleRemoverModel>();
            model.Init(this, idleTruckModel, removerPointRoot);
        }
        //自己收货
        public void LoadItem(int itemId,int num)
        {
            if (IdleSite.itemId != itemId)
            {
                LogUtility.LogError($"送货{itemId}与驿站{IdleSite.itemId}接收货物类型不一致");
                return;
            }
            int loadNum = _playerSite.loadItemNum;
            int siteVolume = IdleSiteVolume.volume;
            int mulNum = siteVolume - _playerSite.loadItemNum;
            num = num < mulNum ? num : mulNum;
            _playerSite.loadItemNum += num;
            //_viewMoney.text = _playerSite.loadItemNum.ToString();
            _viewMoney.text = (siteVolume - _playerSite.loadItemNum).ToString();

            if (_truckTipTimer != null )
            {
                Timer.Instance.DestroyTimer(_truckTipTimer);
                _truckTipTimer = null;
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_ActionTip_CancelToLevelUpTruck));
            }
            
            SetLevelUpPostSiteTip(_playerSite.loadItemNum, siteVolume);

            if (_playerSite.loadItemNum > 0 )
            {
                Timer.Instance.Register(0.5f, 1, (para) =>
                {
                    StaffEndLine();
                }).AddTo(gameObject);
            }
               
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Site_ChangeRest, SiteId));

            UpdateShelfStorey();
        }

        private void SetLevelUpPostSiteTip(int currentVolume,int siteVolume)
        {
            float lPercent = (float)currentVolume / siteVolume;
            float percent = lPercent;
            if (percent >= 0.7f && _postSiteTipTimer == null)
            {
                _postSiteTipTimer = Timer.Instance.Register(_tipTime, OnLevelupPostSiteTipStart);
            }
            else if(percent<0.7f&&_postSiteTipTimer!=null)
            {
                Timer.Instance.DestroyTimer(_postSiteTipTimer);
                _postSiteTipTimer = null;
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_ActionTip_CancelToLevelUpPostSite,_playerSite.Id));
            }
        }

        private void OnLevelupPostSiteTipStart(params object[] objs)
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_ActionTip_NeedToLevelUpPostSite,_playerSite.Id));
            Timer.Instance.DestroyTimer(_postSiteTipTimer);
            _postSiteTipTimer = null;
        }

        public void LoadItem(string itemId, int num)
        {
            LoadItem(int.Parse(itemId), num);
        }
        //卸货
        public void UnLoadItem(int num)
        {
            
            _playerSite.loadItemNum -= num;
            _playerSite.loadItemNum = 0 < _playerSite.loadItemNum ? _playerSite.loadItemNum : 0;
            _viewMoney.text = (_idleSiteVolume.volume - _playerSite.loadItemNum).ToString();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Site_ChangeRest, SiteId));

            if(_playerSite.loadItemNum<=0)
            {
                if(_truckTipTimer == null)
                {
                    _truckTipTimer = Timer.Instance.Register(_tipTime, OnLevelupTruckTipStart);
                }
            }

            int siteVolume = _idleSiteVolume.volume;
            SetLevelUpPostSiteTip(_playerSite.loadItemNum, siteVolume);

            UpdateShelfStorey();
        }

        private void OnLevelupTruckTipStart(params object[]objs)
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_ActionTip_NeedToLevelUpTruck));
            Timer.Instance.DestroyTimer(_truckTipTimer);
            _truckTipTimer = null;
        }

        public int GetItemNum()
        {                
            return _playerSite.loadItemNum;
        }
        /// <summary>
        /// 寻找对应的buildModel
        /// </summary>
        /// <param name="buildId"></param>
        /// <returns></returns>
        public IdleSiteBuildModel GetSiteBuildModel(int buildId)
        {
            if(_idleSiteBuildModels == null || _idleSiteBuildModels.Count <= 0)
            {
                LogUtility.LogError($"{SiteId}没有送货建筑");
                return null;
            }
            for(int i =0;i< _idleSiteBuildModels.Count; i++)
            {
                IdleSiteBuildModel model = _idleSiteBuildModels[i];
                if (model.BuildId == buildId)
                    return model;
            }
            LogUtility.LogError($"{SiteId}没有{buildId}对应的送货建筑");
            return null;
        }
        /// <summary>
        /// 建筑收到货物
        /// </summary>
        /// <param name="buildId"></param>
        public void IdleSiteBuildLoadItem(int buildId)
        {
            IdleSiteBuildModel idleSiteBuildModel = GetSiteBuildModel(buildId);
            if (idleSiteBuildModel == null) return;
            idleSiteBuildModel.LoadItem();
        }
        

        private void OnDisable()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_BaseGrade, BaseGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_TimeGrade, TimeGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_VolumeGrade, VolumeGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateLockSprite);
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnPostSiteUnlock);
        } 
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_BaseGrade, BaseGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_TimeGrade, TimeGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_VolumeGrade, VolumeGrade);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateLockSprite);
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnPostSiteUnlock);
        }

    }
}

