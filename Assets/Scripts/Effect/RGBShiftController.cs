using UnityEngine;

public class RGBShiftController : MonoBehaviour {
	public Material material;
	public float amount = 0.005f;
	[SerializeField, Range(0f, 180f)]
	public float angle = 0f;
	public float time = 0.1f;
	public float manualCount = 2;
	public bool active = false;
	private float _amount = 0f;
	private float _manualCount = 0f;
	void Start() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
	}
	void Update() {
		if (active && _amount >= 0) {
			if (_manualCount > 0) {
				_manualCount -= Time.deltaTime;
			} else {
				_amount -= amount * Time.deltaTime / time;
			}
			material.SetFloat("amount", _amount);
			material.SetFloat("angle", angle / 180f * 3.14f);
		} else {
			material.SetFloat("amount", 0);
		}
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		if (_manualCount > 0) return;
		_amount = amount;
	}
	public void Angle(float value) {
		angle = value * 180f;
	}
	public void Amount(float value) {
		_amount = value / 10f;
		_manualCount = manualCount;
	}
}