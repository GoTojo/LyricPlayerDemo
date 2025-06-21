using UnityEngine;

public class TwistControl : MonoBehaviour {
	public Material material;
	[SerializeField, Range(0.1f, 1f)]
	public float rand = 1f;
	[SerializeField, Range(0f, 1f)]
	public float timer = 0f;
	[SerializeField, Range(0f, 1f)]
	public float c1 = 0.1f;
	[SerializeField, Range(0f, 0.03f)]
	public float c2 = 0.01f;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		material.SetFloat("rand", rand);
		material.SetFloat("timer", timer);
		material.SetFloat("val2", c1);
		material.SetFloat("val3", c2);
	}
}