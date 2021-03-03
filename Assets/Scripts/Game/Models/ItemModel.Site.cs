using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Foundation;

namespace Delivery
{
    public partial class ItemModel : MonoBehaviour
    {
        
        private void CreateSiteItemPos()
        {
            row = 2;//行数
            col = 4;//列数  
            width = 20;
            height = 30;
            littlePos.Clear();
            Vector2 pos = RectTrans.sizeDelta *0.5f+ new Vector2((col -1) *0.5f * width,(row-1) *0.5f * height);
            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    littlePos.Add(pos + new Vector2(j*width,-i*height));
                }
            }
        }

        private void InitSiteItem()
        {
            
        }
        private void AddSiteItemLittle(ItemLittleModel littleModel,ColorType type, ref List<ItemLittleModel> littles)
        {
            
        } 

        public List<ItemLittleModel> GetSiteItemLittle()
        {
            return itemLittles;
        }

        public void UnLoadSiteItemLittle(ItemLittleModel itemlittleModel)
        {
            if (itemLittles.Contains(itemlittleModel))
                itemLittles.Remove(itemlittleModel);
            if (itemLittles.Count > 0)
                ReLoadSiteItemLittle();
        }
        //重新排列
        private void ReLoadSiteItemLittle()
        {
             for(int i = 0; i < itemLittles.Count; i++)
            {
                itemLittles[i].RectTrans.anchoredPosition = littlePos[i];
            }
        }
    }
}

