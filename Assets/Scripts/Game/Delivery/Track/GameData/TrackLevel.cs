using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    [System.Serializable]
    public class TrackLevel
    {
        public List<TrackLevelHouse> houses;
        public List<TrackLevelSite> sites;
        public float createCD;//站点出现cd
    }
    [System.Serializable]
    public class TrackLevelHouse
    {
        public int Id;
        public bool isLock;
    }
    [System.Serializable]
    public class TrackLevelSite
    {
        public int Id;
        public float x;
        public float y;
    }
}

