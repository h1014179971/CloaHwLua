using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

namespace Delivery.Idle 
{
    public class IdleNpcTalkCtrl : MonoSingleton<IdleNpcTalkCtrl>
    {
        private Dictionary<IdleNpcModel, IdleNpcTalkModel> _npcTalkDic = new Dictionary<IdleNpcModel, IdleNpcTalkModel>();
        
        public void CreateNpcTalk(GameObject npc)
        {
            Transform blockerRoot = UIController.Instance.GameRoot;
            IdleNpcModel model = npc.GetComponent<IdleNpcModel>();
            if (model == null) return;
            IdleNpcTalkModel idleNpcTalkModel = null;
            if (_npcTalkDic.ContainsKey(model))
            {
                idleNpcTalkModel = _npcTalkDic[model];
                idleNpcTalkModel.TweenKill();
            }
            else
            {
                GameObject npcTalk = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.npcTalk, true, 1);
                idleNpcTalkModel = npcTalk.GetOrAddComponent<IdleNpcTalkModel>();
                _npcTalkDic[model] = idleNpcTalkModel;
                RectTransform rectTran = npcTalk.GetComponent<RectTransform>();
                rectTran.SetParent(blockerRoot);
                rectTran.localPosition = Vector3.zero;
                rectTran.localScale = Vector3.one;
                idleNpcTalkModel.Init(model);
            }
            
        }
        public void RemoveNpcTalk(IdleNpcModel npcModel)
        {
            if (_npcTalkDic.ContainsKey(npcModel))
                _npcTalkDic.Remove(npcModel);
        }
        public override void Dispose()
        {
            _npcTalkDic.Clear();
        }

    }
}


