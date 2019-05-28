using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animator_finger : MonoBehaviour {
	public Transform master0,master1,master2,slave0,slave1,slave2;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		slave0.transform.rotation = master0.transform.rotation;
		slave1.transform.rotation = master1.transform.rotation;
		slave2.transform.rotation = master2.transform.rotation;

	}
}
