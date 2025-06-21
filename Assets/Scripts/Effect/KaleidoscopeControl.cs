using UnityEngine;

public class KaleidoscopeControl : MonoBehaviour {
	public Material material;
	public float numOfRelections = 6.0f; 
	[SerializeField, Range(0f, 90f)]
	public float angle = 0f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("Sides", numOfRelections);
		material.SetFloat("angle", angle / 90f * 3.14f);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}