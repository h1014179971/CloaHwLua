using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation 
{
    /// <summary>
    /// 可以用于游戏内更新的Proxy
    /// </summary>
    public interface IUpdateProxy
    {
        float TimeSinceUpdate { get; set; }
        void Update(float deltaTime);
    }
}


