using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGQuadController
{
	public float colorS = 0.5f;
	public float colorV = 0.8f;
	public void SetColorS()
	{
		colorS = Random.Range(0.2f, 0.9f);
	}
	private int measureCount = 0;
	private GameObject obj;
	private BGQuadObject quadObject;
	public BGQuadController(GameObject obj)
	{
		this.obj = obj;
		this.quadObject = obj.GetComponent<BGQuadObject>();
		int direction = Random.Range(0, 1);
		this.quadObject.rotateDelta = (direction == 0) ? 0.1f : -0.1f;
		measureCount = 0;
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onTempoIn += TempoIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		SetMyColor();
	}

	void SetMyColor()
	{
		float h = Random.Range(0.2f, 0.9f);
		MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
		Color color = Color.HSVToRGB(h, colorS, colorV);
		color.a = 0.8f;
		// Debug.Log($"Color: {color}");
		renderer.material.color = color;
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		// LyricItem.OnBeat += () => { };
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		measureCount++;
		if (measureCount < 2) {
			SetMyColor();
		} else {
			GameObject.Destroy(obj.GameObject());
			MidiWatcher midiWatcher = MidiWatcher.Instance;
			midiWatcher.onTempoIn -= TempoIn;
			midiWatcher.onBeatIn -= BeatIn;
			midiWatcher.onMeasureIn -= MeasureIn;
		}
	}

}

public class BGQuadObject : MonoBehaviour
{
	public float rotateDelta = -0.1f;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.Rotate(0.0f, 0.0f, rotateDelta);
	}
}