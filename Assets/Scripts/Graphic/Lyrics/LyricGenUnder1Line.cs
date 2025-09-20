/// LyricGenUnder1Line.cs
/// 表示エリア一番下に1Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

public class LyricGenUnder1Line : LyricBase {
	public Vector3 position = new Vector3(0, -6.5f, 0);
	class LyricGenControl : LyricGenBase {
		public TextMeshPro text;
		private int waitCount = 3;
		private int waitClear = 0;
		public LyricGenControl(Vector3 position, TMP_FontAsset font, Transform transform) {
			this.font = font;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float scale = 1f;
			float rotate = 0;
			Vector2 size = new Vector2(20, 2);
			this.active = true;
			GameObject simpleLyric = CreateText("", color, TextAlignmentOptions.Center, size, position, scale, rotate);
			this.text = simpleLyric.GetComponent<TextMeshPro>();
			simpleLyric.transform.SetParent(transform);
		}
		protected override void OnTextChanged(string sentence) {
			text.text = sentence;
			waitClear = waitCount;
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) { }
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					// sentence = "";
					text.text = "";
					// sentenceLength = 0;
				}
			}
		}
		public override void Clear() {
			text.text = "";
		}
		public void SetText(string text) {
			this.text.text = text;
		}
		public void Show() {
			text.enabled = true;
		}
		public void Hide() {
			text.enabled = false;
		}
	};
	LyricGenControl control;

	void Awake() {
		control = new LyricGenControl(position, font, this.transform);
	}
	public override void OnParamChanged() {
		control.active = active;
		if (active) control.Show();
		else control.Hide();
		control.text.transform.position = position;
		control.font = font;
	}
	public override void Clear() {
		control.Clear();
	}
	public override void ShowSampleText(string[] text) {
		control.SetText(text[0]);
	}
	public override void SetPosX(float x) {
		Vector3 pos = control.text.transform.position;
		pos.x = x;
		control.text.transform.position = pos;
	}
	public override void SetPosY(float y) {
		Vector3 pos = control.text.transform.position;
		pos.y = y;
		control.text.transform.position = pos;
	}
	public override float GetPosX() {
		return control.text.transform.position.x;
	}
	public override float GetPosY() {
		return control.text.transform.position.y;
	}
}
