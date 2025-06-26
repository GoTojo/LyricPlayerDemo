using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icons : MonoBehaviour
{
	public const string path = "Prefab/Icons/";
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Create(string name, Vector3 pos, Quaternion angle, float lifetime) {
		GameObject prefab = Resources.Load<GameObject>($"Prefab/Icons/{name}");
		GameObject obj = Instantiate(prefab, pos, angle);
		Destroy(obj, lifetime);
	}
}
