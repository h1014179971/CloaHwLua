using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    public class ItemCtrl : MonoSingleton<ItemCtrl>
    {
        [SerializeField]private List<ItemModel> itemModules;
        Dictionary<ColorType, List<ItemModel>> itemDic = new Dictionary<ColorType, List<ItemModel>>();//存储当前线上的站点
        private DrawLineCtrl drawLineCtrl;
        public void Init(DrawLineCtrl lineCtrl)
        {
            drawLineCtrl = lineCtrl;
            StartCoroutine(CreateItem());
        }

        
        private IEnumerator CreateItem()
        {
            int count = 0;
            while(count < itemModules.Count)
            {
                yield return new WaitForSeconds(0.05f);
                drawLineCtrl.RandomItemPos(itemModules[count].RectTrans);
                count++;
            } 
        }
        #region 每条线路上的Item
        public List<ItemModel> GetLineItems(ColorType type)
        {
            List<ItemModel> items = null;
            if (!itemDic.TryGetValue(type, out items))
            {
                if (items == null)
                    items = new List<ItemModel>();
                itemDic.Add(type, items);
            }
            return items;
        }
        public void InsertLineItem(ItemModel lastItem, ItemModel item, ColorType type)
        {
            List<ItemModel> items = GetLineItems(type);
            if (items.Contains(lastItem))
            {
                int index = items.IndexOf(lastItem);
                items.Insert(index + 1, item);
            }
        }
        public void AddLineItem(ItemModel itemModel, ColorType type)
        {
            List<ItemModel> items = GetLineItems(type);
            if (!items.Contains(itemModel))
                items.Insert(items.Count, itemModel);
        }
        public void RemoveLineItem(ItemModel itemModel, ColorType type)
        {
            List<ItemModel> items = GetLineItems(type);
            if (items.Contains(itemModel))
                items.Remove(itemModel);
        }
        #endregion
    }
}

