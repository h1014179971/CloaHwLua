using System;
using UnityEngine;

namespace Foundation 
{
    /// <summary>
    /// 管理器的接口定义
    /// </summary>
    public interface IManager : IUpdateProxy, IDisposable
    {
        /// <summary>
        /// 管理器初始化
        /// </summary>
        void Init();
    }
}

