using UnityEngine;

public class DigitalGlitchControl : MonoBehaviour {
	public Material material;
	
	public float amount = 0.08f;
	public float angle = 0.02f;
	public float seed = 0.02f;
	public float seed_x = 0.02f;
	public float seed_y = 0.02f;
	public float distortion_x = 0.5f;
	public float distortion_y = 0.6f;
	public float col_s = 0.05f;
	public bool bypass = false;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetInt("byp", bypass ? 1 : 0);
		material.SetFloat("amount", amount);
		material.SetFloat("angle", angle);
		material.SetFloat("seed", seed);
		material.SetFloat("seed_x", seed_x);
		material.SetFloat("seed_y", seed_y);
		material.SetFloat("distortion_x", distortion_x);
		material.SetFloat("distortion_y", distortion_y);
		material.SetFloat("col_s", col_s);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}