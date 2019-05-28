using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animator_thumb : MonoBehaviour {
	public Transform master0,master1,slave1,slave2;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		slave1.transform.rotation = master0.transform.rotation*Quaternion.Euler(100, 300, 270);
		slave2.transform.rotation = master1.transform.rotation*Quaternion.Euler(100, 300, 270);
	}
}
