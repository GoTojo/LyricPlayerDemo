using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Bulb : MonoBehaviour {
	private GameObject bulb = null;
	private const float animtime = 0.03f;
	private float totalAngle = 0;
	private float age = 0f;
	// Start is called before the first frame update
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (bulb) {
			age -= Time.deltaTime;
			if (age <= 0) {
				Destroy(bulb);
				bulb = null;
			}
			if (totalAngle < 60) {
				float angle = 60 / animtime * Time.deltaTime;
				bulb.transform.Rotate(0f, 0f, -angle);
				totalAngle += angle;
			}
		}

	}

	public void Create(Vector3 pos, float lifetime) {
		GameObject obj = Resources.Load<GameObject>("Prefab/Icons/Bulb");
		bulb = Instantiate(obj, pos, Quaternion.Euler(0, 0, 30), this.transform);
		age = lifetime;
		totalAngle = 0;
	}
}
