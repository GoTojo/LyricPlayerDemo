using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using System.Linq;
using System;

public class Naruto : MonoBehaviour {
	// Start is called before the first frame update
	private Rect area = new Rect(-11, 4, 22, 2);
	private float interval = 0.5f;
	private float wait = 0f;
	private float lifetime = 2;
	private float life = 0;
	private float speed = 30f;
	private int num = -1;
	private const int numOfNaruto = 4;
	private List<int> order = new List<int>();
	private List<GameObject> naruto = new List<GameObject>();
 
	void Start() {
		for (var i = 0; i < numOfNaruto; i++) {
			order.Add(i);
			naruto.Add(null);
		}
	}

	// Update is called once per frame
	void Update() {
		if (num >= 0) {
			if (num < numOfNaruto) {
				wait -= Time.deltaTime;
				if (wait <= 0) {
					Create();
				}
			}
			life -= Time.deltaTime;
			if (life <= 0) {
				for (var i = 0; i < numOfNaruto; i++) {
					if (naruto[i]) {
						naruto[i].GetComponent<Rigidbody>().useGravity = true;
						Destroy(naruto[i], 2f);
						naruto[i] = null;
						life = interval;
						return;
					}
				}
				num = -1;
			}
			for (var i = 0; i < numOfNaruto; i++) {
				if (naruto[i]) {
					naruto[i].transform.Rotate(0f, 0f, Time.deltaTime * speed);
				}
			}
		}
	}
	void Create() {
		if (num >= numOfNaruto) return;
		GameObject obj = Resources.Load<GameObject>("Prefab/Icons/Naruto");
		float w = area.width / numOfNaruto;
		float x = area.x + w * order[num] + UnityEngine.Random.Range(0f, w);
		float y = UnityEngine.Random.Range(area.yMin, area.yMax);
		naruto[num] = Instantiate(obj, new Vector3(x, y, 8), Quaternion.Euler(0, 0, 0));
		num++;
		wait = interval;
		life = lifetime;
	}
	public void Begin(float lifetime) {
		if (num == -1) {
			num = 0;
			order = order.OrderBy(a => Guid.NewGuid()).ToList();
			this.lifetime = lifetime;
			Create();
		}
	}
}
