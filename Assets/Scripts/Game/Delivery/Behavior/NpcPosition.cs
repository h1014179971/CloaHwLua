using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Delivery.Behavior
{
    [TaskCategory("Behavior/Common")]
    [TaskDescription("Npc位置")]
    public class NpcPosition : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("位置")]
        public Transform _target;
        public override void OnStart()
        {
            Owner.transform.position = _target.position;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}

