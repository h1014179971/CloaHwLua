using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CustomSlider : Slider
{
    [SerializeField]
    private float enableMaxValue;
    public SliderEvent onEnableMaxValueChange { get; set; }

    public CustomSlider()
    {
        onEnableMaxValueChange = new SliderEvent();
    }

  
    public float EnableMaxValue
    {
        get
        {
            return enableMaxValue;
        }
        set
        {
            enableMaxValue = value;
            onEnableMaxValueChange.Invoke(value);
        }
    }

    protected override void Set(float input, bool sendCallback)
    {
        if (input >= enableMaxValue)
            input = enableMaxValue;
        base.Set(input, sendCallback);
    }

}
