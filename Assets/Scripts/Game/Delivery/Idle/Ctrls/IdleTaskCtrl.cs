using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    /*
            任务类型1：第X个驿站升级到X级。
            任务类型2：调配X辆。
            任务类型3：站点升到X级.
            任务类型4：解锁第X个驿站。
            任务类型5：解锁第X个城市
        */
    public enum TaskType
    {
        Task_LevelUpPostSite=1,
        Task_AllocateTruck,
        Task_LevelUpSite,
        Task_UnlockPostSite,
        Task_UnlockCity,
        Task_LevelUpCityVolume,
        Task_LevelUpSiteVolume,
        Task_LevelUpTruckVolume,
        Task_LevelUpStaffSpeed
    }

    public class IdleTaskCtrl : MonoSingleton<IdleTaskCtrl>
    {
        private const int maxShowTask = 3;//最多显示的任务数量
        private List<PlayerTask> playerTasks;//普通任务
        private PlayerTask mainTask;//当前主线任务
        private Dictionary<TaskType, Func<IdleTaskBase,bool>> _judgeFuncs;
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Task_ProcessChange, OnTaskProcessChange);
        }
        public void Init()
        {
            playerTasks = PlayerMgr.Instance.PlayerCity.playerTasks;
            mainTask = PlayerMgr.Instance.GetCurrentMainTask();
            InitJudgeFuncs();
            InitTaskProcess();
        }
        public override void Dispose()
        {
            base.Dispose();
            playerTasks = null;
            _judgeFuncs = null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Task_ProcessChange, OnTaskProcessChange);
        }

        public IdleMainTask GetIdleMainTask()
        {
            if (mainTask == null) return null;
            return IdleTaskMgr.Instance.GetIdleMainTask(mainTask.Id);
        }

        //前往任务指引的界面
        public void GotoOtherWindow(IdleTaskBase idleTask)
        {
            TaskType taskType = (TaskType)idleTask.TaskType;
            switch (taskType)
            {
                case TaskType.Task_LevelUpStaffSpeed:
                case TaskType.Task_LevelUpSiteVolume:
                case TaskType.Task_LevelUpPostSite:
                    IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(idleTask.Parameter1);
                    if (siteModel != null)
                    {
                        EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
                    }
                    break;
                case TaskType.Task_LevelUpTruckVolume:
                case TaskType.Task_AllocateTruck:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
                    break;
                case TaskType.Task_LevelUpCityVolume:
                case TaskType.Task_LevelUpSite:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
                    break;
                case TaskType.Task_UnlockPostSite:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowBuild));
                    break;
                case TaskType.Task_UnlockCity:
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowMap));
                    break;
            }
        }


        public TaskProcess GetTaskProcess(IdleTaskBase idleTask)
        {
            TaskType taskType = (TaskType)idleTask.TaskType;
            TaskProcess process = new TaskProcess();
            switch (taskType)
            {
                case TaskType.Task_LevelUpPostSite:
                    PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(idleTask.Parameter1);
                    process.maxProcess = idleTask.Parameter2;
                    if (playerSite != null)
                        process.currentProcess = playerSite.siteBaseLv;
                    else
                        process.currentProcess = 0;
                    break;
                case TaskType.Task_AllocateTruck:
                    process.maxProcess = idleTask.Parameter1;
                    process.currentProcess = PlayerMgr.Instance.GetTruckNum();
                    break;
                case TaskType.Task_LevelUpSite:
                    process.maxProcess = idleTask.Parameter1;
                    process.currentProcess = PlayerMgr.Instance.PlayerCity.cityTruckVolumeLv;
                    break;
                case TaskType.Task_UnlockPostSite:
                    PlayerSite playerPostSite = PlayerMgr.Instance.GetPlayerSite(idleTask.Parameter1);
                    process.maxProcess = 1;
                    if (playerPostSite != null)
                        process.currentProcess = playerPostSite.isLock ? 0 : 1;
                    else
                        process.currentProcess = 0;
                    break;

                case TaskType.Task_UnlockCity:
                    PlayerCity playerCity = PlayerMgr.Instance.GetPlayerCity(idleTask.Parameter1);
                    process.maxProcess = 1;
                    process.currentProcess = playerCity == null ? 0 : 1;
                    break;
                case TaskType.Task_LevelUpCityVolume:
                    int storeLv = PlayerMgr.Instance.PlayerCity.storeLv;
                    process.maxProcess = idleTask.Parameter1;
                    process.currentProcess = storeLv;
                    break;
                case TaskType.Task_LevelUpSiteVolume:
                    PlayerSite volumePostSite = PlayerMgr.Instance.GetPlayerSite(idleTask.Parameter1);
                    process.maxProcess = idleTask.Parameter2;
                    if (volumePostSite != null && !volumePostSite.isLock)
                        process.currentProcess = volumePostSite.siteVolumeLv;
                    else
                        process.currentProcess = 0;
                    break;
                case TaskType.Task_LevelUpTruckVolume:
                    int truckLv = PlayerMgr.Instance.GetTruckLv();
                    process.maxProcess = idleTask.Parameter1;
                    process.currentProcess = truckLv;
                    break;
                case TaskType.Task_LevelUpStaffSpeed:
                    PlayerSite speedPostSite = PlayerMgr.Instance.GetPlayerSite(idleTask.Parameter1);
                    process.maxProcess = idleTask.Parameter2;
                    if (speedPostSite != null && !speedPostSite.isLock)
                        process.currentProcess = speedPostSite.siteTimeLv;
                    else
                        process.currentProcess = 0;
                    break;
            }
            return process;
        }

        private void OnTaskProcessChange(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<TaskType> argsOne = (EventArgsOne<TaskType>)baseEventArgs;
            bool isAnyTaskFinish = false;
            TaskType taskType = argsOne.param1;
            int currentTaskCount = maxShowTask;
            if (maxShowTask > playerTasks.Count)
                currentTaskCount = playerTasks.Count;
            for (int i=0;i< currentTaskCount; i++)
            {
                PlayerTask playerTask = playerTasks[i];
                if (playerTask.IsFinish) continue;
                IdleTask idleTask = IdleTaskMgr.Instance.GetIdleTask(playerTask.Id);
                if(idleTask.TaskType==(int)taskType)
                {
                    if (_judgeFuncs.ContainsKey(taskType))
                    {
                        if(_judgeFuncs[taskType].Invoke(idleTask))
                        {
                            playerTask.IsFinish = true;
                            isAnyTaskFinish = true;
                        }
                    }
                    continue;
                }
            }

            //判断主线任务是否完成
            mainTask = PlayerMgr.Instance.GetCurrentMainTask();
            if (mainTask != null && !mainTask.IsFinish)
            {
                IdleMainTask idleMainTask = IdleTaskMgr.Instance.GetIdleMainTask(mainTask.Id);
                if(taskType== (TaskType)idleMainTask.TaskType)
                {
                    if (_judgeFuncs.ContainsKey(taskType))
                    {
                        if (_judgeFuncs[taskType].Invoke(idleMainTask))
                        {
                            isAnyTaskFinish = true;
                            mainTask.IsFinish = true;
                            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Task_MainTaskComplete));
                        }
                    }
                }
            }
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_TaskBtn_ShowNode, isAnyTaskFinish));
        }

        public void InitTaskProcess()
        {
            if (playerTasks == null) return;
            bool isAnyTaskFinish = false;
            //判断是否有普通任务完成
            int currentTaskCount = maxShowTask;
            if (maxShowTask > playerTasks.Count)
                currentTaskCount = playerTasks.Count;
            for (int i = 0; i < currentTaskCount; i++)
            {
                PlayerTask playerTask = playerTasks[i];
                if (playerTask.IsFinish)
                {
                    isAnyTaskFinish = true;
                    continue;
                }
                IdleTask idleTask = IdleTaskMgr.Instance.GetIdleTask(playerTask.Id);
                TaskType taskType = (TaskType)idleTask.TaskType;
                if (_judgeFuncs.ContainsKey(taskType))
                {
                    if (_judgeFuncs[taskType].Invoke(idleTask))
                    {
                        isAnyTaskFinish = true;
                        playerTask.IsFinish = true;
                    }
                }
            }
            //判断主线任务是否完成
            mainTask = PlayerMgr.Instance.GetCurrentMainTask();
            if (mainTask!=null)
            {
                if (mainTask.IsFinish)
                {
                    isAnyTaskFinish = true;
                }
                else
                {
                    IdleMainTask idleMainTask = IdleTaskMgr.Instance.GetIdleMainTask(mainTask.Id);
                    TaskType taskType = (TaskType)idleMainTask.TaskType;
                    if (_judgeFuncs.ContainsKey(taskType))
                    {
                        if (_judgeFuncs[taskType].Invoke(idleMainTask))
                        {
                            isAnyTaskFinish = true;
                            mainTask.IsFinish = true;
                        }
                    }
                }
            }
           
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_TaskBtn_ShowNode,isAnyTaskFinish));
        }


        private void InitJudgeFuncs()
        {
            _judgeFuncs = new Dictionary<TaskType, Func<IdleTaskBase,bool>>();
            _judgeFuncs.Add(TaskType.Task_LevelUpPostSite, IsFinishTask1);
            _judgeFuncs.Add(TaskType.Task_AllocateTruck, IsFinishTask2);
            _judgeFuncs.Add(TaskType.Task_LevelUpSite, IsFinishTask3);
            _judgeFuncs.Add(TaskType.Task_UnlockPostSite, IsFinishTask4);
            _judgeFuncs.Add(TaskType.Task_UnlockCity, IsFinishTask5);
            _judgeFuncs.Add(TaskType.Task_LevelUpCityVolume, IsFinishTask6);
            _judgeFuncs.Add(TaskType.Task_LevelUpSiteVolume, IsFinishTask7);
            _judgeFuncs.Add(TaskType.Task_LevelUpTruckVolume, IsFinishTask8);
            _judgeFuncs.Add(TaskType.Task_LevelUpStaffSpeed, IsFinishTask9);
        }


        #region 任务是否完成判断方法
        /*
            1：id为Parameter1的驿站升级到Parameter2级            2：解锁Parameter1辆车            3：大货车载量升到Parameter1级            4：解锁id为Parameter1的驿站            5：解锁id为Parameter1的城市            6：快递中心的货架等级升到Parameter1            7：id为Parameter1的驿站升到Parameter2级            8：快递车载量等级升到Parameter1级            9：id为Parameter1的驿站配送时间等级升到Parameter2级
        */
        private bool IsFinishTask1(IdleTaskBase idleTask)
        {
            int siteId = idleTask.Parameter1;
            int siteLv = idleTask.Parameter2;
            PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(siteId);
            if (playerSite == null || playerSite.isLock || playerSite.siteBaseLv < siteLv) return false;
            return true;
        }

        private bool IsFinishTask2(IdleTaskBase idleTask)
        {
            int truckCount = idleTask.Parameter1;
            int currentCount = PlayerMgr.Instance.GetTruckNum();
            if (currentCount < truckCount) return false;
            return true;
            //int usedCount = PlayerMgr.Instance.GetUseTruckCount();
            //if (usedCount < truckCount) return false;
            //return true;
        }

        private bool IsFinishTask3(IdleTaskBase idleTask)
        {
            int cityLv = idleTask.Parameter1;
            int playerCityLv = PlayerMgr.Instance.PlayerCity.cityTruckVolumeLv;
            if (playerCityLv < cityLv) return false;
            return true;
        }
        private bool IsFinishTask4(IdleTaskBase idleTask)
        {
            int siteCount = idleTask.Parameter1;
            int unlockSiteCount = PlayerMgr.Instance.GetUnlockSiteCount();
            if (unlockSiteCount < siteCount) return false;
            return true;
        }

        private bool IsFinishTask5(IdleTaskBase idleTask)
        {
            int cityId = idleTask.Parameter1;
            PlayerCity city = PlayerMgr.Instance.GetPlayerCity(cityId);
            if (city == null) return false;
            return true;
        }

        private bool IsFinishTask6(IdleTaskBase idleTask)
        {
            int currentShelfLv = PlayerMgr.Instance.PlayerCity.storeLv;
            int targetShelfLv = idleTask.Parameter1;
            return targetShelfLv >= currentShelfLv;
        }

        private bool IsFinishTask7(IdleTaskBase idleTask)
        {
            int postSiteId = idleTask.Parameter1;
            int targetLv = idleTask.Parameter2;
            PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(postSiteId);
            if (playerSite == null || playerSite.isLock) return false;
            return playerSite.siteVolumeLv >= targetLv;
        }

        private bool IsFinishTask8(IdleTaskBase idleTask)
        {
            int currentTruckLv = PlayerMgr.Instance.GetTruckLv();
            int targetLv = idleTask.Parameter1;
            return currentTruckLv >= targetLv;
        }

        private bool IsFinishTask9(IdleTaskBase idleTask)
        {
            int postSiteId = idleTask.Parameter1;
            int targetLv = idleTask.Parameter2;
            PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(postSiteId);
            if (playerSite == null || playerSite.isLock) return false;
            return playerSite.siteTimeLv >= targetLv;
        }

        #endregion


    }
}

