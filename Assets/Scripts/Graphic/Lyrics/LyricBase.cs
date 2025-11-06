///
/// LyricControl.cs
/// Lyricのエディットやイベントの反映などを行う
/// Copyright (c) 2025 gotojo
/// 
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public abstract class LyricBase : MonoBehaviour
{
	public TMP_FontAsset font;
	public float fontSize = 16;
	public bool active = false;
	void Awake() {
		Lyrics.lyrics.Add(this);
	}
	public void SetActive(bool f) {
		active = f;
		OnParamChanged();
	}
	public virtual bool HasArea() {
		return false;
	}
	public virtual void SetPosX(float x) {
		Vector3 pos = transform.position;
		pos.x = x;
		transform.position = pos;
	}
	public virtual void SetPosY(float y) {
		Vector3 pos = transform.position;
		pos.y = y;
		transform.position = pos;
	}
	public virtual void SetPosW(float w) {
	}
	public virtual void SetPosH(float h) {
	}
	public virtual float GetPosX() {
		return this.transform.position.x;
	}
	public virtual float GetPosY() {
		return this.transform.position.y;
	}
	public virtual float GetPosW() {
		return 0;
	}
	public virtual float GetPosH() {
		return 0;
	}
	public void SetFont(TMP_FontAsset font) {
		this.font = font;
		OnParamChanged();
	}
	public void SetFontSize(float size) {
		this.fontSize = size;
		OnParamChanged();
	}
	public TMP_FontAsset GetFont() {
		return font;
	}
	public virtual void Show() {
		active = true;
		OnParamChanged();
	}
	public virtual void Hide() {
		active = false;
		OnParamChanged();
	}
	public abstract void ShowSampleText(string [] text);
	public abstract void OnParamChanged();
	public abstract void Clear();
}

public class Lyrics {
	static public List<LyricBase> lyrics = new List<LyricBase>();
	static public void Reset() {
		foreach (LyricBase lyric in lyrics) {
			lyric.SetActive(false);
			lyric.Clear();
		}
	}
}