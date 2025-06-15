using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class UnityChanShift : MonoBehaviour {
	public float width;
	// Start is called before the first frame update
	private Transform unityChan;
	private Transform ramen;
	private Transform light;
	public bool flipHorizontally = false;
	void Start() {
		MidiWatcher.Instance.onBeatIn += BeatIn;
		MidiWatcher.Instance.onMeasureIn += MeasureIn;
		unityChan = this.transform.Find("Unity-Chan");
		ramen = this.transform.Find("ramen");
		light = this.transform.Find("Light");
		locate();
	}

	private void locate() {
		float grid = width / 4;
		float leftx = grid * 1;
		float rightx = grid * 3;
		if (flipHorizontally) {
			unityChan.localPosition = new Vector3(leftx, -1.2f, 0);
			ramen.localPosition = new Vector3(rightx, 0, 0);
			light.localPosition = new Vector3(rightx, 0, -7);
		} else {
			unityChan.localPosition = new Vector3(rightx, -1.2f, 0);
			ramen.localPosition = new Vector3(leftx, 0, 0);
			light.localPosition = new Vector3(leftx, 0, -7);
		}
	}

	// Update is called once per frame
	void Update() {
		locate();
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		if ((numerator % 2) == 0) return;
		flipHorizontally = !flipHorizontally;
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		// flipHorizontally = !flipHorizontally;
	}
}
