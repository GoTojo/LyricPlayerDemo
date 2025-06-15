using UnityEngine;

public class RGBShift : MonoBehaviour {
	public Material material;
	public float amount = 0.005f;
	public float angle = 0;
	public float time = 0.1f;
	public bool active = false;
	private float _amount = 0f;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		if (active && _amount >= 0) {
			_amount -= amount * Time.deltaTime / time;
			material.SetFloat("amount", _amount);
			material.SetFloat("angle", angle / 180f * 3.14f);
		} else {
			material.SetFloat("amount", 0);
			material.SetFloat("angle", 0);
		}
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		_amount = amount;
	}
}