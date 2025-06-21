using UnityEngine;

public class SquaresControl : MonoBehaviour {
	public Material material;
	[SerializeField, Range(0.1f, 1000f)]
	public float scale = 100f;
	[SerializeField, Range(0f, 0.5f)]
	public float size = 0.4f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("scale", scale);
		material.SetFloat("size", size);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}