using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UFO : MonoBehaviour {
	// Start is called before the first frame update
	private GameObject ufo = null;
	private float speed = 0.05f;
	private float cycle = 0.3f;
	private float posY = 6;
	private float xMin = -14;
	private float xMax = 14;
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (ufo) {
			Vector3 pos = ufo.transform.position;
			pos.x += speed * Time.deltaTime;
			pos.y = posY + Mathf.Sin(pos.x) * cycle;
			ufo.transform.position = pos;
			if (pos.x > xMax) {
				Destroy(ufo);
				ufo = null;
			}
		}
	}
	public void Create(float lifetime) {
		if (ufo) return;
		GameObject obj = Resources.Load<GameObject>("Prefab/Icons/UFO");
		speed = (xMax - xMin) / lifetime;
		ufo = Instantiate(obj, new Vector3(xMin, posY, 0), Quaternion.Euler(0, 0, 0));
	}
}
