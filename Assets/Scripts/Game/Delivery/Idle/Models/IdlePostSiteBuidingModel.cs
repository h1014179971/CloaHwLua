using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
namespace Delivery.Idle
{
    public class IdlePostSiteBuidingModel : MonoBehaviour
    {
        public string PrefabName;
        private Dictionary<int, List<IdlePostSiteItemModel>> _allItems = new Dictionary<int, List<IdlePostSiteItemModel>>();//驿站所有可解锁物品
        private CameraAotoMove autoMove;

        public void Init(string prefabName)
        {
            autoMove = FindObjectOfType<CameraAotoMove>();
            PrefabName = prefabName;
            IdlePostSiteItemModel[] items = GetComponentsInChildren<IdlePostSiteItemModel>();
            for(int i=0;i< items.Length;i++)
            {
                IdlePostSiteItemModel item = items[i];
                item.Hide();
                if(!_allItems.ContainsKey(item.index))
                {
                    List<IdlePostSiteItemModel> itemList = new List<IdlePostSiteItemModel>();
                    _allItems.Add(item.index, itemList);
                }
                
                _allItems[item.index].Add(item);
            }
        }

        public void ShowItems(int index,bool moveCamera=true)
        {
            if(_allItems.ContainsKey(index))
            {
                List<IdlePostSiteItemModel> itemList = _allItems[index];
                if(moveCamera)
                {
                    int focusItemIndex = Random.Range(0, itemList.Count);
                    float dy = Camera.main.transform.position.y - transform.position.y;
                    Vector3 pos = itemList[focusItemIndex].transform.position;
                    pos.y += dy;

                    float cameraSize = 5;
                    EventDispatcher.Instance.TriggerEvent(new EventArgsThree<Vector3, float,UnityAction>(EnumEventType.Event_Camera_SimpleMoveToTarget, pos, cameraSize,()=> {
                        ShowItems(itemList,moveCamera);
                    }));
                }
                else
                {
                    ShowItems(itemList,moveCamera);
                }
                
            }
        }

        private void ShowItems(List<IdlePostSiteItemModel>itemList,bool moveCamera)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].Show();
                if (moveCamera)
                {
                    itemList[i].transform.DOScale(1.5f, 0.3f).SetDelay(0.5f).SetLoops(2, LoopType.Yoyo);
                }
                FxModel fx = FxCtrl.Instance.PlayFx(FxPrefabPath.idleShelfItemShow, itemList[i].transform.position,-1);
                fx.transform.GetChild(0).localScale = Vector3.one * 3.0f;
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.showItem);
            }
        }

      

        public void ShowAllItems(int endIndex)
        {
            for(int i=0;i<=endIndex;i++)
            {
                ShowItems(i, false);
            }
        }

     

    }
}

