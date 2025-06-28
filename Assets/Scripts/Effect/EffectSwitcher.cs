using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class EffectSwitcher : MonoBehaviour {
	public UIPanelControl uiPanelControl;
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
	private float manualLifeTime = 0;
	private float pulseEffectTime = 0;
	public bool manual = false;
	public void ChangeEffect(string[] args, float beatInterval) {
		if (manual) return;
		float time = 0;
		if (args.Length <= 2) {
			return;
		} else if (args.Length >= 3) {
			switch (args[2]) {
			case "HalfBeat":
				time = beatInterval / 2;
				break;
			case "QuaterBeat":
				time = beatInterval / 4;
				break;
			default:
				break;
			}
		}
		if (time > 0) {
			pulseEffectTime = time;
			effectFeature.SetMatrial(Enum.Parse<FullScreenEffectFeature.EffectType>(args[1]));
		} else {
			curType = Enum.Parse<FullScreenEffectFeature.EffectType>(args[1]);
			effectFeature.SetMatrial(curType);
		}
	}
	public string ChangeParameter(int ch, int ccNum, float value, float lifetime) {
		manual = true;
		manualLifeTime = lifetime;
		string paramName = "";
		switch (ccNum) {
		case Parameter.CCRGBShiftAmount:
			paramName = "RGBShift: Amount";
			type = FullScreenEffectFeature.EffectType.RGBShift;
			rgbShift.Amount(value);
			break;
		case Parameter.CCRGBShiftAngle:
			paramName = "RGBShift: Angle";
			type = FullScreenEffectFeature.EffectType.RGBShift;
			rgbShift.Angle(value);
			break;
		case Parameter.CCEffectSelect:
			FullScreenEffectFeature.EffectType[] typeTbl = new FullScreenEffectFeature.EffectType[] {
				FullScreenEffectFeature.EffectType.GlitchShader,
				// FullScreenEffectFeature.EffectType.BrightnessContrastSaturation,
				FullScreenEffectFeature.EffectType.Displacement,
				FullScreenEffectFeature.EffectType.Kaleidoscope,
				FullScreenEffectFeature.EffectType.Shaker,
				// FullScreenEffectFeature.EffectType.ColorControl,
				FullScreenEffectFeature.EffectType.DotMatrix,
				FullScreenEffectFeature.EffectType.Outline,
				FullScreenEffectFeature.EffectType.Slitscan,
				FullScreenEffectFeature.EffectType.CutSlider,
				FullScreenEffectFeature.EffectType.DotScreen,
				FullScreenEffectFeature.EffectType.Poster,
				FullScreenEffectFeature.EffectType.Squares,
				FullScreenEffectFeature.EffectType.DigitalGlitch,
				FullScreenEffectFeature.EffectType.Film,
				// FullScreenEffectFeature.EffectType.RGBShift,
				FullScreenEffectFeature.EffectType.Twist,
			};
			type = typeTbl[(int)(value * (typeTbl.Length - 1))];
			paramName = type.ToString();
			break;
		}
		effectFeature.SetMatrial(type);
		return paramName;
	}
	void Start() {
		curType = FullScreenEffectFeature.EffectType.Off;
		effectFeature.SetMatrial(curType);
	}
	void Update() {
		if (manualLifeTime > 0) {
			manualLifeTime -= Time.deltaTime;
			if (manualLifeTime <= 0) {
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
		if (pulseEffectTime > 0) {
			pulseEffectTime -= Time.deltaTime;
			if (pulseEffectTime <= 0) {
				effectFeature.SetMatrial(curType);
			}
		}
	}
}
