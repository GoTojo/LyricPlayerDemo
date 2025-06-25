using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingStar : MonoBehaviour {
	// Start is called before the first frame update
	private Rect area = new Rect(-14, -8, 28, 16);
	private int numOfItem = 0;
	private int num = -1;
	private float interval = 0;
	private float wait = 0;
	private List<GameObject> items = new List<GameObject>();
	private const float angle = 45f / 180f * Mathf.PI;
	private const float speed = 7f;
	private const float rotationSpeed = 100f;
	private bool fRotate;
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (num >= 0) {
			if (num < numOfItem) {
				wait -= Time.deltaTime;
				if (wait <= 0) {
					Create();
				}
			}
			float deltaX = Time.deltaTime * speed * -1;
			float deltaY = Mathf.Tan(angle) * deltaX;
			int active = 0;
			for (var i = 0; i < items.Count; i++) {
				Transform transform = items[i].transform;
				if (transform.position.x < area.xMin || transform.position.y < area.yMin) {
					continue;
				}
				transform.position = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0);
				if (fRotate) {
					transform.Rotate(0f, 0f, Time.deltaTime * rotationSpeed);
				}
	 			active++;
			}
			if (active == 0) {
				foreach (GameObject item in items) {
					Destroy(item);
				}
				items.Clear();
				num = -1;
			}
		}
	}
	void Create() {
		if (num >= numOfItem || num < 0) return;
		GameObject obj = Resources.Load<GameObject>(name);
		float xOffset = area.width * 0.3f;
		float w = area.width / numOfItem;
		float x = area.x + xOffset + w * num + w / 2;
		float y = area.yMax;
		Debug.Log($"{x}, {y}");
		items.Add(Instantiate(obj, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0)));
		num++;
		wait = interval;
	}
	public void Trigger(string name, int num, float interval, bool fRotate) {
		if (this.num >= 0) return;
		this.name = $"Prefab/Icons/{name}";
		this.num = 0;
		this.numOfItem = num;
		this.interval = interval;
		this.fRotate = fRotate;
		Create();
	}
}
