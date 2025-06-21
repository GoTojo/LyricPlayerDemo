using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DotScreenControl : MonoBehaviour {
	public Material material;
	public Vector2 tSize = new Vector2(256, 256);
	public float angle = 1.57f;
	public float scale = 1.0f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetVector("tSize (vec2)", tSize);
		material.SetFloat("angle", angle);
		material.SetFloat("scale", scale);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}