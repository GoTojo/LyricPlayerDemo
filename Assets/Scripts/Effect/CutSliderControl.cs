using UnityEngine;

public class CutSliderControl : MonoBehaviour {
	public Material material;
	[SerializeField, Range(1.0f, 255f)]
	public float rand = 1f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("rand", rand);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}