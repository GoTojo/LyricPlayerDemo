/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using System;
using UnityEngine.AI;

public class LyricGenMultiLine : MonoBehaviour {
	public Rect area = new Rect(-6, -4, 20, 6);
	public TMP_FontAsset font;
	public float scale = 0.7f;
	public float textHeight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	public SentenceList sentenceList;
	public bool active = true;
	class LyricGenMultiLineControl : LyricGenMultiLineBase {
		private int waitCount = 3;
		private int waitClear = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, Transform transform, SentenceList sentenceList) : base(area, textHeight, textWidth, font, transform, sentenceList) {
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					Clear();
				}
			}
		}
		protected override void OnTextChanged(string sentence) {
			if (line >= maxLine) {
				Clear();
			}
			CreateText(sentence);
			line++;
			waitClear = waitCount;
		}
	};
	LyricGenMultiLineControl control;

	void Start() {
		control = new LyricGenMultiLineControl(area, textHeight, textWidth, font, this.transform, sentenceList);
		control.maxLine = maxLine;
		control.scale = scale;
		control.vertical = vertical;
		control.active = active;
	}

	public void Clear() {
		control.Clear();
	}

	public void SetActive(bool f) {
		control.Clear();
		active = f;
		control.active = f;
		control.vertical = vertical;
	}
}
