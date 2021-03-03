using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;

namespace Delivery.Track
{
    public class TrackItemMgr : MonoSingleton<TrackItemMgr>
    {
        private Dictionary<int, TrackItem> _trackItemDic = new Dictionary<int, TrackItem>();
        private TrackItem _trackItem;
        public void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.trackItem), this, (jsonStr) => {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    List<TrackItem> trackItems = FullSerializerAPI.Deserialize(typeof(List<TrackItem>), jsonStr) as List<TrackItem>;
                    _trackItemDic = trackItems.ToDictionary(key => key.Id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_TrackData));
                }
            });
        }
        public TrackItem GetTrackItemById(int id)
        {
            TrackItem trackItem;
            _trackItemDic.TryGetValue(id, out trackItem);
            _trackItem = trackItem;
            return trackItem;
        }
        public TrackItem GetTrackItemById(string id)
        {
            return GetTrackItemById(int.Parse(id));
        }
    }
}

