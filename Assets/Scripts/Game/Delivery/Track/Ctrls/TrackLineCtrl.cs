using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    public class TrackLineCtrl : MonoSingleton<TrackLineCtrl>
    {
        private List<TrackLineModel> _trackLineModels = new List<TrackLineModel>();
        public List<TrackLineModel> TrackLineModels()
        {
            return _trackLineModels;
        }
        public void AddTrackLineModel(TrackLineModel trackLineModel)
        {
            if (!_trackLineModels.Contains(trackLineModel))
                _trackLineModels.Add(trackLineModel);
        }
    }
}

