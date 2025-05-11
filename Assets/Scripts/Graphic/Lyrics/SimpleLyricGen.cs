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
	void Start()
	{
		MidiWatcher.Instance.onMeasureIn += MeasureIn;
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

	public void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
		curmeas = measure;
	}
}

public class SimpleLyricGen : MonoBehaviour
{
	private Rect area = new Rect(-10, 5, 20, 10);
	private string curWord = "";
	private int lyricNum = 0;
	private int curmeas = 0;
	private int measInterval = 4000;

	public SimpleLyricGen()
	{
 		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}

	private void LyricObjectText(int ch, int num, int numOfData, float position)
	{
		GameObject newObject = new GameObject("SimpleLyric");
		newObject.AddComponent<TextMeshPro>();
		SimpleLyricBehaviour behaviour = newObject.AddComponent<SimpleLyricBehaviour>();
		behaviour.SetStartMeas(curmeas, measInterval);
		TextMeshPro text = newObject.GetComponent<TextMeshPro>();
		text.font = Resources.Load<TMP_FontAsset>("Fonts/JK-Maru-Gothic-M SDF");;
		text.text = curWord;
		switch (ch) {
			default:
			case 1:
				text.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
				break;
			case 2:
				text.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
				break;
			case 3:
				text.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
				break;
			case 4:
				text.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
				break;
		}
		text.fontSize = 16;
		text.fontSizeMax = 20;
		text.fontSizeMin = 12;
		text.autoSizeTextContainer = true;
		float x = (numOfData != 0) ? (area.width / numOfData) * num + area.x + 1 : area.width * position;
		float areaY = area.height * UnityEngine.Random.Range(0.1f, 0.8f) / 2;
		float y = (curmeas % 2 != 0) ? areaY + 1.0f : areaY - area.y / 2 - 1.0f; 
		float size = UnityEngine.Random.Range(0.5f, 1.2f);
		Transform transform = text.GetComponent<Transform>();
		RectTransform rectTransform = newObject.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(5.0f, 5.0f);
		transform.position = new Vector3(x, y, 0.0f);
		transform.Rotate(0.0f, 0.0f, UnityEngine.Random.Range(-70.0f, 70.0f));
		transform.localScale = new Vector3(size, size, size);
	}

	private void CreateLyric(int id, float position)
	{
		LyricObjectText(id, lyricNum, MidiEventMapAccessor.Instance.GetNumOfLyrics(curmeas), position);
		lyricNum++;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, UInt32 currentMsec)
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

	public void LyricIn(int track, string lyric, float position, UInt32 currentMsec)
	{
		curWord = lyric;
	}

	public void BeatIn(int numerator, int denominator, UInt32 currentMsec)
	{
	}

	public void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
		curmeas = measure;
		measInterval = measureInterval;
		lyricNum = 0;
	}
}
