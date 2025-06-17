using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class EffectSwitcher : MonoBehaviour {
	public FullScreenEffectFeature effectFeature;

	public void ChangeEffect(string[] args) {
		FullScreenEffectFeature.EffectType type = Enum.Parse<FullScreenEffectFeature.EffectType>(args[1]);
		effectFeature.SetMatrial(type);
	}
}
