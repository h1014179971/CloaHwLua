using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;
using libx;

namespace Delivery.Idle
{
    public class IdleTruckMgr : Singleton<IdleTruckMgr>
    {
        private Dictionary<int, IdleTruckType> _idleTruckTypeDic = new Dictionary<int, IdleTruckType>();
        private Dictionary<int, IdleTruckLv> _idleTruckLvDic = new Dictionary<int, IdleTruckLv>();
        private List<IdleTruckRes> _idleTruckRess = new List<IdleTruckRes>();
        private Dictionary<int, IdleTruckNum> _idleTruckNumDic = new Dictionary<int, IdleTruckNum>();
        private int _loadTruckData=3;
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            //Assets.LoadAssetAsync(Files.idleTruckType, typeof(TextAsset)).completed += delegate (AssetRequest request)
            //{
            //    if (string.IsNullOrEmpty(request.error))
            //    {
            //        string jsonStr = (request.asset as TextAsset).text;
            //        List<IdleTruckType> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckType>), jsonStr) as List<IdleTruckType>;
            //        _idleTruckTypeDic = idleTrucks.ToDictionary(key => key.Id, value => value);
            //        _loadTruckData--;
            //        if (_loadTruckData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //    else
            //        LogUtility.LogError($"{Files.idleTruckType}读取失败,error={request.error}");
            //};
            AssetLoader.LoadAsync(Files.idleTruckLv, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleTruckLv> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckLv>), jsonStr) as List<IdleTruckLv>;
                    _idleTruckLvDic = idleTrucks.ToDictionary(key => key.Lv, value => value);
                    _loadTruckData--;
                    if (_loadTruckData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleTruckLv}读取失败");
            });
            AssetLoader.LoadAsync(Files.idleTruckRes, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    _idleTruckRess = FullSerializerAPI.Deserialize(typeof(List<IdleTruckRes>), jsonStr) as List<IdleTruckRes>;
                    _loadTruckData--;
                    if (_loadTruckData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleTruckRes}读取失败");
            });
            AssetLoader.LoadAsync(Files.idleTruckNum, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleTruckNum> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckNum>), jsonStr) as List<IdleTruckNum>;
                    _idleTruckNumDic = idleTrucks.ToDictionary(key => key.num, value => value);
                    _loadTruckData--;
                    if (_loadTruckData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleTruckNum}读取失败");
            });
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleTruckType), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleTruckType> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckType>), jsonStr) as List<IdleTruckType>;
            //        _idleTruckTypeDic = idleTrucks.ToDictionary(key => key.Id, value => value);
            //        _loadTruckData--;
            //        if (_loadTruckData <=0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleTruckLv), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleTruckLv> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckLv>), jsonStr) as List<IdleTruckLv>;
            //        _idleTruckLvDic = idleTrucks.ToDictionary(key => key.Lv, value => value);
            //        _loadTruckData--;
            //        if (_loadTruckData<=0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleTruckRes), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleTruckRes> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckRes>), jsonStr) as List<IdleTruckRes>;
            //        for (int i = 0; i < idleTrucks.Count; i++)
            //        {
            //            IdleTruckRes truckRes = idleTrucks[i];
            //            if (!_idleTruckResDic.ContainsKey(truckRes.truckId))
            //            {
            //                List<IdleTruckRes> res = new List<IdleTruckRes>();
            //                _idleTruckResDic[truckRes.truckId] = res;
            //            }
            //            _idleTruckResDic[truckRes.truckId].Add(truckRes);
            //        }                                                                          
            //        _loadTruckData--;
            //        if (_loadTruckData<=0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleTruckNum), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleTruckNum> idleTrucks = FullSerializerAPI.Deserialize(typeof(List<IdleTruckNum>), jsonStr) as List<IdleTruckNum>;
            //        _idleTruckNumDic = idleTrucks.ToDictionary(key => key.num, value => value);
            //        _loadTruckData--;
            //        if (_loadTruckData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
        }
        //public IdleTruckType GetIdleTruckTypeById(int id)
        //{
        //    IdleTruckType truck = null;
        //    _idleTruckTypeDic.TryGetValue(id, out truck);
        //    return truck;
        //}
        //public IdleTruckType GetIdleTruckTypeById(string id)
        //{
        //    return GetIdleTruckTypeById(int.Parse(id));
        //}
        ///// <summary>
        ///// 根据获取id获取卡车类型对象
        ///// </summary>
        //public List<IdleTruckType> GetIdleTruckTypeByItemId(string itemId)
        //{
        //    List<IdleTruckType> idleTruckTypes = new List<IdleTruckType>();
        //    var enumerator = _idleTruckTypeDic.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        IdleTruckType idleTruckType = enumerator.Current.Value;
        //        string[] itemIds = idleTruckType.itemId.Split(',');
        //        for (int i = 0; i < itemIds.Length; i++)
        //        {
        //            if (itemIds[i] == itemId)
        //            {
        //                idleTruckTypes.Add(idleTruckType);
        //                break;
        //            }
                        
        //        }
        //    }
        //    return idleTruckTypes;
        //}

        public IdleTruckLv GetIdleTruckLv(int level)
        {
            if (!_idleTruckLvDic.ContainsKey(level))
                return null;
            IdleTruckLv truckLv = _idleTruckLvDic[level];
            return truckLv;
        }
        public IdleTruckRes GetIdleTruckRes( int level)
        {
            for (int i = 0; i < _idleTruckRess.Count; i++)
            {
                IdleTruckRes lv = _idleTruckRess[i];
                if (lv.lvmin <= level && level <= lv.lvmax)
                    return lv;
            }
            return null;
        }
        /// <summary>
        /// 获取货车数量升级花费
        /// </summary>
        public string GetTruckLevelUpCost(int num)
        {
            if (!_idleTruckNumDic.ContainsKey(num))
                return "null";
            return _idleTruckNumDic[num].price;
        }

        public bool IsMaxTruckCount(int count)
        {
            if (!_idleTruckNumDic.ContainsKey(count))
                return true;
            return false;
        }

        public bool IsMaxTruckLv(int Lv)
        {
            if (!_idleTruckLvDic.ContainsKey(Lv))
                return true;
            return false;
        }

    }
}


