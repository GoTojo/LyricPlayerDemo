using UnityEngine;

public class ShakerControl : MonoBehaviour {
	public Material material;
	public float vec_x = 1f;
	public float vec_y = 1f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("blur_vec_x", vec_x);
		material.SetFloat("blur_vec_y", vec_y);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}