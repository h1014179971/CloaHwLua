using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Delivery.Behavior 
{
    [TaskCategory("Behavior/Common")]
    [TaskDescription("设置到屏幕外")]
    public class OutScreen : Action
    {
        public override void OnStart()
        {
            Owner.transform.position = new Vector3(-1000,-1000,0);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}


