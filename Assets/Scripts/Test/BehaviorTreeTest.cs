using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using Foundation;

public class BehaviorTreeTest : MonoBehaviour
{
    private Behavior _behavior;
    // Start is called before the first frame update
    void Start()
    {
        _behavior = GetComponent<Behavior>();
        _behavior.EnableBehavior();
        _behavior.OnBehaviorEnd += BehaviorEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void BehaviorEnded(Behavior behavior)
    {
        behavior.DisableBehavior();
        behavior.OnBehaviorEnd -= BehaviorEnded;
        //_behavior.DisableBehavior();
        Timer.Instance.Register(2, (pare) =>
        {
            behavior.EnableBehavior();
        });
    }
}
