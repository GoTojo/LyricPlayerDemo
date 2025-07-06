using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class SimpleLyricGen : MonoBehaviour {
	public Rect area = new Rect(-10, 0, 20, 2);
	public float sizeMin = 1.2f;
	public float sizeMax = 1.3f;
	public float rotateAngle = 0.0f;
	public TMP_FontAsset font;
	public bool active = false;
	public MidiEventMapAccessor eventMap;
	public SentenceList sentenceList;
	class LyricGenControl : LyricGenBase {
		private Transform transform;
		private Rect area;
		private float sizeMin = 1.2f;
		private float sizeMax = 1.3f;
		private float rotateAngle = 0.0f;
		private TMP_FontAsset font;
		private Vector2 sizeDelta = new Vector2(5, 5);
		private int waitCount = 3;
		private int waitClear = 0;
		public int measureCount = 2;
		private float measureInterval = 2;
		private string curWord = "";
		private int lyricNum = 0;
		private int sentenceLength = 0;

		private List<GameObject> lyrics = new List<GameObject>();


		public LyricGenControl(Rect area, TMP_FontAsset font, float sizeMin, float sizeMax, float rotateAngle, Transform transform, SentenceList sentenceList) : base(sentenceList, SentenceList.kanjiMap, MidiWatcher.Instance) {
			this.area = area;
			this.font = font;
			this.sizeMin = sizeMin;
			this.sizeMax = sizeMax;
			this.rotateAngle = rotateAngle;
			this.transform = transform;
			fontSize = 16;
			autoSizeTextContainer = true;
		}
		private void CreateLyric() {
			if (!active) return;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float width = (sentenceLength != 0) ? area.width / sentenceLength : area.width;
			float x = width * lyricNum + area.x + width / 2;
			float y = UnityEngine.Random.Range(area.yMin, area.yMax);
			Vector3 pos = new Vector3(x, y, -1);
			float scale = UnityEngine.Random.Range(sizeMin, sizeMax);
			float rotate = UnityEngine.Random.Range(-rotateAngle, rotateAngle);
			GameObject lyric = CreateText(curWord, font, color, TextAlignmentOptions.Top, sizeDelta, pos, scale, rotate);
			lyric.transform.SetParent(transform);
			lyrics.Add(lyric);
			lyricNum++;
			waitClear = measureCount;
		}
		public void Clear() {
			foreach (var lyric in lyrics) {
				Destroy(lyric);
			}
			lyrics.Clear();
			lyricNum = 0;
		}
		protected override void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
			byte status = (byte)(midiEvent[0] & 0xF0);
			switch (status) {
			case 0x90:
				if (midiEvent[2] == 0) {
				} else {
					CreateLyric();
				}
				break;
			case 0x80:
				break;
			default:
				break;
			}
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
			curWord = lyric;
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			this.measureInterval = measureInterval / 1000f;
			curMeas = measure;
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					Clear();
				}
			}
		}
		protected override void OnTextChanged(string sentence) {
			Clear();
			sentenceLength = sentence.Length;
		}
	};
	LyricGenControl control;
	void Start() {
		control = new LyricGenControl(area, font, sizeMin, sizeMax, rotateAngle, this.transform, sentenceList);
	}
	public void SetActive(bool f) {
		this.active = f;
		control.active = f;
	}
}
