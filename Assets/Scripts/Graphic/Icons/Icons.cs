using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icons : MonoBehaviour {
	public const string path = "Prefab/Icons/";
	private float minX = -14;
	private float maxX = 14;
	private float posY = 6;
	private float posZ = 8;
	private struct IconObj {
		public GameObject obj;
		public float deltaX;
	};
	private List<IconObj> icons = new List<IconObj>();
	// Start is called before the first frame update
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (icons.Count > 0) {
			foreach(IconObj icon in icons.ToArray()) {
				Vector3 pos = icon.obj.transform.position;
				pos.x += icon.deltaX * Time.deltaTime;
				icon.obj.transform.position = pos;
				if (pos.x >= maxX) {
					Destroy(icon.obj);
					icons.Remove(icon);
				} 
			}
		}
	}

	public void Create(string name, Vector3 pos, Quaternion angle, float lifetime) {
		GameObject prefab = Resources.Load<GameObject>($"Prefab/Icons/{name}");
		GameObject obj = Instantiate(prefab, pos, angle);
		Destroy(obj, lifetime);
	}

	public void Create(string name, float lifetime) {
		GameObject prefab = Resources.Load<GameObject>($"Prefab/Icons/{name}");
		IconObj obj = new IconObj();
		obj.obj = Instantiate(prefab, new Vector3(minX, posY, posZ), Quaternion.identity);
		obj.deltaX = (maxX - minX) / lifetime;
		icons.Add(obj);
	}
}
