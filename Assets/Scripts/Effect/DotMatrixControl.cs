using UnityEngine;

public class DotMatrixControl : MonoBehaviour {
	public Material material;
	public float spacing = 10.0f;
	public float size = 4.0f;
	public float blur = 4.0f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("spacing", spacing);
		material.SetFloat("size", size);
		material.SetFloat("blur", blur);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}