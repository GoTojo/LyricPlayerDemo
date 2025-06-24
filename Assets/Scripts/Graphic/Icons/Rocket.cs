using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Rocket : MonoBehaviour {
	// Start is called before the first frame update
	private Vector3 lauchPoint = new Vector3(-10, -5, 0);
	private const float endX = 13;
	private GameObject rocket = null;
	private Vector3 lastPosition;
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (rocket) {
			Vector3 position = rocket.transform.position;
			if (position == lastPosition) return;
			Quaternion rotation = Quaternion.LookRotation(position - lastPosition, Vector3.up);
			rotation.x = 0;
			rotation.y = 0;
			rocket.transform.rotation = rotation;
			lastPosition = position;
			if (position.x >= endX) {
				Destroy(rocket);
				rocket = null;
			}
		}
	}

	public void Launch() {
		if (rocket) return;
		GameObject obj = Resources.Load<GameObject>("Prefab/Icons/Rocket");
		rocket = Instantiate(obj, lauchPoint, Quaternion.Euler(0, 0, 0));
		rocket.GetComponent<Rigidbody>().AddForce(new Vector3(10, 15, 0), ForceMode.Impulse);
		lastPosition = rocket.transform.position;
	}
}
