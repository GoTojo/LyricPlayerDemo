using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class EffectSwitcher : MonoBehaviour {
	public FullScreenEffectFeature effectFeature;
	public FullScreenEffectFeature.EffectType type;
	private FullScreenEffectFeature.EffectType curType;
	public bool manual = false;
	public void ChangeEffect(string[] args) {
		if (manual) return;
		curType = Enum.Parse<FullScreenEffectFeature.EffectType>(args[1]);
		effectFeature.SetMatrial(curType);
	}
	void Start() {
		curType = effectFeature.GetEffectType();
	}
	void Update() {
		if (manual) {
			if (curType != type) {
				curType = type;
				effectFeature.SetMatrial(curType);
			}
		}
	}
}
