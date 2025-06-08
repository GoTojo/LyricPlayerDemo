using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour {
	private float amplitude = 20f; // 最大角度（度）
	private float frequency =30f; // 初期周波数（回/秒）
	private float damping = 5f;    // 減衰係数（大きいほど早く止まる）
	private float timeElapsed = 0f;
	private bool isShaking = true;
	// Start is called before the first frame update
	void Start() {
		MidiWatcher.Instance.onBeatIn += BeatIn;
	}

	// Update is called once per frame
	void Update() {
		if (isShaking) {
			timeElapsed += Time.deltaTime;

			// 減衰係数を使って振幅を減らす
			float dampedAmplitude = amplitude * Mathf.Exp(-damping * timeElapsed);

			// 周期も時間とともに遅くする（指数関数的に）
			float currentFrequency = frequency * Mathf.Exp(-damping * timeElapsed * 0.5f);

			// 正弦波で回転角を計算
			float angle = Mathf.Sin(timeElapsed * currentFrequency * 2 * Mathf.PI) * dampedAmplitude;

			// Z軸回転（Euler角で指定）
			transform.localRotation = Quaternion.Euler(0f, 0f, angle);

			// ほぼ停止したらゼロにして止める
			if (dampedAmplitude < 0.1f) {
				transform.localRotation = Quaternion.identity;
				isShaking = false;
			}
		}
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		timeElapsed = 0f;
		isShaking = true;
	}
}
