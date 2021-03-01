using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Examples;

public class ExampleMover : MonoBehaviour
{
    RVOExampleAgent agent;
    public Transform target;
    private void Awake()
    {
        agent = GetComponent<RVOExampleAgent>();
    }
    void Start()
    {
        agent.SetTarget(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
