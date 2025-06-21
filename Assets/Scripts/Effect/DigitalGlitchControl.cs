using UnityEngine;

public class DigitalGlitchControl : MonoBehaviour {
	public Material material;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}