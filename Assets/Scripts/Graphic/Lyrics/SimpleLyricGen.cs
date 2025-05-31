using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleLyricBehaviour : MonoBehaviour
{
	private int startmeas = 0;
	private int curmeas = 0;
	private int lifetime = 0;
	// Start is called before the first frame update
	void Awake()
	{
		MidiWatcher.Instance.onMeasureIn += MeasureIn;
	}
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (curmeas > startmeas + 1) {
			Destroy(this.gameObject);
		} else if (lifetime > 0) {
			lifetime -= (int)(Time.deltaTime * 1000);
			if (lifetime < 0) {
				Destroy(this.gameObject);
			}
		}
	}

	public void SetStartMeas(int meas, int measInterval)
	{
		startmeas = meas;
		curmeas = meas;
		lifetime = measInterval * 2;
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		curmeas = measure;
	}
}

public class SimpleLyricGen : MonoBehaviour
{
	public Rect area = new Rect(-10, 5, 20, 10);
	public float sizeMin = 0.8f;
	public float sizeMax = 0.8f; 
	public float yMin = 1.0f;
	public float yMax = 1.0f;
	public float rotateAngle = 0.0f;
	// public Rect area = new Rect(-10, 5, 20, 10);
	// public float sizeMin = 0.5f;
	// public float sizeMax = 1.2f; 
	// public float yMin = 0.1f;
	// public float yMax = 0.8f;
	// public float rotateAngle = 70.0f;
	private string curWord = "";
	private int lyricNum = 0;
	private int curmeas = 0;
	private int measInterval = 4000;
	private string [,] sentence;
	private int curArea = 0;
	private List<Track> [] tracks;
	private MidiEventMapAccessor eventMap;
	void Awake()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}
	void Oestroy()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
	}
	public void Init()
	{
		GameObject mainObj = GameObject.Find("MainGameObject");
		LyricList[] maps = mainObj.GetComponents<LyricList>();
		eventMap = mainObj.GetComponent<MidiEventMapAccessor>();
		const int numOfMap = MidiEventMapAccessor.numOfEventMap;
		tracks = new List<Track>[numOfMap];
		int numOfTrack = 0;
		for (var i = 0; i < numOfMap; i++) {
			tracks[i] = maps[i].tracks;
			if (numOfTrack < tracks.Length) {
				numOfTrack = tracks.Length;
			}
		}
		sentence = new string[numOfMap, numOfTrack];
	}

	private GameObject CreateText(string word, TMP_FontAsset font, Color color, float size, Vector3 position, float scale, float rotate)
	{
		GameObject	simpleLyric = new GameObject("SimpleLyric");
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
		RectTransform rectTransform =	simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(size, size);
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
	}

	private void LyricObjectText(int ch, int num, int numOfData, float position)
	{
		TMP_FontAsset font = FontResource.Instance.GetFont();
		Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		float width = (numOfData != 0) ? area.width / numOfData : area.width;
		float x = width * num + area.x + width / 2;
		float yOffset = yMax / 2;
		if (curArea % 2 != 0) yOffset *= -1;
		float y = UnityEngine.Random.Range(yMin, yMax / 2) + yOffset;
		  
		Vector3 pos = new Vector3(x, y, -1.0f);
		float scale = UnityEngine.Random.Range(sizeMin, sizeMax);
		float rotate = UnityEngine.Random.Range(-rotateAngle, rotateAngle);

		GameObject simpleLyric = CreateText(curWord, font, color, 5.0f, pos, scale, rotate);

		SimpleLyricBehaviour behaviour = simpleLyric.AddComponent<SimpleLyricBehaviour>();
		behaviour.SetStartMeas(curmeas, measInterval);
	}

	private void CreateLyric(int track, float position)
	{
		if (lyricNum == 0) {
			if (track >= 0 && track < sentence.GetLength(1)) {
				for (var map = 0; map < sentence.GetLength(0); map++) {
					sentence[map, track] = tracks[map][track].lyrics[curmeas].sentence;
					if (eventMap.currentMap == map) curArea++;
					// Debug.Log($"{curArea} {sentence[map, track]}");
				}
			}
		}
		LyricObjectText(track, lyricNum, eventMap.GetNumOfLyrics(curmeas, track), position);
		lyricNum++;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec)
	{
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

	public void LyricIn(int track, string lyric, float position, uint currentMsec)
	{
		curWord = lyric;
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		curmeas = measure;
		measInterval = measureInterval;
		lyricNum = 0;
	}
}
