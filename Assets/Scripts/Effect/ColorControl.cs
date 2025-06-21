using UnityEngine;

public class ColorControl : MonoBehaviour {
	public Material material;
	[SerializeField, Range(0.0f, 1.0f)]
	public float riseR = 0.2f;
	[SerializeField, Range(0.0f, 1.0f)]
	public float riseG = 0.2f;
	[SerializeField, Range(0.0f, 1.0f)]
	public float riseB = 0.2f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("riseR", riseR);
		material.SetFloat("riseG", riseG);
		material.SetFloat("riseB", riseB);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}