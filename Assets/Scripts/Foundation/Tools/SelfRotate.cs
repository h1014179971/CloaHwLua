using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour 
{
    public Vector3 axis = Vector3.forward;
    public float speed = 180f;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
        transform.Rotate(axis, Time.deltaTime * speed, Space.Self);
	}
}
