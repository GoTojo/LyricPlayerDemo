using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleLyric : LyricGenBase {
	public bool active = false;
	private SimpleLyricGen simpleLyricGen;
	public SimpleLyric(SimpleLyricGen simpleLyricGen) : base(simpleLyricGen.sentenceList, MidiWatcher.Instance) {
		this.simpleLyricGen = simpleLyricGen;
	}
	protected override void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		simpleLyricGen.OnMIDIIn(track, midiEvent, position, currentMsec);
	}
	protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
		simpleLyricGen.OnLyricIn(track, lyric, position, currentMsec);
	}
	protected override void OnBeatIn(int numerator, int denominator, uint currentMsec) {
		simpleLyricGen.OnBeatIn(numerator, denominator, currentMsec);
	}
	protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
		simpleLyricGen.OnMeasureIn(measure, measureInterval, currentMsec);
	}
	protected override void OnTextChanged(string sentence) {
		simpleLyricGen.OnSentenceChanged(sentence);
	}
}

public class SimpleLyricGen : MonoBehaviour {
	public Rect area = new Rect(-10, 0, 20, 2);
	public float sizeMin = 1.2f;
	public float sizeMax = 1.3f;
	public float rotateAngle = 0.0f;
	public TMP_FontAsset font;
	public bool active = false;
	private int measureCount = 2;
	private string curWord = "";
	private int lyricNum = 0;
	private int sentenceLength = 0;
	private int curmeas = 0;
	private int measInterval = 4000;
	private int waitClear = 0;
	public MidiEventMapAccessor eventMap;
	public SentenceList sentenceList;
	private SimpleLyric simpleLyric;
	private String sentence = "";
	private List<GameObject> lyrics = new List<GameObject>();

	void Start() {
		simpleLyric = new SimpleLyric(this);
		simpleLyric.active = this.active;
	}

	void Update() {
		simpleLyric.active = this.active;
	}

	private GameObject CreateText(string word, TMP_FontAsset font, Color color, float size, Vector3 position, float scale, float rotate) {
		GameObject simpleLyric = new GameObject("SimpleLyric");
		simpleLyric.AddComponent<TextMeshPro>();
		TextMeshPro text = simpleLyric.GetComponent<TextMeshPro>();
		text.font = font;
		text.text = word;
		text.color = color;
		text.fontSize = 16;
		text.fontSizeMax = 20;
		text.fontSizeMin = 12;
		text.autoSizeTextContainer = true;
		Transform transform = text.GetComponent<Transform>();
		RectTransform rectTransform = simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(size, size);
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
	}

	private void LyricObjectText(int num, int numOfData) {
		if (!active) return;
		Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		float width = (numOfData != 0) ? area.width / numOfData : area.width;
		float x = width * num + area.x + width / 2;
		float y = UnityEngine.Random.Range(area.yMin, area.yMax);

		Vector3 pos = new Vector3(x, y, -1.0f);
		float scale = UnityEngine.Random.Range(sizeMin, sizeMax);
		float rotate = UnityEngine.Random.Range(-rotateAngle, rotateAngle);
		GameObject simpleLyric = CreateText(curWord, font, color, 5.0f, pos, scale, rotate);
		lyrics.Add(simpleLyric);
	}
	private void UpdateSentence(String text) {
		ClearSentence();
		lyricNum = 0;
		sentence = text;
		// Debug.Log($"sentence: {sentence}");
		sentenceLength = sentence.Length;
	}
	private void CreateLyric() {
		if (sentence.Length == 0) {
			// Debug.Log($"CreateLyric: {simpleLyric.GetSentence()}");
			UpdateSentence(simpleLyric.GetSentence());
		}
		// Debug.Log($"{lyricNum}/{sentenceLength}: {curWord}");
		LyricObjectText(lyricNum, sentenceLength);
		if (sentence.Length >= curWord.Length) {
			sentence = sentence.Substring(curWord.Length);
			lyricNum += curWord.Length;
		} else {
			sentence = "";
			sentenceLength = 0;
			lyricNum = 0;
		}
		waitClear = measureCount;
	}

	public void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
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
			// Debug.Log($"MIDIData Status = {status}");
			break;
		}
	}

	public void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
		curWord = lyric;
	}

	public void OnBeatIn(int numerator, int denominator, uint currentMsec) {
	}

	public void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
		curmeas = measure;
		measInterval = measureInterval;
		if (sentence.Length == 0) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					ClearSentence();
				}
			} 
		}
	}
	private void ClearSentence() {
		for (var i = 0; i < lyrics.Count; i++) {
			Destroy(lyrics[i]);
		}
		lyrics.Clear();
		waitClear = 0;
	}
	public void OnSentenceChanged(String sentence) {
		// if (string.IsNullOrEmpty(this.sentence)) {
			
			// Debug.Log($"OnSentenceChanged: {sentence}");
			UpdateSentence(sentence);
		// }
	}
}
