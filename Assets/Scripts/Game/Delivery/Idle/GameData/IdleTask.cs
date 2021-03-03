using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{

    public class IdleTaskBase
    {
        public int Id;
        public int CityId;
        public string Desc;
        public string Introduce;
        public int TaskType;
        public int Parameter1;
        public int Parameter2;
        public string Reward;
        public int RewardType;
    }

    public class IdleMainTask: IdleTaskBase
    {
        public string Tips;
    }

    public class IdleTask:IdleTaskBase
    {
        //public int Id;
        //public int CityId;
        //public string Desc;
        //public string Introduce;
        //public int TaskType;
        //public int Parameter1;
        //public int Parameter2;
        //public int Reward;
        //public int RewardType;
    }
}

