using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleGuide
    {
        public int Id;
        public int ConditionKey;
        public string ConditionValue;
        public int NextGuide;
        public string GuideStepIds;
        public bool NeedComplete;
    }
}
