using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Idle
{
    [System.Serializable]
    public class IdleSite
    {
        public readonly int Id;
        public readonly int cityId;
        public readonly string name;
        public readonly string desc;
        public readonly bool isLock;
        public readonly string unlockPrice;
        public readonly int itemId;
        public readonly int siteBaseLv;
        public readonly string siteBase;
        public readonly string siteGrade;
        public readonly int siteTimeLv;
        public readonly string siteTime;
        public readonly int siteVolumeLv;
        public readonly string siteVolume;
        public readonly float posX;
        public readonly float posY;
        public readonly float size;
        public readonly string image;
    }
    public class IdleSiteBase
    {
        public readonly int Lv;
        public readonly string price;
        public readonly string value;
        public readonly int itemnum;
    }
    public class IdleSiteGrade
    {
        public readonly int Id;
        public readonly int lvmin;
        public readonly int lvmax;

        public readonly int multiple;
        public readonly string totalmultiple;
        public readonly string image;
        public readonly int staffnum;
        public readonly string staffRes;
        public readonly string prefabName;
        public readonly int itemIndex;
    }
    public class IdleSiteTime
    {
        public readonly int Lv;
        public readonly string price;
        public readonly float qtime;
        public readonly float speed;
    }
    public class IdleSiteVolume
    {
        public readonly int Lv;
        public readonly string price;
        public readonly int volume;
        public readonly string shelfRes;
    }
}

