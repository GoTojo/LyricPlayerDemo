using UnityEngine;

public class BrightnessContrastSaturationControl : MonoBehaviour {
	public Material material;
	public bool active = false;
	[SerializeField, Range(0.0f, 1.0f)]
	public float saturation = 1.0f;
	[SerializeField, Range(0.0f, 1.0f)]
	public float brightness = 1.0f;
	[SerializeField, Range(0.0f, 2.0f)]
	public float contrast = 1.0f;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		material.SetFloat("Saturation Amount", saturation);
		material.SetFloat("Brightness Amount", brightness);
		material.SetFloat("Contrast Amount", contrast);
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
}