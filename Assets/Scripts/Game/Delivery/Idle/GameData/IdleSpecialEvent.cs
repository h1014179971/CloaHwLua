using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSpecialEvent
{
    public int id;
    public string desc;
    public int stayTime;
    public string btnRes;
    public string spriteRes;

    public IdleSpecialEvent()
    {

    }
    public IdleSpecialEvent(IdleSpecialEvent specialEvent)
    {
        id = specialEvent.id;
        desc = specialEvent.desc;
        stayTime = specialEvent.stayTime;
        btnRes = specialEvent.btnRes;
        spriteRes = specialEvent.spriteRes;
    }
}
