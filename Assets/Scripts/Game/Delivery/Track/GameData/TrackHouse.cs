using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    public class TrackHouse
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string siteType { get; set; }
        public string colorHex { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public bool isLock;
    }
}

