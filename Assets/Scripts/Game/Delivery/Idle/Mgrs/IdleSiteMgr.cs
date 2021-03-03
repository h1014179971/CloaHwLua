using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Foundation;
using libx;

namespace Delivery.Idle
{
    public class IdleSiteMgr : Singleton<IdleSiteMgr>
    {
        private Dictionary<int, List<IdleSite>> _idleSiteDic = new Dictionary<int, List<IdleSite>>();
        private Dictionary<int, Dictionary<int, IdleSiteBase>> _idleSiteBaseDic = new Dictionary<int, Dictionary<int, IdleSiteBase>>();
        private Dictionary<int, Dictionary<int, IdleSiteGrade>> _idleSiteGradeDic = new Dictionary<int, Dictionary<int, IdleSiteGrade>>();
        private Dictionary<int, Dictionary<int, IdleSiteTime>> _idleSiteTimeDic = new Dictionary<int, Dictionary<int, IdleSiteTime>>();
        private Dictionary<int, Dictionary<int, IdleSiteVolume>> _idleSiteVolumeDic = new Dictionary<int, Dictionary<int, IdleSiteVolume>>();
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleSite, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    LoadFileCallBack(jsonStr);
                }
                else
                    LogUtility.LogError($"{Files.idleSite}读取失败");
            });
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleSite), this,LoadFileCallBack); 
        }
        private void LoadFileCallBack(string jsonStr)
        {
            if (!string.IsNullOrEmpty(jsonStr))
            {
                List<IdleSite> idleSites = FullSerializerAPI.Deserialize(typeof(List<IdleSite>), jsonStr) as List<IdleSite>;
                for (int i = 0; i < idleSites.Count; i++)
                {
                    IdleSite site = idleSites[i];
                    if (!_idleSiteDic.ContainsKey(site.cityId))
                    {
                        List<IdleSite> lvs = new List<IdleSite>();
                        _idleSiteDic[site.cityId] = lvs;
                    }
                    _idleSiteDic[site.cityId].Add(site);
                }
                LoadAssistFile();
            }
        }
        //转生后需要调用此方法
        public void LoadAssistFile()
        {
            _idleSiteBaseDic.Clear();
            _idleSiteGradeDic.Clear();
            _idleSiteTimeDic.Clear();
            _idleSiteVolumeDic.Clear();
            List<IdleSite> idleSites = GetIdleSites(PlayerMgr.Instance.CityId);
            int index = 0;
            LoadAssistFile(idleSites, index);
        }
        public void LoadAssistFile(List<IdleSite> idleSites,int index)
        {
            if(index >= idleSites.Count)
            {
                //PlayerMgr.Instance.CreatePlayerSites(idleSites);
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                return;
            }
            IdleSite idleSite = idleSites[index];
            int fileLength = 4;
            Assets.LoadAssetAsync(idleSite.siteBase + ".json", typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleSiteBase> siteBases = FullSerializerAPI.Deserialize(typeof(List<IdleSiteBase>), jsonStr) as List<IdleSiteBase>;
                    Dictionary<int, IdleSiteBase> dic = new Dictionary<int, IdleSiteBase>();
                    dic = siteBases.ToDictionary(key => key.Lv, value => value);
                    _idleSiteBaseDic.Add(idleSite.Id, dic);
                    fileLength--;
                    if (fileLength <= 0)
                    {
                        index++;
                        LoadAssistFile(idleSites, index);
                    }
                }
                else
                    LogUtility.LogError($"{idleSite.siteBase + ".json"}读取失败,error={request.error}");
            };
            Assets.LoadAssetAsync(idleSite.siteGrade + ".json", typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleSiteGrade> siteGrades = FullSerializerAPI.Deserialize(typeof(List<IdleSiteGrade>), jsonStr) as List<IdleSiteGrade>;
                    Dictionary<int, IdleSiteGrade> dic = new Dictionary<int, IdleSiteGrade>();
                    dic = siteGrades.ToDictionary(key => key.Id, value => value);
                    _idleSiteGradeDic.Add(idleSite.Id, dic);
                    fileLength--;
                    if (fileLength <= 0)
                    {
                        index++;
                        LoadAssistFile(idleSites, index);
                    }
                }
                else
                    LogUtility.LogError($"{idleSite.siteGrade + ".json"}读取失败,error={request.error}");
            };
            Assets.LoadAssetAsync(idleSite.siteTime + ".json", typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleSiteTime> siteTimes = FullSerializerAPI.Deserialize(typeof(List<IdleSiteTime>), jsonStr) as List<IdleSiteTime>;
                    Dictionary<int, IdleSiteTime> dic = new Dictionary<int, IdleSiteTime>();
                    dic = siteTimes.ToDictionary(key => key.Lv, value => value);
                    _idleSiteTimeDic.Add(idleSite.Id, dic);
                    fileLength--;
                    if (fileLength <= 0)
                    {
                        index++;
                        LoadAssistFile(idleSites, index);
                    }
                }
                else
                    LogUtility.LogError($"{idleSite.siteTime + ".json"}读取失败,error={request.error}");
            };
            Assets.LoadAssetAsync(idleSite.siteVolume + ".json", typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleSiteVolume> siteVolumes = FullSerializerAPI.Deserialize(typeof(List<IdleSiteVolume>), jsonStr) as List<IdleSiteVolume>;
                    Dictionary<int, IdleSiteVolume> dic = new Dictionary<int, IdleSiteVolume>();
                    dic = siteVolumes.ToDictionary(key => key.Lv, value => value);
                    _idleSiteVolumeDic.Add(idleSite.Id, dic);
                    fileLength--;
                    if (fileLength <= 0)
                    {
                        index++;
                        LoadAssistFile(idleSites, index);
                    }
                }
                else
                    LogUtility.LogError($"{idleSite.siteTime + ".json"}读取失败,error={request.error}");
            };
            #region  StreamFile
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, idleSite.siteBase+".json"), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleSiteBase> siteBases = FullSerializerAPI.Deserialize(typeof(List<IdleSiteBase>), jsonStr) as List<IdleSiteBase>;
            //        Dictionary<int, IdleSiteBase> dic = new Dictionary<int, IdleSiteBase>();
            //        dic = siteBases.ToDictionary(key => key.Lv, value => value);
            //        _idleSiteBaseDic.Add(idleSite.Id,dic);
            //        fileLength--;
            //        if (fileLength <= 0)
            //        {
            //            index++;
            //            LoadAssistFile(idleSites, index);
            //        }
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, idleSite.siteGrade + ".json"), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleSiteGrade> siteGrades = FullSerializerAPI.Deserialize(typeof(List<IdleSiteGrade>), jsonStr) as List<IdleSiteGrade>;
            //        Dictionary<int, IdleSiteGrade> dic = new Dictionary<int, IdleSiteGrade>();
            //        dic = siteGrades.ToDictionary(key => key.Id, value => value);
            //        _idleSiteGradeDic.Add(idleSite.Id, dic);
            //        fileLength--;
            //        if (fileLength <= 0)
            //        {
            //            index++;
            //            LoadAssistFile(idleSites, index);
            //        }
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, idleSite.siteTime + ".json"), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleSiteTime> siteTimes = FullSerializerAPI.Deserialize(typeof(List<IdleSiteTime>), jsonStr) as List<IdleSiteTime>;
            //        Dictionary<int, IdleSiteTime> dic = new Dictionary<int, IdleSiteTime>();
            //        dic = siteTimes.ToDictionary(key => key.Lv, value => value);
            //        _idleSiteTimeDic.Add(idleSite.Id,dic);
            //        fileLength--;
            //        if (fileLength <= 0)
            //        {
            //            index++;
            //            LoadAssistFile(idleSites, index);
            //        }
            //    }
            //});
            #endregion
        }

        public List<IdleSite> GetIdleSites(int cityId = -1)
        {
            if (cityId == -1)
                cityId = PlayerMgr.Instance.CityId;
            if (_idleSiteDic.ContainsKey(cityId))
                return _idleSiteDic[cityId];
            else
            {
                LogUtility.LogError($"驿站表未有对应的城市{cityId}");
                return null;
            }
        }
        public IdleSite GetIdleSite(int siteId)
        {
            int cityId = PlayerMgr.Instance.CityId;
            return GetIdleSite(siteId,cityId);
        }
        public IdleSite GetIdleSite(int siteId,int cityId)
        {
            if (!_idleSiteDic.ContainsKey(cityId)) return null;
            List<IdleSite> sites = _idleSiteDic[cityId];
            for(int i = 0; i < sites.Count; i++)
            {
                IdleSite site = sites[i];
                if (site.Id == siteId)
                    return site;
            }
            return null;
        }

        //根据货物id获取驿站基础信息（取驿站id最小的一个）
        public IdleSite GetIdleSiteByItemId(int itemId)
        {
            int cityId= PlayerMgr.Instance.CityId;
            if (!_idleSiteDic.ContainsKey(cityId)) return null;
            List<IdleSite> sites = _idleSiteDic[cityId];
            int minSiteId = -1;
            for (int i = 0; i < sites.Count; i++)
            {
                IdleSite site = sites[i];
                if (site.itemId == itemId)
                {
                    if (minSiteId < 0)
                        minSiteId = site.Id;
                    else if (minSiteId > site.Id)
                        minSiteId = site.Id;
                }
            }
            return GetIdleSite(minSiteId);
        }

        #region siteBase
        public Dictionary<int,IdleSiteBase> GetSiteBaseBySiteId(int siteId)
        {
            if (_idleSiteBaseDic.ContainsKey(siteId))
                return _idleSiteBaseDic[siteId];
            else
            {
                LogUtility.LogError($"没有对应的驿站{siteId}信息");
                return null;
            }
        }
        public IdleSiteBase GetSiteBase(int siteId,int lv)
        {
            Dictionary<int, IdleSiteBase> siteBaseDic = GetSiteBaseBySiteId(siteId);
            if (siteBaseDic.ContainsKey(siteId))
                return siteBaseDic[lv];
            else
            {
                LogUtility.LogError($"没有对应的驿站{siteId}基础等级{lv}信息");
                return null;
            }
        }
        #endregion
        #region siteGrade
        public IdleSiteGrade GetSiteGradeBySiteId(int siteId,int siteBaseLv)
        {
            if (_idleSiteGradeDic.ContainsKey(siteId))
            {
                Dictionary<int, IdleSiteGrade> siteGradeDic = _idleSiteGradeDic[siteId];
                var enumerator = siteGradeDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IdleSiteGrade siteGrade = enumerator.Current.Value;
                    if (siteGrade.lvmin <= siteBaseLv && siteBaseLv <= siteGrade.lvmax)
                        return siteGrade;
                }
                LogUtility.LogError($"驿站{siteId}阶段配置表没有对应的等级{siteBaseLv}");
                return null;
            }
            else
            {
                LogUtility.LogError($"驿站{siteId}没有阶段配置表");
                return null;
            }
        }
        /// <summary>
        /// 获取驿站下一阶段配置
        /// </summary>
        /// <returns></returns>
        public IdleSiteGrade GetNextSiteGrade(int siteId,int siteBaseLv,out bool max)
        {
            max = false;
            if (_idleSiteGradeDic.ContainsKey(siteId))
            {
                Dictionary<int, IdleSiteGrade> siteGradeDic = _idleSiteGradeDic[siteId];
                var enumerator = siteGradeDic.GetEnumerator();
                IdleSiteGrade siteGrade = new IdleSiteGrade();
                while (enumerator.MoveNext())
                {
                    siteGrade = enumerator.Current.Value;
                    if (siteGrade.lvmin <= siteBaseLv && siteBaseLv <= siteGrade.lvmax)
                    {
                        if(enumerator.MoveNext())
                        {
                            IdleSiteGrade nextSiteGrade = enumerator.Current.Value;
                            return nextSiteGrade;
                        }
                    }
                }
                LogUtility.LogError($"驿站{siteId}阶段配置表没有对应的等级{siteBaseLv}");
                return siteGrade;
            }
            else
            {
                LogUtility.LogError($"驿站{siteId}没有阶段配置表");
                max = true;
                return null;
            }
        }
        #endregion
        #region siteTime
        public Dictionary<int, IdleSiteTime> GetSiteTimeBySiteId(int siteId)
        {
            if (_idleSiteTimeDic.ContainsKey(siteId))
                return _idleSiteTimeDic[siteId];
            else
            {
                LogUtility.LogError($"没有对应的驿站配送时间{siteId}信息");
                return null;
            }
        }
        public IdleSiteTime GetSiteTime(int siteId, int timelv)
        {
            Dictionary<int, IdleSiteTime> siteTimeDic = GetSiteTimeBySiteId(siteId);
            if (siteTimeDic.ContainsKey(siteId))
                return siteTimeDic[timelv];
            else
            {
                LogUtility.LogError($"没有对应的驿站{siteId}配送时间等级{timelv}信息");
                return null;
            }
        }
        //获取配送时间最大等级
        public int GetSiteTimeMaxLv(int siteId)
        {
            Dictionary<int, IdleSiteTime> siteTimeDic = GetSiteTimeBySiteId(siteId);
            int maxLv = 0;
            if(siteTimeDic!=null)
            {
                var enumerator = siteTimeDic.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    int currentLv = enumerator.Current.Value.Lv;
                    if (currentLv > maxLv)
                        maxLv = currentLv;
                }
            }
            return maxLv;
        }
        #endregion
        #region siteVolume
        public Dictionary<int, IdleSiteVolume> GetSiteVolumeBySiteId(int siteId)
        {
            if (_idleSiteVolumeDic.ContainsKey(siteId))
                return _idleSiteVolumeDic[siteId];
            else
            {
                LogUtility.LogError($"没有对应的驿站容量{siteId}信息");
                return null;
            }
        }
        public IdleSiteVolume GetSiteVolume(int siteId, int volumelv)
        {
            Dictionary<int, IdleSiteVolume> siteVolumeDic = GetSiteVolumeBySiteId(siteId);
            if (siteVolumeDic.ContainsKey(siteId))
                return siteVolumeDic[volumelv];
            else
            {
                LogUtility.LogError($"没有对应的驿站{siteId}容量等级{volumelv}信息");
                return null;
            }
        }
        //获取驿站容量最大等级
        public int GetSiteVolumeMaxLv(int siteId)
        {
            Dictionary<int, IdleSiteVolume> siteVolumeDic = GetSiteVolumeBySiteId(siteId);
            int maxLv = 0;
            if(siteVolumeDic!=null)
            {
                var enumerator = siteVolumeDic.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    int currentLv = enumerator.Current.Value.Lv;
                    if(currentLv>maxLv)
                    {
                        maxLv = currentLv;
                    }
                }
            }

            return maxLv;
        }
        #endregion
    }
}

