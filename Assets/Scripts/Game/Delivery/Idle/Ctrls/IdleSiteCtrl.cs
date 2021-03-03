using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Idle
{
    public class IdleSiteCtrl : MonoSingleton<IdleSiteCtrl>
    {
        private List<IdleSiteModel> _idleSiteModels = new List<IdleSiteModel>();
        public bool AnySiteCanLock
        {
            get;
            private set;
        }
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, UnLockSite);
        }
        public void Init()
        {     
            CitySite();
        }
        private void CitySite()
        {
            AnySiteCanLock = false;
            GameObject[] siteObjs = GameObject.FindGameObjectsWithTag(Tags.CitySite);
            int nextLockSiteId = PlayerMgr.Instance.GetNextLockSiteId();
            for(int i = 0; i < siteObjs.Length; i++)
            {
                GameObject obj = siteObjs[i];
                IdleSiteModel model = obj.GetComponent<IdleSiteModel>();
                if (model == null) continue;     
                PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(model.SiteId);
                if (playerSite != null)
                {
                    model.Init(playerSite);
                    _idleSiteModels.Add(model);
                    if (model.SiteId == nextLockSiteId)
                    {
                        model.ShowLock();
                        AnySiteCanLock = model.CanUnlock;
                    }
                    else
                        model.HideLock();
                }
                    
            }
        }

        public List<IdleSiteModel> GetIdleSiteModels()
        {
            return _idleSiteModels;
        } 
        //获取需要分配货物的收货点
        public List<IdleSiteModel> GetIdleSiteModelsByItemId(string itemId)
        {
            return GetIdleSiteModelsByItemId(int.Parse(itemId));
        }
        public List<IdleSiteModel> GetIdleSiteModelsByItemId(int itemId)
        {      
            List<IdleSiteModel> models = new List<IdleSiteModel>();
            for (int i = 0; i < _idleSiteModels.Count; i++)
            {
                IdleSiteModel model = _idleSiteModels[i];
                if (model.IdleSite.itemId.Equals(itemId) && !model.PlayerSite.isLock) 
                    models.Add(model);
            }
            return models;
        } 
        public List<IdleSiteModel> GetUnLockIdleSite()
        {
            List<IdleSiteModel> models = new List<IdleSiteModel>();
            for (int i = 0; i < _idleSiteModels.Count; i++)
            {
                IdleSiteModel model = _idleSiteModels[i];
                if (!model.PlayerSite.isLock)
                    models.Add(model);
            }
            return models;
        }
         //解锁驿站
        private void UnLockSite(BaseEventArgs args)
        {
            EventArgsOne<IdleSiteModel> arg = args as EventArgsOne<IdleSiteModel>;
            IdleSiteModel model = arg.param1;
            model.HideLock();
            
            Timer.Instance.Register(0.5f, ShowBuilding,model);
            AudioCtrl.Instance.PlaySingleSound(GameAudio.BuildSmoke);
            FxCtrl.Instance.PlayFx(FxPrefabPath.idleCreateSite, model.transform.position - Vector3.up, () => {
                //model.SpriteShow();
                model.CreateStaff();
                FxCtrl.Instance.PlayFx(FxPrefabPath.idleFinishCreateSite, model.transform.position, -1);
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.firework);
            }, false, 1.5f);


            int nextLockSiteId = PlayerMgr.Instance.GetNextLockSiteId();
            IdleSiteModel nextModel = GetIdleSiteModelById(nextLockSiteId);
            if (nextModel != null)
            {
                nextModel.ShowLock();
                AnySiteCanLock = nextModel.CanUnlock;
            }
            else
            {
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_Site_CanUnlock, false));//通知界面没有可解锁驿站
            }

        }

        private void ShowBuilding(params object[] objs)
        {
            IdleSiteModel model = objs[0] as IdleSiteModel;
            model.SpriteShow();
        }
        

        public IdleSiteModel GetIdleSiteModelById(int siteId)
        {
            for (int i = 0; i < _idleSiteModels.Count; i++)
            {
                IdleSiteModel model = _idleSiteModels[i];
                if (model.SiteId == siteId)
                    return model;
            }
            return null;
        }
        public override void Dispose()
        {
            base.Dispose();
            _idleSiteModels.Clear();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, UnLockSite);

        }

    }
}

