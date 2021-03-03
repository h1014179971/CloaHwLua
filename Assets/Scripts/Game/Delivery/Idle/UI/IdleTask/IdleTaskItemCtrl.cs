using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Delivery.Idle
{
    public class IdleTaskItemCtrl : MonoBehaviour
    {
        private IdleTaskItem[] idleTaskItems;
        private IdleMainTaskItem mainTaskItem;
        private Dictionary<int, Vector2> taskItemsPos;
        private void Awake()
        {
            taskItemsPos = new Dictionary<int, Vector2>();
            idleTaskItems = transform.GetComponentsInChildren<IdleTaskItem>();
            for(int i=0;i<idleTaskItems.Length;i++)
            {
                int index = int.Parse(idleTaskItems[i].name.Split('_')[1]);
                RectTransform rectTrans = idleTaskItems[i].GetComponent<RectTransform>();
                taskItemsPos.Add(index, rectTrans.anchoredPosition);
            }
            mainTaskItem = GetComponentInChildren<IdleMainTaskItem>();
        }

        public void InitAllTaskItem()
        {
            for(int i=0;i<idleTaskItems.Length;i++)
            {
                idleTaskItems[i].InitData();
            }
            mainTaskItem.InitData();
        }

        public Vector2 GetPosByIndex(int index)
        {
            if (!taskItemsPos.ContainsKey(index)) return Vector2.zero;
            return taskItemsPos[index];
        }

        public Vector2 GetEndPos()
        {
            int index = taskItemsPos.Count - 1;
            return GetPosByIndex(index);
        }

        public int GetEndIndex()
        {
            return taskItemsPos.Count - 1;
        }
    }
}

