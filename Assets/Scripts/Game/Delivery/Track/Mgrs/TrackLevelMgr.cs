using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Track
{
    public class TrackLevelMgr : MonoSingleton<TrackLevelMgr>
    {
        private TrackLevel _trackLevel;
        private string _fileName;
        public TrackLevel TrackLevel { get { return _trackLevel; } }
        public void Init(int levelId)
        {
            _fileName = $"TrackLevel_{levelId}.json";
            ReadFile();
        }
        private void ReadFile()
        {
            StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, _fileName), this, (jsonStr) => {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    _trackLevel = FullSerializerAPI.Deserialize(typeof(TrackLevel), jsonStr) as TrackLevel; 
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_TrackData));
                }
            });
        }
        public TrackLevelHouse GetTrackLevelHouseById(int houseId)
        {                                 
            for(int i = 0; i < _trackLevel.houses.Count; i++)
            {
                if (_trackLevel.houses[i].Id == houseId)
                    return _trackLevel.houses[i];
            }
            return null;
        }
        
    }
}

