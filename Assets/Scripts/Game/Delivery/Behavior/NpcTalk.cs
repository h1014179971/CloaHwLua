using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Foundation;
using Spine.Unity;
using Delivery.Idle;
namespace Delivery.Behavior
{
    [TaskCategory("Behavior")]
    [TaskDescription("说话")]
    public class NpcTalk : Action
    {
        public override void OnStart()
        {
            IdleNpcTalkCtrl.Instance.CreateNpcTalk(Owner.gameObject);
        }

        public override TaskStatus OnUpdate()
        {
            
            return TaskStatus.Success;
        }
    }
}


