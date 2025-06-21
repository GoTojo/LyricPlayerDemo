using UnityEngine;

public class FilmControl : MonoBehaviour {
	public Material material;
	public float time = 0.0f;
	public float nIntensity = 0.5f;
	public float sIntensity = 0.05f;
	public float sCount = 4096f;
	public bool grayscale = true;
	public bool active = false;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("time", time);
		material.SetFloat("nIntensity", nIntensity);
		material.SetFloat("sIntensity", sIntensity);
		material.SetFloat("sCount", sCount);
		material.SetInt("grayscale", grayscale ? 1 : 0);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}