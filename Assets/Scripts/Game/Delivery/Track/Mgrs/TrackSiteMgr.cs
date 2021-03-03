using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;

namespace Delivery.Track
{
    public class TrackSiteMgr : MonoSingleton<TrackSiteMgr>
    {
        private Dictionary<int, TrackSite> _trackSiteDic = new Dictionary<int, TrackSite>();
        private TrackSite _trackSite;
        public void Init()
        {
            ReadFile();
        }

        private void ReadFile()
        {
            StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.trackSite), this, (jsonStr) => {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    List<TrackSite> trackSites = FullSerializerAPI.Deserialize(typeof(List<TrackSite>), jsonStr) as List<TrackSite>;
                    _trackSiteDic = trackSites.ToDictionary(key => key.Id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_TrackData));
                }
            });
        }
        public TrackSite GetTrackSiteById(int id)
        {
            TrackSite trackSite;
            _trackSiteDic.TryGetValue(id, out trackSite);
            _trackSite = trackSite;
            return trackSite;
        }
        public TrackSite GetTrackSiteById(string id)
        {
            return GetTrackSiteById(int.Parse(id));
        }
    }
}

