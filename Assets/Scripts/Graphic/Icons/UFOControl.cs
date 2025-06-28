using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOControl : MonoBehaviour {
	public bool falling = false;

	void OnCollisionEnter(Collision collision) {
		falling = true;
		GetComponent<Rigidbody>().useGravity = true;
	}
}
