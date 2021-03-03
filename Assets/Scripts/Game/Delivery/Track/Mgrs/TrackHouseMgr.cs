using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;

namespace Delivery.Track
{
    public class TrackHouseMgr : MonoSingleton<TrackHouseMgr>
    {
        private Dictionary<int, TrackHouse> _trackHouseDic = new Dictionary<int, TrackHouse>();
        private TrackHouse _trackHouse;
        public void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.trackHouse), this, (jsonStr) => {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    List<TrackHouse> trackHouses = FullSerializerAPI.Deserialize(typeof(List<TrackHouse>), jsonStr) as List<TrackHouse>;
                    _trackHouseDic = trackHouses.ToDictionary(key => key.Id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_TrackData));
                }
            });
        }
        public TrackHouse GetTrackHouseById(int id)
        {
            TrackHouse trackHouse;
            _trackHouseDic.TryGetValue(id, out trackHouse);
            _trackHouse = trackHouse;
            return trackHouse;
        }
        public TrackHouse GetTrackHouseById(string id)
        {
            return GetTrackHouseById(int.Parse(id));
        }
    }
}

