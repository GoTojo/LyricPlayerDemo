using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleLyricBehaviour : MonoBehaviour {
	private int startmeas = 0;
	private int curmeas = 0;
	private int measInterval = 0;
	private float lifetime = 0;
	private int measCount = 1;
	// Start is called before the first frame update
	void Awake() {
		MidiWatcher.Instance.onMeasureIn += MeasureIn;
		MidiWatcher.Instance.onLyricIn += LyricIn;
		MidiWatcher.Instance.onEventIn += EventIn;
	}
	void OnDestroy() {
		MidiWatcher.Instance.onMeasureIn -= MeasureIn;
		MidiWatcher.Instance.onLyricIn -= LyricIn;
		MidiWatcher.Instance.onEventIn -= EventIn;
	}
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (curmeas > startmeas + measCount) {
			Destroy(this.gameObject);
		} else if (lifetime > 0) {
			lifetime -= Time.deltaTime;
			if (lifetime < 0) {
				Destroy(this.gameObject);
			}
		}
	}
	public void SetStartMeas(int meas, int measureCount) {
		startmeas = meas;
		curmeas = meas;
		measCount = measureCount;
	}
	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		if (curmeas > startmeas) {
			Destroy(this.gameObject);
		}
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		curmeas = measure;
		measInterval = measureInterval;
	}
	public void EventIn(MIDIHandler.Event playerEvent) {
		switch (playerEvent) {
		case MIDIHandler.Event.Stop:
		case MIDIHandler.Event.End:
			lifetime = measInterval / 1000f;
			break;
		default:
			break;
		}
	}
}

public class SimpleLyricGen : MonoBehaviour {
	public Rect area = new Rect(-10, 5, 20, 10);
	public float sizeMin = 0.8f;
	public float sizeMax = 0.8f;
	public float rotateAngle = 0.0f;
	public TMP_FontAsset font;
	public bool active = false;
	public int measureCount = 1;
	private string curWord = "";
	private int lyricNum = 0;
	private int curmeas = 0;
	private int measInterval = 4000;
	public MidiEventMapAccessor eventMap;
	void Awake() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}
	void OnDestroy() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
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

	private void LyricObjectText(int ch, int num, int numOfData, float position) {
		Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		float width = (numOfData != 0) ? area.width / numOfData : area.width;
		float x = width * num + area.x + width / 2;
		float y = UnityEngine.Random.Range(area.yMin, area.yMax);

		Vector3 pos = new Vector3(x, y, -1.0f);
		float scale = UnityEngine.Random.Range(sizeMin, sizeMax);
		float rotate = UnityEngine.Random.Range(-rotateAngle, rotateAngle);

		GameObject simpleLyric = CreateText(curWord, font, color, 5.0f, pos, scale, rotate);

		SimpleLyricBehaviour behaviour = simpleLyric.AddComponent<SimpleLyricBehaviour>();
		behaviour.SetStartMeas(curmeas, measureCount);
	}

	private void CreateLyric(int track, float position) {
		if (!active) return;
		LyricObjectText(track, lyricNum, eventMap.GetNumOfLyrics(curmeas, track), position);
		lyricNum++;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		byte status = (byte)(midiEvent[0] & 0xF0);
		int ch = (status & 0xF0) >> 4;
		switch (status) {
		case 0x90:
			if (midiEvent[2] == 0) {
			} else {
				CreateLyric(track, position);
			}
			break;
		case 0x80:
			break;
		default:
			// Debug.Log($"MIDIData Status = {status}");
			break;
		}
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		curWord = lyric;
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		curmeas = measure;
		measInterval = measureInterval;
		lyricNum = 0;
	}
}
