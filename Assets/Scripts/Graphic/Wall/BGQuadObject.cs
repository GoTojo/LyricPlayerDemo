using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
	public GameObject gameObject;
	public bool fDestroy = false;
	private BGQuadObject quadObject;
	private float zoom = 1.0f;
	private float orgScale = 1.0f;
	public BGQuadController(GameObject obj)
	{
		gameObject = obj;
		quadObject = obj.GetComponent<BGQuadObject>();
		int direction = Random.Range(0, 1);
		quadObject.rotateDelta = (direction == 0) ? 0.1f : -0.1f;
		measureCount = 0;
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onTempoIn += TempoIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		Vector3 scale = obj.transform.localScale;
		orgScale = scale.x;
		SetMyColor();
	}

	void SetMyColor()
	{
		float h = Random.Range(0.2f, 0.9f);
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		Color color = Color.HSVToRGB(h, colorS, colorV);
		color.a = 0.8f;
		// Debug.Log($"Color: {color}");
		renderer.material.color = color;
	}

	public void Update()
	{
		zoom = Mathf.Lerp(zoom, 1.0f, Time.deltaTime * 3f);
		gameObject.transform.localScale = new Vector3(orgScale * zoom, orgScale * zoom, 0.01f);
	}
	public void Stop() {
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onTempoIn -= TempoIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec) {
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		// LyricItem.OnBeat += () => { };
		zoom = 1.5f;
		gameObject.transform.localScale = new Vector3(orgScale * zoom, orgScale * zoom, 0.01f);
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		measureCount++;
		if (measureCount < 2) {
			SetMyColor();
		} else {
			fDestroy = true;
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