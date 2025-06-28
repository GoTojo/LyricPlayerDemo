using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SunController : MonoBehaviour {
	// Start is called before the first frame update
	private float orgZoom = 3f;
	private float pastTime = 1.0f;
	public GameObject sun;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	// Update is called once per frame
	void Update() {
		if (pastTime < 1.0f) {
			pastTime += Time.deltaTime;
			float zoom = pastTime >= 1.0f ? 1.0f : Mathf.Lerp(3.5f, orgZoom, pastTime * 10f);
			sun.transform.localScale = new Vector3(zoom, zoom, 0.01f);
		}
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		pastTime = 0;
		sun.transform.Rotate(0.0f, 0.0f, 22.5f);
	}

	public void SetCommand(string command) {
		switch (command) {
		case "8":
			sun.SetActive(true);
			sun.transform.position = new Vector3(-10f, 5f, 9);
			break;
		case "12":
			sun.SetActive(true);
			sun.transform.position = new Vector3(0f, 5f, 9);
			break;
		case "15":
			sun.SetActive(true);
			sun.transform.position = new Vector3(6f, 5f, 9);
			break;
		case "Off":
			sun.SetActive(false);
			break;
		default:
			break;
		}
	}
}
