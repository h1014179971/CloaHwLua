using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
namespace Delivery.Idle
{
    public struct TaskProcess
    {
        public int currentProcess;
        public int maxProcess;
    }
    public class IdleTaskWindowLogic : Singleton<IdleTaskWindowLogic>
    {
        private PlayerMgr playerMgr;
        private IdleTaskMgr idleTaskMgr;
        private List<PlayerTask> playerTasks;
        public void InitData()
        {
            playerMgr = PlayerMgr.Instance;
            idleTaskMgr = IdleTaskMgr.Instance;
            playerTasks = playerMgr.PlayerCity.playerTasks;
        }


        public PlayerTask GetMainTask()
        {
            return playerMgr.GetCurrentMainTask();
        }

        public IdleMainTask GetIdleMainTask()
        {
            PlayerTask playerTask = GetMainTask();
            if (playerTask == null) return null;
            return idleTaskMgr.GetIdleMainTask(playerTask.Id);
        }

       public PlayerTask GetPlayerTaskByIndex(int index)
        {
            if (index < 0 || index >= playerTasks.Count) return null;
            return playerTasks[index];
        }

        public IdleTask GetIdleTaskByIndex(int index)
        {
            PlayerTask playerTask = GetPlayerTaskByIndex(index);
            if (playerTask==null) return null;
            IdleTask idleTask = idleTaskMgr.GetIdleTask(playerTask.Id);
            return idleTask;
        }
        
        public TaskProcess GetCurrentProcessValue(IdleTaskBase idleTask)
        {
            return IdleTaskCtrl.Instance.GetTaskProcess(idleTask);
            //TaskType taskType = (TaskType)idleTask.TaskType;
            //TaskProcess process = new TaskProcess();
            //switch (taskType)
            //{
            //    case TaskType.Task_LevelUpPostSite:
            //        PlayerSite playerSite = playerMgr.GetPlayerSite(idleTask.Parameter1);
            //        process.maxProcess = idleTask.Parameter2;
            //        if (playerSite != null)
            //            process.currentProcess = playerSite.siteBaseLv;
            //        else
            //            process.currentProcess = 0;
            //        break;
            //    case TaskType.Task_AllocateTruck:
            //        process.maxProcess = idleTask.Parameter1;
            //        process.currentProcess = playerMgr.GetTruckNum();
            //        break;
            //    case TaskType.Task_LevelUpSite:
            //        process.maxProcess = idleTask.Parameter1;
            //        process.currentProcess = playerMgr.PlayerCity.cityTruckVolumeLv;
            //        break;
            //    case TaskType.Task_UnlockPostSite:
            //        PlayerSite playerPostSite = playerMgr.GetPlayerSite(idleTask.Parameter1);
            //        process.maxProcess = 1;
            //        if (playerPostSite != null)
            //            process.currentProcess = playerPostSite.isLock ? 0 : 1;
            //        else
            //            process.currentProcess = 0;
            //        break;
                    
            //    case TaskType.Task_UnlockCity:
            //        PlayerCity playerCity = playerMgr.GetPlayerCity(idleTask.Parameter1);
            //        process.maxProcess = 1;
            //        process.currentProcess = playerCity == null ? 0 : 1;
            //        break;
            //}
            //return process;
        }



        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }

        public void OnReceiveMainTaskBtnClick()
        {
            IdleMainTask idleMainTask = GetIdleMainTask();
            playerMgr.ReceiveTaskReward(idleMainTask, true);
            
        }

        public void OnReveiveBtnClick(int index)
        {
            IdleTask idleTask = GetIdleTaskByIndex(index);
            playerMgr.ReveiveReward(idleTask);

            Dictionary<string, string> siteProertie = new Dictionary<string, string>();
            siteProertie["taskid"] = idleTask.Id.ToString();
            PlatformFactory.Instance.TAEventPropertie("task_finish", siteProertie);
          
        }

        public void ShowOtherWindow(IdleTaskBase idleTask)
        {
            IdleTaskCtrl.Instance.GotoOtherWindow(idleTask);
            //TaskType taskType = (TaskType)idleTask.TaskType;
            //switch (taskType)
            //{
            //    case TaskType.Task_LevelUpPostSite:
            //        IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(idleTask.Parameter1);
            //        if (siteModel != null)
            //        {
            //            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
            //        }
            //        break;
            //    case TaskType.Task_AllocateTruck:
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
            //        break;
            //    case TaskType.Task_LevelUpSite:
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
            //        break;
            //    case TaskType.Task_UnlockPostSite:
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowBuild));
            //        break;
            //    case TaskType.Task_UnlockCity:
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowMap));
            //        break;
            //}
        }

        public void OnGotoBtnClick(int index)
        {
            IdleTask idleTask = GetIdleTaskByIndex(index);
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleTaskBase>(EnumEventType.Event_Window_CloseTask,idleTask));
        }

        public void OnGotoBtnClick()
        {
            IdleMainTask idleTask = GetIdleMainTask();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleTaskBase>(EnumEventType.Event_Window_CloseTask, idleTask));
        }

   

    }
}

