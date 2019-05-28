using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour {

	void OnCollisionEnter (Collision coll) {
		Debug.Log("Initial Collision");
	}
	void OnCollisionStay (Collision coll) {
		Debug.Log("Objects are Touching...");
	}
	void OnCollisionExit (Collision coll) {
		Debug.Log("Objects not touching");
	}
}
