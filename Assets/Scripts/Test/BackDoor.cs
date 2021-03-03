using Delivery;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackDoor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isStart = false;
    private float _maxt = 5f;
    private float _t = 0;
    private void Awake()
    {
    }
    void Update()
    {
        if (_isStart)
        {
            _t += Time.deltaTime;
            if (_t >= _maxt && _t - _maxt >= 0.02f)
            {
                _t = _maxt;
                PlayerMgr.Instance.AddMoney(new Long2(10000000, 600)); 
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("按下");
        _isStart = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("抬起");
        _isStart = false;
        _t = 0;
    }
}
