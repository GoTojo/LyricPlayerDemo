using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateContentsZ : MonoBehaviour {
	public enum Type {
		Normal,
		HalfAndStop
	}
	public Type type = Type.Normal;
	private float rotationTime = 8; // 4measure at BPM120
									// Start is called before the first frame update
	private int manualCount = 0;
	private int curMeas = 0;
	private int startMeas = 0;
	private float curAngle = 0;
	private int phase = 0;
	private bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
		Vector3 angle = this.transform.eulerAngles;
		angle.z = 0;
		this.transform.eulerAngles = angle;
	}

	// Update is called once per frame
	void Update() {
		if (type == Type.Normal) {
			float deltaAngle = 360 * Time.deltaTime / rotationTime;
			this.transform.Rotate(0f, 0f, -deltaAngle);
		} else if (type == Type.HalfAndStop) {
			float deltaAngle = 360 * Time.deltaTime / (rotationTime / 2);
			if (phase < 2) {
				this.transform.Rotate(0f, 0f, -deltaAngle);
				curAngle += deltaAngle;
			} else {
				Vector3 angle = this.transform.eulerAngles;
				if (curAngle < 360) {
					this.transform.Rotate(0f, 0f, -deltaAngle);
					curAngle += deltaAngle;
				} else if (angle.z != 0) {
					angle.z = 0;
					this.transform.eulerAngles = angle;
				}
			}
		}
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		// 4小節で一周
		if (curMeas + 1 < measure) {
			startMeas = measure - 1;
			if (startMeas < 0) {
				startMeas = 0;
			}
		}
		curMeas = measure;
		if (manualCount > 0) {
			manualCount--;
		} else {
			rotationTime = (float)measureInterval * 4 / 1000;
		}
		phase = (curMeas - startMeas) % 4;
		if (phase == 0) curAngle = 0;
	}
	public void ChangeRotationTime(float f) {
		float t = 1 - f;
		if (t <= 0) return;
		rotationTime = t;
		manualCount = 2;
	}
	public void SetActive(bool f) {
		this.active = true;
	}
}
