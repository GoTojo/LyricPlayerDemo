using System;
using UnityEngine;

public class BeatReactiveMaterial : MonoBehaviour
{
	public Material material;
	private float beatValue = 0f;
	void Awake()
	{
		MidiWatcher.Instance.onBeatIn += BeatIn;
	}
	void Start()
	{
	}
	void Update()
	{
		// フェードアウト
		beatValue = Mathf.Lerp(beatValue, 0f, Time.deltaTime * 3f);
		material.SetFloat("_BeatIntensity", beatValue);
	}

	// 外部から呼ばれる
	public void OnBeat()
	{
	}
	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		beatValue = 1.0f;  // ビートのタイミングで上げる
	}
}