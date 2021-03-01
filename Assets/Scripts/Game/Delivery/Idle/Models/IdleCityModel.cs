using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System;
using libx;

namespace Delivery.Idle
{
    public class NeedItem
    {
        public int itemId;                 
        public Long2 num; //当前数量
    }
    public class IdleCityModel : MonoBehaviour
    {
       
        private List<NeedItem> _needItems = new List<NeedItem>();
        private PlayerCity _playerCity;
        private int _volume;//当前送货容量
        private float _cycle;//送货周期
        private float _timeCycle;//周期计时
        private TimerEvent _timer;

        private float itemCountTimes=1;//特殊事件货物数量分配倍数
        private int specialItemId=-1;//特殊事件货物Id(无特殊货物id小于0)
        
        private Dictionary<int, List<IdleShelfModel>> _allIdleShelfModel = new Dictionary<int, List<IdleShelfModel>>();//所有货架
        private Dictionary<int, Transform> _allShelfParentDic = new Dictionary<int, Transform>();//货架父节点

        private Transform shelfNode;//货架总节点
        private Transform _shelfLevelUpFxNode;//货架升级特效节点
        private float _lastPlayShelfFxTime;//上一次播放货架升级特效的时间
        private float _dPlayShelfFxTime = 0.2f;//播放货架升级特效的时间间隔

        private TimerEvent _siteTipTimer;//升级快递中心的操作计时器
        private float _tipTime=30f;//操作计时器时间

        private MyText _enableCountText;//剩余可使用空位
        private GameObject _redVolumeScreen;
        private GameObject _greenVolumeScreen;

        
        public void Init(PlayerCity city)
        {
            _playerCity = city;
            _volume = IdleCityMgr.Instance.GetVolume(_playerCity.cityTruckVolumeLv);
            _cycle = IdleCityMgr.Instance.GetCycle(_playerCity.cycleLv);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeVolume, ChangeVolume);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeCycle, ChangeCycle);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeStoreVolume, OnStoreVolumeChange);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, UnLockSite);
            //添加特殊事件相关监听
            //EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangeItemCount, OnItemCountTimesChange);
            //EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
            CreatNeedItem();
            //AllotItem(_playerCity.loadItemNum);//现在不需要分配货物
            //UpdateItem();

            _enableCountText = this.GetComponentByPath<MyText>("jijiaqi/enableCountCanvas/enableCount");
            _redVolumeScreen = this.GetComponentByPath<Transform>("jijiaqi/red").gameObject;
            _greenVolumeScreen = this.GetComponentByPath<Transform>("jijiaqi/green").gameObject;


            shelfNode = GameObject.Find("map/shelfPoints").transform;
            _shelfLevelUpFxNode = GameObject.Find("map/storeVolumeFxNode").transform;
            _lastPlayShelfFxTime = Time.time;
            InitShelf();//初始化货架

        }
        
        private void InitShelf()
        {
            _allIdleShelfModel.Clear();
            _allShelfParentDic.Clear();
            List<PlayerSite> sites = PlayerMgr.Instance.PlayerCity.playerSites;

            int storeLv = _playerCity.storeLv;
            IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
            string shelfIdStr = storeVolume.shelfRes;
            string[] shelfIdArr = shelfIdStr.Split(',');
            for (int i = 0; i < sites.Count; i++)
            {
                IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(sites[i].Id);
                int itemId = idleSite.itemId;
                if (_allShelfParentDic.ContainsKey(itemId))
                    continue;

                Transform shelfRoot = shelfNode.Find("shelfRoot" + itemId.ToString());
                if(!_allShelfParentDic.ContainsKey(itemId))
                    _allShelfParentDic.Add(itemId, shelfRoot);
                List<IdleShelfModel> shelfModelList = new List<IdleShelfModel>();
                if (!_allIdleShelfModel.ContainsKey(itemId))
                    _allIdleShelfModel.Add(itemId, shelfModelList);

                if(!sites[i].isLock)
                {
                    for (int j = 0; j < shelfIdArr.Length; j++)
                    {
                        int shelfId = int.Parse(shelfIdArr[j]);
                        Transform shelfParent = _allShelfParentDic[itemId].Find("shelf" + j.ToString());
                        IdleShelfModel shelfModel = new IdleShelfModel(shelfId, itemId, shelfParent);
                        _allIdleShelfModel[itemId].Add(shelfModel);
                    }
                    UpdateShelfStorey(itemId,false);
                }
            }
        }




        //更新显示屏
        private void UpdateScreen()
        {
            int maxVolume = 0;
            int currentVolume = PlayerMgr.Instance.GetCurrentTotalVolume(out maxVolume);
            _enableCountText.text = (maxVolume - currentVolume).ToString();
            if (currentVolume >= maxVolume)
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


        //特殊货物分配倍数变化
        private void OnItemCountTimesChange(BaseEventArgs baseEventArgs)
        {
            EventArgsTwo<int, float> argsTwo = (EventArgsTwo<int, float>)baseEventArgs;
            specialItemId = argsTwo.param1;
            itemCountTimes = argsTwo.param2;
        }
        //特殊事件结束
        private void OnSpecialEventEnd(BaseEventArgs baseEventArgs)
        {
            specialItemId = -1;
            itemCountTimes = 1;
        }

        private void ChangeVolume(BaseEventArgs args)
        {
            _volume = IdleCityMgr.Instance.GetVolume(_playerCity.cityTruckVolumeLv);
        }
        private void ChangeCycle(BaseEventArgs args)
        {
            _cycle = IdleCityMgr.Instance.GetCycle(_playerCity.cycleLv);
        }
        private void UnLockSite(BaseEventArgs args)
        {
            EventArgsOne<IdleSiteModel> arg = args as EventArgsOne<IdleSiteModel>;
            UpdateScreen();
            bool isAdd = true;
            for(int i = 0;i< _needItems.Count; i++)
            {
                 if(_needItems[i].itemId == arg.param1.IdleSite.itemId)
                {
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
            {
                NeedItem needItem = new NeedItem();
                needItem.itemId = arg.param1.IdleSite.itemId;
                needItem.num = Long2.zero;
                _needItems.Add(needItem);

                PlayerMgr.Instance.UnlockItemStore(needItem.itemId);
               

                //创建对应货物的所有货架
                int itemId = needItem.itemId;
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_City_UnlockShelf, itemId));
                int storeLv = _playerCity.storeLv;
                IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
                string shelfIdStr = storeVolume.shelfRes;
                string[] shelfIdArr = shelfIdStr.Split(',');
                if(!_allIdleShelfModel.ContainsKey(itemId))
                {
                    List<IdleShelfModel> shelfModelList = new List<IdleShelfModel>();
                    _allIdleShelfModel.Add(itemId, shelfModelList);
                   
                }
                for (int i = 0; i < shelfIdArr.Length; i++)
                {
                    int shelfId = int.Parse(shelfIdArr[i]);
                    Transform shelfParent = _allShelfParentDic[itemId].Find("shelf"+i.ToString()) ;
                    IdleShelfModel shelfModel = new IdleShelfModel(shelfId, itemId, shelfParent);
                    _allIdleShelfModel[itemId].Add(shelfModel);
                }
                UpdateShelfStorey(itemId, false);
               
            }
        }
        private void CreatNeedItem()
        {
            for(int i = 0; i < PlayerMgr.Instance.PlayerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = PlayerMgr.Instance.PlayerCity.playerSites[i];
                if (playerSite.isLock) continue;
                IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id);
                bool needAdd = true;
                for(int j=0;j<_needItems.Count;j++)
                {
                    if(_needItems[j].itemId==idleSite.itemId)
                    {
                        needAdd = false;
                    }
                }
                if(needAdd)
                {
                    NeedItem need = new NeedItem();
                    need.itemId = idleSite.itemId;
                    need.num = Long2.zero;
                    _needItems.Add(need);
                }
                
            }
        }


        //货车到达站点开始装货
        public void LoadItem(int itemId)
        {
            LoadItem(itemId, _volume);
        }
        public void LoadItem(int itemId, int loadItemCount)
        {
            int storeMaxVolume = PlayerMgr.Instance.GetStoreMaxVolume();
            if (specialItemId == itemId)
            {
                loadItemCount = Mathf.RoundToInt(loadItemCount * itemCountTimes);
            }

            if (!_playerCity.storeItems.ContainsKey(itemId))
            {
                _playerCity.storeItems.Add(itemId,0);
            }
            int currentStoreNum = _playerCity.storeItems[itemId];
            if (currentStoreNum + loadItemCount > storeMaxVolume)
                loadItemCount = storeMaxVolume - currentStoreNum;
            _playerCity.storeItems[itemId] += loadItemCount;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_City_ChangeStoreRest));

            SetSiteTipTimer(_playerCity.storeItems[itemId], storeMaxVolume);

            //AddShelfStorey(itemId);
            UpdateShelfStorey(itemId);
        }
        //设置升级快递中心操作的提示计时器
        private void SetSiteTipTimer(int currentVolume,int maxVolume)
        {
            float percent = (float)currentVolume / maxVolume;
            if (percent <= 0.1f && _siteTipTimer == null)
            {
                _siteTipTimer = Timer.Instance.Register(_tipTime, OnLevelupSiteStart);
            }
            else if (percent > 0.1f && _siteTipTimer != null)
            {
                Timer.Instance.DestroyTimer(_siteTipTimer);
                _siteTipTimer = null;
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_ActionTip_CancelToLevelUpSite));
            }
        }

        private void OnLevelupSiteStart(params object[]objs)
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_ActionTip_NeedToLevelUpSite));
            Timer.Instance.DestroyTimer(_siteTipTimer);
            _siteTipTimer = null;
        }


        //仓库装货量升级时调用，更新对应资源
        private void OnStoreVolumeChange(BaseEventArgs baseEventArgs)
        {
            //更新货架
            for(int i=0;i<NeedItems.Count;i++)
            {
                int itemId = NeedItems[i].itemId;
                UpdateShelfRes(itemId);
                UpdateShelfStorey(itemId);
                //PlayerShelfAni(itemId);
            }

            IdleCityCtrl.Instance.SetFloor();//更新地板

            //if (Time.time-_lastPlayShelfFxTime>_dPlayShelfFxTime)
            //{
            //    FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, _shelfLevelUpFxNode.position, 0.5f);
            //    model.transform.localScale = Vector3.one * 9f;
            //}
            
        }

        private void UpdateShelfStorey(int itemId,bool showFx=true)
        {
            if (_allIdleShelfModel.ContainsKey(itemId))
            {
                int currentVolume = PlayerMgr.Instance.GetCurrentStoreVolume(itemId);
                List<IdleShelfModel> shelfModelList = _allIdleShelfModel[itemId];
                for(int i=0;i<shelfModelList.Count;i++)
                {
                    int shelfMaxItemCount = shelfModelList[i].MaxItemCount;
                    int itemCount = shelfMaxItemCount < currentVolume ? shelfMaxItemCount : currentVolume;
                    shelfModelList[i].SetItemCount(itemCount, showFx);
                    currentVolume -= shelfMaxItemCount;
                    if (currentVolume < 0) currentVolume = 0;
                }
                UpdateScreen();//更新显示屏
            }
            
        }

        private void UpdateShelfRes(int itemId)
        {
            if(_allIdleShelfModel.ContainsKey(itemId))
            {
                int storeLv = _playerCity.storeLv;
                IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
                string shelfIdStr = storeVolume.shelfRes;
                string[] shelfIdArr = shelfIdStr.Split(',');

                List<IdleShelfModel> shelfModelList = _allIdleShelfModel[itemId];
                int currentShelfCount = shelfModelList.Count;
               for(int i=0;i<shelfIdArr.Length;i++)
                {
                    int shelfId = int.Parse(shelfIdArr[i]);
                    if(i< currentShelfCount)
                    {
                        shelfModelList[i].UpdateShelfRes(shelfId, 3.5f);
                    }
                    else
                    {
                        Transform shelfParent = _allShelfParentDic[itemId].Find("shelf" + i.ToString());
                        IdleShelfModel shelfModel = new IdleShelfModel(shelfId, itemId, shelfParent);
                        shelfModelList.Add(shelfModel);
                        shelfModel.PlayShelfAni(true, 3.5f);
                    }
                }
            }
        }
        //private void PlayerShelfAni(int itemId)
        //{
        //    if(_allIdleShelfModel.ContainsKey(itemId))
        //    {
        //        List<IdleShelfModel> shelfModelList = _allIdleShelfModel[itemId];
        //        for (int i=0;i< shelfModelList.Count;i++)
        //        {
        //            shelfModelList[i].PlayShelfAni(false);
        //        }
        //    }

        //}
        
        public List<NeedItem> NeedItems { get { return _needItems; } }
        ////获取当前仓库此货物的数量
        //public Long2 GetItemNum(int itemId)
        //{
        //    for(int i = 0; i < _needItems.Count; i++)
        //    {
        //        NeedItem needItem = _needItems[i];
        //        if (needItem.itemId == itemId)
        //            return needItem.num;
        //    }    
        //    return Long2.zero;
        //}
        //public Long2 GetItemNum(string itemId)
        //{
        //    return GetItemNum(int.Parse(itemId));
        //}
        //出货
        //public void UnLoadItem(int itemId,Long2 num)
        //{
        //    for(int i = 0; i < _needItems.Count; i++)
        //    {
        //        NeedItem needItem = _needItems[i];
        //        if(needItem.itemId == itemId)
        //        {
        //            needItem.num -= num;
        //            needItem.num = needItem.num < Long2.zero ? Long2.zero : needItem.num;
        //            _playerCity.loadItemNum -= num;
        //            break;
        //        }
        //    }
                                          
        //}

        //出货
        public void UnLoadItem(int itemId,int num)
        {
            if (!_playerCity.storeItems.ContainsKey(itemId))
                return;
            _playerCity.storeItems[itemId] -= num;
            if (_playerCity.storeItems[itemId] < 0)
                _playerCity.storeItems[itemId] = 0;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_City_ChangeStoreRest));
            //ReduceShelfStorey(itemId);
            UpdateShelfStorey(itemId);

            int storeMaxVolume = PlayerMgr.Instance.GetStoreMaxVolume();
            SetSiteTipTimer(_playerCity.storeItems[itemId], storeMaxVolume);

        }
        
        

        private void OnDestroy()
        {
            //if (_timer != null)
            //    Timer.Instance.DestroyTimer(_timer);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeVolume, ChangeVolume);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeCycle, ChangeCycle);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeStoreVolume, OnStoreVolumeChange);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, UnLockSite);
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangeItemCount, OnItemCountTimesChange);
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
        }

    }
}

