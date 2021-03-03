using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

namespace Delivery.Idle
{
    public class IdleMapCityItem : ScrollRectItem
    {
        private IdleMapLogic idleMapLogic;
        private RectTransform rectTrans;
        private void Awake()
        {
            idleMapLogic = IdleMapLogic.Instance;
            rectTrans = GetComponent<RectTransform>();
        }


        private void InitData()
        {
            int unlockCityCount = idleMapLogic.GetAllCityCount();
            for(int i=0;i<3;i++)
            {
                int currentIndex = Index * 3 + i;
                MyButton child = this.GetComponentByPath<MyButton>("btn-city" + i.ToString());
                if (currentIndex >= unlockCityCount)
                {
                    //transform.GetChild(i).gameObject.SetActive(false);
                   child.gameObject.SetActive(false);
                }
                else
                {
                    IdleCity city = idleMapLogic.GetIdleCityByIndex(currentIndex);
                    child.gameObject.SetActive(true);
                    child.GetComponentByPath<MyText>("MyText").text = city.name;
                    child.interactable = idleMapLogic.IsCityUnlock(city);

                }
              
            }
        }

        public override int Index
        {
            get
            {
                return base.Index; ;
            }
            set
            {
                _index = value;
                rectTrans.anchoredPosition = _scroller.GetPosition(_index);
                InitData();
            }
        }

        public override void CreateItem()
        {
            base.CreateItem();
        }

        public override void Init()
        {
            base.Init();
        }
    }
}

