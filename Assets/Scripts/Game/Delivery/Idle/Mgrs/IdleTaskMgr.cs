using Foundation;
using libx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Delivery.Idle
{
    public class IdleTaskMgr : Singleton<IdleTaskMgr>
    {
        private int dataLenth = 2;
        private Dictionary<int, IdleTask> idleTaskDic = new Dictionary<int, IdleTask>();//储存所有任务配置(<idletask.id,idletask>)
        private Dictionary<int, IdleMainTask> idleMainTaskDic = new Dictionary<int, IdleMainTask>();//存储所有主线任务

        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleTask, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleTask> idleTasks = FullSerializerAPI.Deserialize(typeof(List<IdleTask>), jsonStr) as List<IdleTask>;
                    idleTaskDic = idleTasks.ToDictionary(key => key.Id, value => value);
                    dataLenth--;
                    if (dataLenth <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleTask}读取失败");
            });

            AssetLoader.LoadAsync(Files.idleMainTask, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleMainTask> idleMainTasks = FullSerializerAPI.Deserialize(typeof(List<IdleMainTask>), jsonStr) as List<IdleMainTask>;
                    idleMainTaskDic = idleMainTasks.ToDictionary(key => key.Id, value => value);
                    dataLenth--;
                    if (dataLenth <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleTask}读取失败");
            });

            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleTask), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleTask> idleTasks = FullSerializerAPI.Deserialize(typeof(List<IdleTask>), jsonStr) as List<IdleTask>;
            //        idleTaskDic = idleTasks.ToDictionary(key => key.Id, value => value);
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
        }

        public IdleTask GetIdleTask(int id)
        {
            if (!idleTaskDic.ContainsKey(id))
                return null;
            return idleTaskDic[id];
        }

        public List<IdleTask>GetIdleTasks(int cityId)
        {
            List<IdleTask> idleTasks = new List<IdleTask>();
            var enumerator = idleTaskDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                IdleTask idleTask = enumerator.Current.Value;
                if (idleTask.CityId == cityId)
                    idleTasks.Add(idleTask);
            }
            return idleTasks;
        }

        public IdleMainTask GetIdleMainTask(int id)
        {
            if (!idleMainTaskDic.ContainsKey(id))
                return null;
            return idleMainTaskDic[id];
        }

        public List<IdleMainTask>GetIdleMainTasks(int cityId)
        {
            List<IdleMainTask> idleMainTasks = new List<IdleMainTask>();
            var enumerator = idleMainTaskDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                IdleMainTask mainTask = enumerator.Current.Value;
                if (mainTask.CityId == cityId)
                    idleMainTasks.Add(mainTask);
            }
            return idleMainTasks;
        }
        

    }
}

