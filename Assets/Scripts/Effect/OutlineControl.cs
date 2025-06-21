using UnityEngine;

public class OutlineControl : MonoBehaviour {
	public Material material;
	//　no properties
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