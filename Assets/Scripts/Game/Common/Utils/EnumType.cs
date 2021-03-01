using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    [System.Serializable]
    public enum TrackGameState
    {
        None,
        Began,
        Pause,
        Win,
        Lose
    }

    [System.Serializable]
    public enum ItemType
    {
        None,
        WareHouse,//仓库
        Site, //站点
        ChangeSite,//换乘站点
    }
    [System.Serializable]
    public enum ColorType
    {
        None,
        Red,
        Yellow,
        Blue,
        Green,
        Purple

    }
    [System.Serializable]
    public enum StageLineType
    {
        None,
        Middle,
        End
    }
    [System.Serializable]
    public enum GameState
    {
        None,
        Start
    }


}


