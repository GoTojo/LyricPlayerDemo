using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateContentsZ : MonoBehaviour {
	private float rotationTime = 8000; // 4measure at BPM120
	// Start is called before the first frame update
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
	}

	// Update is called once per frame
	void Update() {
		float deltaAngle = 360 * Time.deltaTime / rotationTime;
		this.transform.Rotate(0f, 0f, -deltaAngle);
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		// 4小節
		rotationTime = (float)measureInterval * 4 / 1000 ;
	}
}
