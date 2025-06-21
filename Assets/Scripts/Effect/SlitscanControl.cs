using UnityEngine;

public class SlitscanControl : MonoBehaviour {
	public Material material;
	public float slit_h = 0.008f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("val3", slit_h);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}