using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Foundation;
using libx;

namespace Delivery.Idle
{
    public class IdleCityMgr : Singleton<IdleCityMgr>
    {
        private Dictionary<int, IdleCity> _idleCityDic = new Dictionary<int, IdleCity>();
        private Dictionary<int, IdleCityVolume> _idleCityVolume = new Dictionary<int, IdleCityVolume>();
        private Dictionary<int, IdleCityCycle> _idleCityCycle = new Dictionary<int, IdleCityCycle>();
        private Dictionary<int, IdleCityStoreVolume> _idleCityStoreVolume = new Dictionary<int, IdleCityStoreVolume>();
        private int _loadCityData = 4;
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleCity, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleCity> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCity>), jsonStr) as List<IdleCity>;
                    _idleCityDic = idleCitys.ToDictionary(key => key.Id, value => value);
                    _loadCityData--;
                    if (_loadCityData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCity}读取失败");
            });
            AssetLoader.LoadAsync(Files.idleCityVolume, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleCityVolume> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityVolume>), jsonStr) as List<IdleCityVolume>;
                    _idleCityVolume = idleCitys.ToDictionary(key => key.Lv, value => value);
                    _loadCityData--;
                    if (_loadCityData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCityVolume}读取失败");
            });
            AssetLoader.LoadAsync(Files.idleCityCycle, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleCityCycle> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityCycle>), jsonStr) as List<IdleCityCycle>;
                    _idleCityCycle = idleCitys.ToDictionary(key => key.Lv, value => value);
                    _loadCityData--;
                    if (_loadCityData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCityCycle}读取失败");
            });
            AssetLoader.LoadAsync(Files.idleCityStoreVolume, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleCityStoreVolume> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityStoreVolume>), jsonStr) as List<IdleCityStoreVolume>;
                    _idleCityStoreVolume = idleCitys.ToDictionary(key => key.Lv, value => value);
                    _loadCityData--;
                    if (_loadCityData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCityStoreVolume}读取失败");
            });
            #region Assets
            //Assets.LoadAssetAsync(Files.idleCity, typeof(TextAsset)).completed += delegate (AssetRequest request)
            // {
            //     if (string.IsNullOrEmpty(request.error))
            //     {
            //         string jsonStr = (request.asset as TextAsset).text;
            //         List<IdleCity> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCity>), jsonStr) as List<IdleCity>;
            //         _idleCityDic = idleCitys.ToDictionary(key => key.Id, value => value);
            //         _loadCityData--;
            //         if (_loadCityData <= 0)
            //             EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //     }
            //     else
            //         LogUtility.LogError($"{Files.idleCity}读取失败,error={request.error}");
            // };
            //Assets.LoadAssetAsync(Files.idleCityVolume, typeof(TextAsset)).completed += delegate (AssetRequest request)
            //{
            //    if (string.IsNullOrEmpty(request.error))
            //    {
            //        string jsonStr = (request.asset as TextAsset).text;
            //        List<IdleCityVolume> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityVolume>), jsonStr) as List<IdleCityVolume>;
            //        _idleCityVolume = idleCitys.ToDictionary(key => key.Lv, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //    else
            //        LogUtility.LogError($"{Files.idleCityVolume}读取失败,error={request.error}");
            //};
            //Assets.LoadAssetAsync(Files.idleCityCycle, typeof(TextAsset)).completed += delegate (AssetRequest request)
            //{
            //    if (string.IsNullOrEmpty(request.error))
            //    {
            //        string jsonStr = (request.asset as TextAsset).text;
            //        List<IdleCityCycle> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityCycle>), jsonStr) as List<IdleCityCycle>;
            //        _idleCityCycle = idleCitys.ToDictionary(key => key.Lv, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //    else
            //        LogUtility.LogError($"{Files.idleCityCycle}读取失败,error={request.error}");
            //};
            //Assets.LoadAssetAsync(Files.idleCityStoreVolume, typeof(TextAsset)).completed += delegate (AssetRequest request)
            //{
            //    if (string.IsNullOrEmpty(request.error))
            //    {
            //        string jsonStr = (request.asset as TextAsset).text;
            //        List<IdleCityStoreVolume> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityStoreVolume>), jsonStr) as List<IdleCityStoreVolume>;
            //        _idleCityStoreVolume = idleCitys.ToDictionary(key => key.Lv, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //    else
            //        LogUtility.LogError($"{Files.idleCityStoreVolume}读取失败,error={request.error}");
            //};
            #endregion
            #region StreamFile
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleCity), this, (jsonStr) =>
            //{
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleCity> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCity>), jsonStr) as List<IdleCity>;
            //        _idleCityDic = idleCitys.ToDictionary(key => key.Id, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleCityVolume), this, (jsonStr) =>
            //{
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleCityVolume> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityVolume>), jsonStr) as List<IdleCityVolume>;
            //        _idleCityVolume = idleCitys.ToDictionary(key => key.Lv, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleCityCycle), this, (jsonStr) =>
            //{
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleCityCycle> idleCitys = FullSerializerAPI.Deserialize(typeof(List<IdleCityCycle>), jsonStr) as List<IdleCityCycle>;
            //        _idleCityCycle = idleCitys.ToDictionary(key => key.Lv, value => value);
            //        _loadCityData--;
            //        if (_loadCityData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            #endregion
        }
        public IdleCity GetIdleCityById(int cityId = -1)
        {
            if (cityId == -1)
                cityId = PlayerMgr.Instance.CityId;
            if (!_idleCityDic.ContainsKey(cityId))
                return null;
            return _idleCityDic[cityId];
        }
        public IdleCity GetIdleCityById(string cityId)
        {
            return GetIdleCityById(int.Parse(cityId));
        }

        public Dictionary<int, IdleCity> GetAllIdleCity()
        {
            return _idleCityDic;
        }

        public IdleCityVolume GetIdleCityVolume(int lv)
        {
            if (!_idleCityVolume.ContainsKey(lv))
                return null;
            return _idleCityVolume[lv];
        }
        public int GetVolume(int lv)
        {
            if (!_idleCityVolume.ContainsKey(lv))
                return 0;
            return _idleCityVolume[lv].volume;
        }
        public IdleCityCycle GetIdleCityCycle(int lv)
        {
            if (!_idleCityCycle.ContainsKey(lv))
                return null;
            return _idleCityCycle[lv];
        }
        public float GetCycle(int lv)
        {
            if (!_idleCityCycle.ContainsKey(lv))
            {
                LogUtility.LogError($"站点装货周期升级表没有{lv}等级");
                return 0;
            }
                
            return _idleCityCycle[lv].cycle;
        }

        //获取仓库容量
        public int GetStoreVolume(int lv)
        {
            if(!_idleCityStoreVolume.ContainsKey(lv))
            {
                return 0;
            }
            return _idleCityStoreVolume[lv].volume;
        }

        public IdleCityStoreVolume GetIdleStoreVolume(int lv)
        {
            if(!_idleCityStoreVolume.ContainsKey(lv))
            {
                return null;
            }
            return _idleCityStoreVolume[lv];
        }

        /// <summary>
        /// 判断是否到达站点容量等级上限
        /// </summary>
        public bool IsMaxVolumeLv(int lv)
        {
            if (!_idleCityVolume.ContainsKey(lv))
                return true;
            return false;
        }

     
        public bool IsMaxCycleLv(int lv)
        {
            if (!_idleCityCycle.ContainsKey(lv))
                return true;
            return false;
        }

        public bool IsMaxStoreLv(int lv)
        {
            if (!_idleCityStoreVolume.ContainsKey(lv))
                return true;
            return false;
        }

    }
}

