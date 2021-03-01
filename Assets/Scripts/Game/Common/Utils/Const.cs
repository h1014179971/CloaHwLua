using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery {
    public class Const
    {
        
    }
    public static class Tags
    {
        public static readonly string Item = "Item";
        public static readonly string CityOutSite = "CityOutSite";//城市出货点
        public static readonly string CityOutPoint = "CityOutPoint";//出货停车点
        public static readonly string CitySite = "CitySite";//收货点
        public static readonly string DeliverySite = "DeliverySite";//快递站
        public static readonly string TruckSite = "TruckSite";//车站
        public static readonly string ForkStopPoint = "ForkStopPoint";//叉车停靠点（也是出生点）
        public static readonly string Npc = "Npc";
        public static readonly string SiteLock = "SiteLock";//驿站锁的标志
        public static readonly string WaitParkPoint = "WaitParkPoint";
        public static readonly string CityStaffWaitPoint = "CityStaffWaitPoint";
        public static readonly string RewardNpc = "RewardNpc";
        public static readonly string WaitParkArea = "WaitParkArea";
        public static readonly string LockShelfArea = "LockShelfArea";
    }
    public static class Layers 
    {
        public static readonly string Road = "Road";
    }

    public static class SortLayers 
    {
        public static readonly string mid = "mid";
    }

    public static class FixScreen
    {
        public static readonly float width = 1080;
        public static readonly float height = 1920;
        //public static readonly float idleCameraSize = 9.6f; //(1920/100/2) pixels per unit =100;
        public static readonly float idleCameraSize = 8f;
    }
    public static class Files
    {
        public static readonly string jsonFolder = "JsonData";
        public static readonly string player = "Player";
        public static string hashPlayer = CryptoPrefs.GetHash(Files.player);
        public static readonly string unitConvert = "UnitConvert.json";
        public static readonly string trackHouse = "TrackHouse.json";
        public static readonly string trackSite = "TrackSite.json";
        public static readonly string trackItem = "TrackItem.json";
        public static readonly string idleCity = "IdleCity.json";
        public static readonly string idleCityVolume = "IdleCityVolume.json";
        public static readonly string idleCityCycle = "IdleCityCycle.json";
        public static readonly string idleCityStoreVolume = "IdleCityStoreVolume.json";
        public static readonly string idleSite = "IdleSite.json";
        public static readonly string idleTruckType = "IdleTruckType.json";
        public static readonly string idleTruckLv = "IdleTruckLv.json";
        public static readonly string idleTruckRes = "IdleTruckRes.json";
        public static readonly string idleTruckNum = "IdleTruckNum.json";
        public static readonly string idleItem = "IdleItem.json";
        public static readonly string idleTask = "IdleTask.json";
        public static readonly string idleMainTask = "IdleMainTask.json";
        public static readonly string idleGuide = "IdleGuide.json";
        public static readonly string idleGuideStep = "IdleGuideStep.json";
        public static readonly string idleSpecialEvent = "IdleSpecialEvent.json";
        public static readonly string idleCityTruckRes = "IdleCityTruckRes.json";
        public static readonly string idleShelfRes = "IdleShelfRes.json";
    }
    public static class PrefabPath
    {
        public static readonly string uavPath = "Prefabs/Uav/";
        public static readonly string itemLittlePath = "Prefabs/Shape/";
        public static readonly string trackSitePath = "Prefabs/Track/Site/";
        public static readonly string idlePath = "Prefabs/Idle/";
        public static readonly string idleTruckRes = "Textures/Idle/truck/";
    }
    public static class PrefabName 
    {
        public static readonly string staff = "staff1.prefab";
        public static readonly string staff2 = "staff2.prefab";
        public static readonly string remover = "remover.prefab";
        public static readonly string plane =  "plane.prefab";
        public static readonly string truck =  "truck.prefab";
        public static readonly string fork_down_left = "fork_down_left.prefab";
        public static readonly string fork_down_right = "fork_down_right.prefab";
        public static readonly string fork_up_left = "fork_up_left.prefab";
        public static readonly string fork_up_right = "fork_up_right.prefab";
        public static readonly string npcTalk = "npctalk.prefab";
        public static readonly string truckProcess = "truckUnloadProcess.prefab";
        public static readonly string tipUI = "tipUI.prefab";
        public static readonly string faced = "faced.prefab";
        public static readonly string finger = "finger.prefab";
        public static readonly string xiangzi = "xiangzi.prefab";
        public static readonly string doubleIncomeTip = "doubleIncomeTip.prefab";
    }

    public static class FxPrefabPath 
    {
        //public static readonly string idleCreateSite = "Prefabs/Idle/Fx/FX_KD_jianzao_lizi.prefab";
        //public static readonly string idleAddMoney = "Prefabs/Idle/Fx/FX_KD_moneyGet.prefab";
        //public static readonly string uiIdleAddMoney = "Prefabs/Idle/Fx/FX_UI_Money.prefab";
        //public static readonly string idleCityGrade = "Prefabs/Idle/Fx/FX_KD_shengji_lizi.prefab";
        //public static readonly string idleSiteGrade = "Prefabs/Idle/Fx/FX_KD_shengji_site.prefab";
        //public static readonly string idleSiteView = "Prefabs/Idle/Fx/Fx_UI_SiteView.prefab";
        //public static readonly string idleGuideClickTarget = "Prefabs/Idle/Fx/FX_dianji.prefab";
        //public static readonly string idleCreateSite = "FX_KD_jianzao_lizi.prefab";
        public static readonly string idleCreateSite = "fx_jianzaoyanwu_01.prefab";
        public static readonly string idleFinishCreateSite = "fx_lihua.prefab";
        public static readonly string idleAddMoney = "FX_KD_moneyGet.prefab";
        public static readonly string uiIdleAddMoney = "FX_UI_Money.prefab";
        public static readonly string idleCityGrade = "FX_KD_shengji_lizi.prefab";
        //public static readonly string idleSiteGrade = "FX_KD_shengji_site.prefab";
        public static readonly string idleSiteGrade = "fx_nongchangshengji_01.prefab";
        public static readonly string idleGradeFx = "fx_tchechang.prefab";
        public static readonly string idleSiteView = "Fx_UI_SiteView.prefab";
        public static readonly string idleGuideClickTarget = "FX_dianji.prefab";
        public static readonly string idleGuideTargetCircle = "FX_KD_quan.prefab";
        public static readonly string idleTriggerSpecialEvent = "FX_KD_caidai_lizi.prefab";
        public static readonly string idleInvestorReward = "fx_feiqian.prefab";
        public static readonly string idleShelfItemShow = "fx_youxi_jieshu_01.prefab";
        public static readonly string idleMoneyBoom = "fx_feijinbi_baoji.prefab";
        public static readonly string idleClickFx = "fx_baoguang_dianji.prefab";
    }
    public static class GameAudio
    {
        public static readonly string bgm = "bgm.mp3";
        public static readonly string vendingMoney = "VendingMoney.mp3";
        public static readonly string buttonClick = "ButtonClick.mp3";
        public static readonly string buttonClose = "ButtonClose.mp3";
        public static readonly string showItem = "ShowItem.mp3";
        public static readonly string firework = "audio_fireworks.mp3";
        public static readonly string xiangzidown = "xiangzidown.mp3";
        public static readonly string login = "Login.mp3";
        public static readonly string clickFeiji = "clickFeiji.mp3";//点击飞机
        public static readonly string clickNpcTalk = "clickNpcTalk.mp3";//点击npc说话气泡
        public static readonly string refreshFeiji = "refreshFeiji.mp3";//飞机刷新
        public static readonly string staffLoadItem = "staffLoadItem.mp3";//驿站快递员取货
        public static readonly string moneyFxShow = "MoneyFxShow.mp3";//飞钱音效
        public static readonly string doubleIncome = "DoubleIncome.mp3";//双倍收益音效
        public static readonly string BuildSmoke = "BuildSmoke.mp3";//建造烟雾特效
        public static readonly string forkUnloadItem = "forkUnloadItem.mp3";//叉车装卸货
        public static readonly string siteTruckStop = "siteTruckStop.mp3";//快递车停止
        public static readonly string cityTruckMove = "cityTruckMove.mp3";//城市卡车开过来
        public static readonly string cityTruckStop = "cityTruckStop.mp3";//城市卡车倒车
    }

    public static class Config
    {
        public static readonly Vector3 objPoolPos = new Vector3(-10000, -10000, -10000);
        public static readonly long leaveTime = 7200;//离线最长时间
        public static readonly string Fx_Riddle_yanwu = "Prefabs/Fx/yanwu_d";
        public static readonly string Fx_Riddle_qiaoda = "Prefabs/Fx/yanwu_d_qiaoda";
        public static readonly string Fx_Riddle_yanchen = "Prefabs/Fx/yanchen_d";
        public static readonly string Fx_Riddle_baozha = "Prefabs/Fx/yanwu_d_baozha";
        public static readonly int maxShelfStoreyCount = 3;//货架最大层数
    }




}


