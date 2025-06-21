using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BGObjectController {
	public bool fDestroy = false;
	public GameObject gameObject;
	public BGObjectController(GameObject obj)
	{
		gameObject = obj;
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onTempoIn += TempoIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}
	public virtual void Update()
	{

	}
	public void Stop()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onTempoIn -= TempoIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
		TempoDo(msecPerQuaterNote, tempo, currentMsec);
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		BeatDo(numerator, denominator, currentMsec);
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		MeasureDo(measure, measureInterval, currentMsec);
	}
	public virtual void TempoDo(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
	}

	public virtual void BeatDo(int numerator, int denominator, uint currentMsec) {
	}

	public virtual void MeasureDo(int measure, int measureInterval, uint currentMsec) {
	}
};

public class BGCircleController : BGObjectController
{
	public int measureCount = 0;
	private float zoom = 1.0f;
	private float orgScale = 1.0f;
	private LineRenderer lineRenderer;
	private MeshRenderer meshRenderer;
	private Material material;
	private const int segments = 100;
	private Rect area;
	private float x;
	private float y;
	private float r;
	public float colorH = 0f;
	public float colorS = 0.8f;
	public float colorV = 0.8f;

	public BGCircleController(GameObject obj, Rect area) : base(obj)
	{
		this.area = area;
		meshRenderer = obj.GetComponent<MeshRenderer>();
		lineRenderer = obj.GetComponent<LineRenderer>();
		material = obj.GetComponent<Material>();
		x = Random.Range((float)area.x, (float)area.x + (float)area.width);
		y = Random.Range((float)area.y, (float)area.y + (float)area.height);
		r = Random.Range(1.0f, 3.0f) * zoom;
		colorH = Random.Range(0f, 1.0f);
		Draw();
	}
	public override void Update()
	{
		if (zoom > 1.0f) {
			zoom = Mathf.Lerp(zoom, 1.0f, Time.deltaTime * 3f);
			Draw();
		}
	}
	public override void BeatDo(int numerator, int denominator, uint currentMsec)
	{
		// LyricItem.OnBeat += () => { };
		zoom = 1.5f;
		gameObject.transform.localScale = new Vector3(orgScale * zoom, orgScale * zoom, 0.01f);
	}
	public override void MeasureDo(int measure, int measureInterval, uint currentMsec)
	{
		measureCount++;
		if (measureCount < 1) {
			lineRenderer.positionCount = 0;
		} else {
			fDestroy = true;
		}
	}
	private void Draw()
	{
		lineRenderer.positionCount = 0;
		Color color = Color.HSVToRGB(colorH, colorS, colorV);
		color.a = 0.8f;
		meshRenderer.material.color = color;
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		lineRenderer.positionCount = segments + 1;

		float deltaTheta = 2.0f * Mathf.PI / segments;
		float theta = 0.0f;
		float _r = r * zoom;

		for (int i = 0; i < segments + 1; i++) {
			float _x = _r * Mathf.Cos(theta) + x;
			float _y = _r * Mathf.Sin(theta) + y;
			Vector3 pos = new Vector3(_x, _y, 8f);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
		// Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
		// rigidbody.AddForce(new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.0f), ForceMode.Impulse);
	}
};

public class BGQuadController : BGObjectController {
	public float colorS = 0.8f;
	public float colorV = 0.8f;
	public void SetColorS() {
		colorS = Random.Range(0.2f, 0.9f);
	}
	private int measureCount = 0;
	private BGQuadObject quadObject;
	private MeshRenderer renderer;
	private float zoom = 1.0f;
	private float orgScale = 1.0f;
	public BGQuadController(GameObject obj) : base(obj) {
		quadObject = obj.GetComponent<BGQuadObject>();
		renderer = gameObject.GetComponent<MeshRenderer>();
		int direction = Random.Range(0, 1);
		quadObject.rotateDelta = (direction == 0) ? 0.1f : -0.1f;
		measureCount = 0;
		Vector3 scale = obj.transform.localScale;
		orgScale = scale.x;
		SetColor();
	}

	private void SetColor()
	{
		float h = Random.Range(0f, 1f);
		Color color = Color.HSVToRGB(h, colorS, colorV);
		color.a = 0.7f;
		// Debug.Log($"Color: {color}");
		renderer.material.color = color;
	}

	public override void Update()
	{
		if (zoom > 1.0f) {
			zoom = Mathf.Lerp(zoom, 1.0f, Time.deltaTime * 3f);
			gameObject.transform.localScale = new Vector3(orgScale * zoom, orgScale * zoom, 0.01f);
		}
	}
	public override void TempoDo(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
	}

	public override void BeatDo(int numerator, int denominator, uint currentMsec) {
		// LyricItem.OnBeat += () => { };
		zoom = 1.5f;
	}

	public override void MeasureDo(int measure, int measureInterval, uint currentMsec) {
		measureCount++;
		if (measureCount < 2) {
			SetColor();
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