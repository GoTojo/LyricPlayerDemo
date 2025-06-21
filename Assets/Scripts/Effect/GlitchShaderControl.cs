using UnityEngine;

public class GlitchShaderControl : MonoBehaviour {
	public Material material;
	public float intensity = 1;
	public float filterRadius = 1;
	public float flip_up = 1;
	public float flip_down = 1;
	public float displace = 1;
	public float scale = 1;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("Glitch Intensity", intensity);
		material.SetFloat("filterRadius", filterRadius);
		material.SetFloat("flip_up", flip_up);
		material.SetFloat("flip_down", flip_down);
		material.SetFloat("displace", displace);
		material.SetFloat("scale", scale);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}