using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    public class TrackSiteModel : TrackModel
    {
        private TrackSite _trackSite;
        private Dictionary<ColorType, List<TrackModel>> _trackLineDic = new Dictionary<ColorType, List<TrackModel>>();
        public TrackSite TrackSite { get { return _trackSite; } }
        public void Init(TrackSite trackSite)
        {
            _trackSite = trackSite;
        }
    }
}

