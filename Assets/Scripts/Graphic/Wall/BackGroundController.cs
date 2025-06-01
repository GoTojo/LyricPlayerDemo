// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class BackGroundController : MonoBehaviour
{
	public Parameter parameter;
	// Start is called before the first frame update
	public Rect area = new Rect(-30, 5, 60, 10);
	public float sizeMin = 0.001f;
	public float sizeMax = 0.003f;
	public GameObject wall;
	public int numOfCube = 20;
	public int numOfCircle = 10;
	private Parameter.WallType type;
	private const int segments = 100;
	private List<BGObjectController> bgObjectControllers = new List<BGObjectController>();
 
	void Start()
	{
		type = (Parameter.WallType)PlayerPrefs.GetInt("WallType");
		SetColor();
		CreateNew();
		MidiMaster.noteOnDelegate += NoteOn;
		MidiMaster.noteOffDelegate += NoteOff;
		MidiMaster.knobDelegate += knobChanged;
		MidiWatcher midiWatcher = MidiWatcher.Instance;

		midiWatcher.onMeasureIn += MeasureIn;
	}
	void OnDestroy()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= knobChanged;
		PlayerPrefs.SetInt("WallType", (int)type);
		// Debug.Log("Destract");
	}

	// Update is called once per frame
	void Update() {
		foreach (BGObjectController controller in bgObjectControllers) {
			controller.Update();
		}
	}

	private void CreateNew()
	{
		if (type == Parameter.WallType.Circle) {
			CreateCircle();
		} else {
			CreateCube();
		}
	}

	private void SetColor() {
	}

	private void CreateCube()
	{
		foreach (BGObjectController controller in bgObjectControllers) {
			if (controller.fDestroy) {
				Destroy(controller.gameObject);
				controller.Stop();
			}
		}
		bgObjectControllers.RemoveAll(controller => controller.fDestroy == true);
		if (bgObjectControllers.Count > 0) return;
		for (var i = 0; i < numOfCube; i++) {
			GameObject obj = (GameObject)Resources.Load("Prefab/Wall/BGQuad");
			float x = Random.Range((float)area.x, (float)area.x + (float)area.width);
			float y = Random.Range((float)area.y, (float)area.y + (float)area.height);
			float size = Random.Range(sizeMin, sizeMax);
			Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
			GameObject instantiatedObj = Instantiate(obj, new Vector3(x, y, 19), rotation);
			instantiatedObj.transform.parent = wall.transform;
			instantiatedObj.transform.localScale = new Vector3(size, size, 0.01f);
			instantiatedObj.transform.SetParent(wall.transform);
			bgObjectControllers.Add(new BGQuadController(instantiatedObj));
		}
	}

	private void CreateCircle()
	{
		foreach (BGObjectController controller in bgObjectControllers) {
			if (controller.fDestroy) {
				Destroy(controller.gameObject);
				controller.Stop();
			}
		}
		bgObjectControllers.RemoveAll(controller => controller.fDestroy == true);
		for (var i = 0; i < numOfCircle; i++) {
			GameObject obj = new GameObject();
			obj.transform.SetParent(wall.transform);
			obj.transform.position = new Vector3(0, 0, 0);
			LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
			// lineRenderer.useWorldSpace = false;
			Material material = new Material(Shader.Find("Sprites/Default"));
			lineRenderer.material = material;
			MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lineRenderer.widthMultiplier = 0.05f;
			bgObjectControllers.Add(new BGCircleController(obj, area));
		}
	}

	public static void BeatIn()
	{
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		CreateNew();
	}

	private void NoteOn(MidiChannel channel, int note, float velocity) {
		// Debug.Log("NoteOn: " + channel + ", " + note + ", " + velocity);
		if (note == Parameter.NoteWallTypeRect) {
			type = Parameter.WallType.Rectangle;
		} else if (note == Parameter.NoteWallTypeCircle) {
			type = Parameter.WallType.Circle;
		}
	}

	private void NoteOff(MidiChannel channel, int note) {
		// Debug.Log("NoteOff: " + channel + ", " + note);
	}

	private void knobChanged(MidiChannel ch, int ccNum, float value) {
		// Debug.Log($"knobChanged: ch{ch}, cc{ccNum}: {value}");
	}
}
