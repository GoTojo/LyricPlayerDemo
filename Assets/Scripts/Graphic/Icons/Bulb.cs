using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Bulb : MonoBehaviour {
	private GameObject bulb = null;
	private const float animtime = 0.08f;
	private float totalAngle = 0;
	private const float lifetime = 1f;
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

	public void Create() {
		GameObject obj = Resources.Load<GameObject>("Prefab/Icons/Bulb");
		bulb = Instantiate(obj, new Vector3(1, 5.5f, 0), Quaternion.Euler(0, 0, 30));
		age = lifetime;
		totalAngle = 0;
	}
}
