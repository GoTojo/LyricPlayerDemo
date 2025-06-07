using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandCircle : MonoBehaviour {
	public Material material;
	public float expandTime = 1f;
	private float timer = 0f;

	void Update() {
		timer += Time.deltaTime;
		float radius = Mathf.Lerp(0f, 2, timer / expandTime);
		material.SetFloat("_Radius", radius);
	}
}
