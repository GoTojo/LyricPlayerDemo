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
	public  GlitchShaderControl	glitchShader;
	public  BrightnessContrastSaturationControl	brightnessContrastSaturation;
	public  DisplacementControl	displacement;
	public  KaleidoscopeControl	kaleidoscopeImageEffect;
	public  ShakerControl	shaker;
	public  ColorControl	colorControl;
	public  DotMatrixControl	dotMatrix;
	public  OutlineControl	outline;
	public  SlitscanControl	slitscan;
	public  CutSliderControl	cutSlider;
	public  DotScreenControl	dotScreen;
	public  PosterControl	poster;
	public  SquaresControl	squares;
	public  DigitalGlitchControl	digitalGlitch;
	public  FilmControl	film;
	public  RGBShiftController	rgbShift;
	public  TwistControl	twist;
	public float manualCount = 2f;
	private float _manualCount = 0;
	public bool manual = false;
	public void ChangeEffect(string[] args) {
		if (manual) return;
		curType = Enum.Parse<FullScreenEffectFeature.EffectType>(args[1]);
		effectFeature.SetMatrial(curType);
	}
	public void ChangeParameter(int ch, int ccNum, float value) {
		_manualCount = manualCount;
		manual = true;
		switch (ccNum) {
		case Parameter.CCRGBShiftAmount:
			type = FullScreenEffectFeature.EffectType.RGBShift;
			rgbShift.Amount(value);
			break;
		case Parameter.CCRGBShiftAngle:
			type = FullScreenEffectFeature.EffectType.RGBShift;
			rgbShift.Angle(value);
			break;
		}
		effectFeature.SetMatrial(type);
	}
	void Start() {
		curType = effectFeature.GetEffectType();
	}
	void Update() {
		if (_manualCount > 0) {
			_manualCount -= Time.deltaTime;
			if (_manualCount <= 0) {
				manual = false;
				effectFeature.SetMatrial(FullScreenEffectFeature.EffectType.Off);
			}
		}
		if (manual) {
			if (curType != type) {
				curType = type;
				effectFeature.SetMatrial(curType);
			}
		}
	}
}
