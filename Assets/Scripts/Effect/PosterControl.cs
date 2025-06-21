using UnityEngine;

public class PosterControl : MonoBehaviour {
	public Material material;
	public float step = 5;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("step", step);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}