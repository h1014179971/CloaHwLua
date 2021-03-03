using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;  

namespace Delivery.Idle
{                        
    public class IdleCity 
    {
        public readonly int Id;
        public readonly string name;
        public readonly string desc;
        public readonly string unlockPrice;
        public readonly int truckNum;
        //public readonly string truckIds;
        public readonly string needItem;
        public readonly int outItem;
        public readonly string initialItem;
        public readonly int volumeLv;
        public readonly int cycleLv;
        public readonly string money;
        public readonly int multiple;
        public readonly string totalmultiple;
        public readonly float basecameraX;
        public readonly float basecameraY;
        public readonly float basecameraSize;
        public readonly float stationcameraX;
        public readonly float stationcameraY;
        public readonly float stationcameraSize;

        public readonly int storeLv;
        public readonly float diffTruckTime;
        public readonly float sameTruckTime;

        public IdleCity()
        {
        }
        public IdleCity(IdleCity city)
        {
            this.Id = city.Id;
            this.name = city.name;
            this.desc = city.desc;
            this.unlockPrice = city.unlockPrice;
            this.truckNum = city.truckNum;
            //this.truckIds = city.truckIds;
            this.needItem = city.needItem;
            this.outItem = city.outItem;
            this.initialItem = city.initialItem;
            this.volumeLv = city.volumeLv;
            this.cycleLv = city.cycleLv;
            this.money = city.money;
            this.multiple = city.multiple;
            this.totalmultiple = city.totalmultiple;

            storeLv = city.storeLv;
            diffTruckTime = city.diffTruckTime;
            sameTruckTime=city.sameTruckTime;

            this.basecameraX = city.basecameraX;
            this.basecameraY = city.basecameraY;
            this.basecameraSize = city.basecameraSize;
            this.stationcameraX = city.stationcameraX;
            this.stationcameraY = city.stationcameraY;
            this.stationcameraSize = city.stationcameraSize;

        }
    }
    public class IdleCityVolume
    {
        public int Lv { get; set; }
        public string price { get; set; }
        public int volume { get; set; }

    }
    public class IdleCityCycle
    {
        public int Lv { get; set; }
        public string price { get; set; }
        public float cycle { get; set; }
    }

    public class IdleCityStoreVolume
    {
        public int Lv;
        public string price;
        public int volume;
        public string floorRes;
        public string shelfRes;
        public int staffnum;
    }

}

