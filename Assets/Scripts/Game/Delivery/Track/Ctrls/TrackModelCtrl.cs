using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Track
{
    public class TrackModelCtrl : MonoSingleton<TrackModelCtrl>
    {
        [SerializeField]RectTransform _siteParent;
        [SerializeField]RectTransform _houseParent;             
        private List<TrackSiteModel> _trackSiteModels = new List<TrackSiteModel>();
        private List<TrackHouseModel> _trackHouseModels = new List<TrackHouseModel>();
        Dictionary<string, List<TrackModel>> _trackDic = new Dictionary<string, List<TrackModel>>();//存储当前线上的站点
        private TrackDrawGrid _TrackDrawGrid;
        public void Init(TrackDrawGrid drawGrid)
        {
            _TrackDrawGrid = drawGrid;
            CreateHouse();
            StartCoroutine(CreateTrack());
        }

        private void CreateHouse()
        {
            TrackLevel trackLevel = TrackLevelMgr.Instance.TrackLevel;
            for (int i = 0; i < _houseParent.childCount; i++)
            {
                //↓↓临时随机生成Id,待编辑器完成后根据数据读取Id↓↓
                int houseId = 4001 + i;
                //↑↑临时随机生成Id,待编辑器完成后根据数据读取Id↑↑ 
                TrackLevelHouse trackLevelHouse = TrackLevelMgr.Instance.GetTrackLevelHouseById(houseId);
                TrackHouse trackHouse = TrackHouseMgr.Instance.GetTrackHouseById(houseId);
                trackHouse.isLock = trackLevelHouse.isLock;
                Transform house = _houseParent.GetChild(i);
                Transform lockTrans = house.Find("lock");
                if (trackHouse.isLock)
                    lockTrans.gameObject.SetActive(true);
                else
                    lockTrans.gameObject.SetActive(false);
                TrackHouseModel trackHouseModel = house.GetOrAddComponent<TrackHouseModel>();
                trackHouseModel.Init(trackHouse);
            }
        }

        private IEnumerator CreateTrack()
        {
            TrackLevel trackLevel = TrackLevelMgr.Instance.TrackLevel;
            int count = 0;
            while (count < trackLevel.sites.Count)
            {
                yield return new WaitForSeconds(trackLevel.createCD);
                //↓↓临时随机生成Id,待编辑器完成后根据数据读取Id↓↓
                //int siteId = Random.Range(3001, 3007);
                //↑↑临时随机生成Id,待编辑器完成后根据数据读取Id↑↑
                TrackLevelSite lvSite = trackLevel.sites[count];
                int siteId = lvSite.Id;
                TrackSite trackSite = TrackSiteMgr.Instance.GetTrackSiteById(siteId);
                string prefabPath = PrefabPath.trackSitePath + trackSite.prefabRes;
                GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
                TrackSiteModel trackSiteModel = obj.GetOrAddComponent<TrackSiteModel>();
                
                trackSiteModel.Init(trackSite);
                trackSiteModel.RectTrans.SetParent(_siteParent);
                trackSiteModel.RectTrans.localScale = Vector3.one;
                trackSiteModel.RectTrans.anchoredPosition3D = new Vector3(lvSite.x, lvSite.y,0);
                //_TrackDrawGrid.RandomItemPos(trackSiteModel.RectTrans);
                _trackSiteModels.Add(trackSiteModel);
                count++;
            }
        }
        #region 每条线路上的Site
        public List<TrackModel> GetLineSites(string colorHex)
        {
            List<TrackModel> tracks = null;
            if (!_trackDic.TryGetValue(colorHex, out tracks))
            {
                if (tracks == null)
                    tracks = new List<TrackModel>();
                _trackDic.Add(colorHex, tracks);
            }
            return tracks;
        }
        public void InsertLineSite(TrackModel lastTrack, TrackModel track, string colorHex)
        {
            List<TrackModel> tracks = GetLineSites(colorHex);
            if (tracks.Contains(lastTrack))
            {
                int index = tracks.IndexOf(lastTrack);
                tracks.Insert(index + 1, track);
            }
        }
        public void AddLineSite(TrackModel trackModel, string colorHex)
        {
            List<TrackModel> tracks = GetLineSites(colorHex);
            if (!tracks.Contains(trackModel))
                tracks.Insert(tracks.Count, trackModel);
        }
        public void RemoveLineSite(TrackModel trackModel, string colorHex)
        {
            List<TrackModel> tracks = GetLineSites(colorHex);
            if (tracks.Contains(trackModel))
                tracks.Remove(trackModel);
        }
        #endregion
    }
}

