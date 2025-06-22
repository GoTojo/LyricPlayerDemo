using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
	// Start is called before the first frame update
	private float targetTime = 0.5f;
	private bool fRotate = true;
	public bool clockwise = true;
	private float totalAngle = 0f;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
	}

	// Update is called once per frame
	void Update()
	{
		Transform transform = this.gameObject.transform;
		if (fRotate) {
			// 1/4小節で300度回す
			float deltaAngle = 300 * Time.deltaTime / targetTime;
			totalAngle += deltaAngle;
			deltaAngle *= clockwise ? 1 : -1;
			transform.Rotate(0f, deltaAngle, 0f);
			float angle = transform.eulerAngles.y;
			if (totalAngle >= 330) {
				if (clockwise) {
					transform.rotation = Quaternion.Euler(0f, 120f, 0f);
				} else {
					transform.rotation = Quaternion.Euler(0f, 220f, 0f);
				}
				fRotate = false;
				totalAngle = 0;
			}
		}
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		// 1/4小節
		transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		targetTime = (float)measureInterval / 4000;
		fRotate = true;
	}
}
