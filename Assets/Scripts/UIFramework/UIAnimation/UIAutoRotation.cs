using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class UIAutoRotation : MonoBehaviour {

    private static float _spinSpeed = 200f;

	// Update is called once per frame
	void Update () {
		if (this.gameObject.activeInHierarchy)
        {
            this.transform.Rotate(new Vector3(0, 0, -_spinSpeed * Time.deltaTime));
        }
	}
}
