using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndDragListener : MonoBehaviour, IEndDragHandler
{
    event Action<PointerEventData> _OnDragEnd;

    public static EndDragListener Get(GameObject go)
    {
        return go.GetOrAddComponent<EndDragListener>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_OnDragEnd != null)
        {
            _OnDragEnd(eventData);
        }
    }

    public void AddHandle(Action<PointerEventData> handler)
    {
        _OnDragEnd += handler;
    }

    public void RemoveHandle(Action<PointerEventData> handler)
    {
        _OnDragEnd -= handler;
    }
}
