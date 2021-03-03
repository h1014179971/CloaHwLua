using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Foundation;

namespace Delivery
{
    public partial class ItemModel : MonoBehaviour
    {
        

        TimerEvent timer = null;
        private void CreatePos()
        {
            row = 2;//行数
            col = 3;//列数  
            width = 20;
            height = 30;

            littlePos.Clear();
            Vector2 pos =  new Vector2(-(col -1) *0.5f * width,(row-1) *0.5f * height);
            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    littlePos.Add(pos + new Vector2(j*width,-i*height));
                }
            }
        }

        private void InitHouseItem()
        {
            CreatePos();
            CreateItemLittle();
        }
        private void CreateItemLittle()
        {
            timer = Timer.Instance.Register(2, -1, (pare) => {
                if (itemLittles.Count >= row * col) return;
                int id = Random.Range(3001, 3007);
                string prefabPath = PrefabPath.itemLittlePath + "item_little_" + id;
                GameObject littleObj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
                ItemLittleModel itemLittleModel = littleObj.GetOrAddComponent<ItemLittleModel>();
                itemLittleModel.Id = id;
                itemLittleModel.RectTrans.SetParent(RectTrans);
                itemLittleModel.RectTrans.anchoredPosition3D = littlePos[itemLittles.Count];
                itemLittleModel.RectTrans.localEulerAngles = Vector3.zero;
                itemLittleModel.RectTrans.localScale = Vector3.one;
                itemLittles.Add(itemLittleModel);  
            }).AddTo(gameObject);
        } 

        public List<ItemLittleModel> GetHouseItemLittle()
        {
            return itemLittles;
        }

        public void UnLoadItemLittle(ItemLittleModel itemlittleModel)
        {
            if (itemLittles.Contains(itemlittleModel))
                itemLittles.Remove(itemlittleModel);
            if (itemLittles.Count > 0)
                ReLoadItemLittle();
        }
        //重新排列
        private void ReLoadItemLittle()
        {
             for(int i = 0; i < itemLittles.Count; i++)
            {
                itemLittles[i].RectTrans.anchoredPosition = littlePos[i];
            }
        }
    }
}

