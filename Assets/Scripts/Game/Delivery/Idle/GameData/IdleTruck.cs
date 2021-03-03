using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System;

namespace Delivery.Idle
{
    public class IdleTruck
    {
        //public readonly int Id;
        //public readonly string name;
        //public readonly string desc;

        public readonly string itemId;
        public readonly float speed;
        public readonly int volume;
        public readonly string spriteUp;
        public readonly string spriteDown;
        //public int count { get; set; } //数量
        public IdleTruck(){ }
        public IdleTruck( IdleTruckLv truckLv,IdleTruckRes truckRes)
        {
            //this.Id = truckType.Id;
            //this.name = truckType.name;
            //this.desc = truckType.desc;
            //this.itemId = truckType.itemId;       
            this.speed = truckLv.speed;
            this.volume = truckLv.volume;    
            this.spriteUp = truckRes.spriteUp;
            this.spriteDown = truckRes.spriteDown;
        }
    }
    public class IdleTruckType
    {
        public readonly int Id;
        public readonly string name;
        public readonly string desc;
        public readonly string itemId;
    }
    public class IdleTruckNum
    {
        public readonly int num;
        public readonly string price;
    }
    public class IdleTruckLv
    {
        public readonly int Lv;
        public readonly string price;
        public readonly float speed;
        public readonly int volume;
        public readonly string icon;    
    }
    public class IdleTruckRes
    {
        public readonly int Id;
        public readonly int lvmin;
        public readonly int lvmax;
        public readonly string icon;
        public readonly string spriteUp;
        public readonly string spriteDown;
        public readonly string emptySpriteUp;
        public readonly string emptySpriteDown;
    }

}

