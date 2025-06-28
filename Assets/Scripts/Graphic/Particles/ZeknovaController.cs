using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeknovaController : MonoBehaviour {
	public ParticleSystem p1;
	public ParticleSystem p2;
	public ParticleSystem p3;
	public ParticleSystem p4;
	public GameObject background;
	// Start is called before the first frame update
	void Start() {
		Stop();
	}

	// Update is called once per frame
	void Update() {

	}

	public void Play() {
		background.SetActive(true);
		p1.Play();
		p2.Play();
		p3.Play();
		p4.Play();
	}

	public void Stop() {
		background.SetActive(false);
		p1.Stop();
		p2.Stop();
		p3.Stop();
		p4.Stop();
	}
}
