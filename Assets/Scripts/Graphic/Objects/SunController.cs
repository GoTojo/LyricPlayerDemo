using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {
	// Start is called before the first frame update
	private float orgZoom = 3f;
	private float pastTime = 1.0f;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	// Update is called once per frame
	void Update() {
		if (pastTime < 1.0f) {
			pastTime += Time.deltaTime;
			float zoom = pastTime >= 1.0f ? 1.0f : Mathf.Lerp(3.5f, orgZoom, pastTime * 10f);
			gameObject.transform.localScale = new Vector3(zoom, zoom, 0.01f);
		}
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		pastTime = 0;
		this.transform.Rotate(0.0f, 0.0f, 22.5f);
	}
}
