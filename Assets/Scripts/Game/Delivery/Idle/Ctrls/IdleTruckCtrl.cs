using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;

namespace Delivery.Idle
{
    public class IdleTruckCtrl : MonoSingleton<IdleTruckCtrl>
    {
        private Transform _mid;
        private List<IdleTruckModel> _idleTruckModels = new List<IdleTruckModel>();
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_TruckBack, TruckBack);
        }
        public void Init()
        {
            if (_mid == null)
                _mid = GameObject.Find("map/mid").transform;
            CreateTruck();                                                             
        }
        private void CreateTruck()
        {
            CreateTruck(PlayerMgr.Instance.GetTruckNum());
        }
        private void CreateTruck(int createNum)
        {
            if (createNum <= 0) return;
            PlayerTruck playerTruck = PlayerMgr.Instance.GetPlayerTruck();
            for (int i = 0; i < createNum; i++)
            {
                IdleTruck idleTruck = PlayerMgr.Instance.GetIdleTruck(playerTruck.truckLv);
                string prefabPath = PrefabPath.idlePath + PrefabName.truck;
                GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.truck, true, 1);
                obj.transform.SetParent(_mid);
                IdleTruckModel model = obj.GetOrAddComponent<IdleTruckModel>();
                IdleTruckStopModel stopModel = IdleCityCtrl.Instance.UseParkPoint(model);
                obj.transform.position = stopModel.GetPoint();
                obj.name = prefabPath.Replace('/', '_');
                model.SetStopModel(stopModel);
                model.Init(idleTruck);
                
                _idleTruckModels.Add(model);
            }
        }
        public IdleTruckModel CreateTruck(ParkPoint parkPoint)
        {
            PlayerTruck playerTruck = PlayerMgr.Instance.GetPlayerTruck();
            IdleTruck idleTruck = PlayerMgr.Instance.GetIdleTruck(playerTruck.truckLv);
            string prefabPath = PrefabPath.idlePath + PrefabName.truck;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.truck, true, 1);
            obj.transform.SetParent(_mid);
            IdleTruckModel model = obj.GetOrAddComponent<IdleTruckModel>();
            parkPoint.truckModel = model;
            obj.transform.position = parkPoint.stopModel.GetPoint();
            obj.name = prefabPath.Replace('/', '_');
            model.Init(idleTruck);
            model.SetStopModel(parkPoint.stopModel);
            _idleTruckModels.Add(model);
            return model;
        }

        private void ChangeTruck(BaseEventArgs args)
        {
            PlayerTruck playerTruck = PlayerMgr.Instance.GetPlayerTruck();
            int offsetTruckNum = playerTruck.truckNum - _idleTruckModels.Count;
            CreateTruck(offsetTruckNum);
        }
        private void TruckBack(BaseEventArgs args)
        {
            EventArgsOne<IdleTruckModel> arg = args as EventArgsOne<IdleTruckModel>;
            IdleTruckModel truckModel = arg.param1;
            IdleCityCtrl.Instance.TruckBack(truckModel);
        }

        public override void Dispose()
        {
            base.Dispose();
            _mid = null;
            _idleTruckModels.Clear();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Truck_ChangeMaxCount, ChangeTruck);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_TruckBack, TruckBack);
        }

    }
}

