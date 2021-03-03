using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Delivery.Idle;
using Delivery.Track;
using System;

namespace Delivery
{
    //public class Player
    //{
    //    public int cityId;//当前城市id;
    //    public Long2 money;
    //    public Dictionary<int,int> truckLv; //卡车等级
    //    public Dictionary<int,int> truckNum; //卡车数量
    //    public List<IdleCity> idleCity { get; set; }
    //    public Dictionary<int, List<IdleSite>> idleSite { get; set; }
    //    public Dictionary<int, List<IdleTruck>> idleTruck { get; set; }
    //}
    [System.Serializable]
    public class Player
    {
        public int cityId;//当前城市id; 
        public Dictionary<int, PlayerCity> playerCity;
        public Dictionary<int,PlayerGuide> playerGuide;
        public long firstLoginTime; //玩家第一次登陆时间
    }
    [System.Serializable]
    public class PlayerCity
    {
        public int cityId;
        public Long2 money;
        public long loginTime; //登录时间
        public long leaveTime; //离开时间
        public long lastDoubleIncomeTime;//上次开启双倍收益的时间
        public int lastDoubleIncomeTotalTime;//上次双倍收益的总时间
        public int cityTruckVolumeLv;  //站点卡车容量等级
        public int cycleLv;
        public int storeLv;//仓库容量等级
        public Dictionary<int, int> storeItems;//当前仓库储存的货物（<货物id，货物数量>）
        //public Long2 currentStoreItemNum;//当前仓库存储货物的总数量
        public List<PlayerSite> playerSites;
        public PlayerTruck playerTruck;
        //public List<PlayerTruck> playerTrucks;//车类型
        public List<PlayerTask> playerTasks;//当前城市任务
        public List<PlayerTask> playerMainTasks;//当前城市主线任务
    }
    [System.Serializable]
    public class PlayerSite 
    {
        public int Id { get; set; }
        public int cityId { get; set; }
        public bool isLock { get; set; }
        public int siteBaseLv { get; set; }
        public int siteTimeLv { get; set; }
        public int siteVolumeLv { get; set; }
        public int loadItemNum { get; set; }
    }

    [System.Serializable]
    public class PlayerTruck 
    {
        //public int Id { get; set; }
        //public int count { get; set; } //数量
        public int truckLv;
        public int truckNum;
    }

    public class PlayerTask
    {
        public int Id { get; set; }
        public bool IsFinish { get; set; }//是否已完成(未领取)
    }

    public class PlayerGuide
    {
        public int id { get; set; }
        public bool hasStart { get; set; }
    }

}

