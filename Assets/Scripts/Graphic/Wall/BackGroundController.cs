// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class BackGroundController : MonoBehaviour
{
	public Parameter parameter;
	// Start is called before the first frame update
	public static Rect area = new Rect(-20, 10, 40, 20);
	public GameObject wall;
	public static int numOfCube = 30;
	private Parameter.WallType type;
	private const int segments = 100;
	private BGQuadController [] quadController = new BGQuadController[numOfCube];
 
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
	void OnDestroy() {
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= knobChanged;
		PlayerPrefs.SetInt("WallType", (int)type);
		// Debug.Log("Destract");
	}

	// Update is called once per frame
	void Update()
	{
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
		// BGQuad.SetColor();
		// 	float h = 1.0f / (i + 1);
		// 	GameObject obj = (GameObject)Resources.Load($"BGQuad");
		// 	Renderer renderer = obj.GetComponent<Renderer>();
		// 	Color color = Color.HSVToRGB(h, s, v);
		// 	color.a = 0.8f;
		// 	// Debug.Log($"Color: {color}");
		// 	renderer.sharedMaterial.color = color;
		// 	colors[i] = color;
		// }
	}

	private void CreateCube()
	{
		for (var i = 0; i < numOfCube; i++)
		{
			int colorId = Random.Range(1, 16);
			GameObject obj = (GameObject)Resources.Load("Prefab/Wall/BGQuad");
			float x = Random.Range((float)area.x, (float)area.x + (float)area.width);
			float y = Random.Range((float)area.y, (float)area.y - (float)area.height);
			float size = Random.Range(2.0f, 5.0f);
			Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
			obj.transform.localScale = new Vector3(size, size, 0.01f);
			GameObject instantiatedObj = Instantiate(obj, new Vector3(x, y, 0.1f), rotation);
			instantiatedObj.transform.SetParent(wall.transform);
			quadController[i] = new BGQuadController(instantiatedObj);
		}
	}

	private void CreateCircle()
	{
		// wall = new GameObject("WallPaper");
		// for (var i = 0; i < numOfCube; i++) {
		// 	GameObject obj = new GameObject();
		// 	items[i] = obj;
		// 	obj.transform.SetParent(wall.transform);
		// 	int colorId = Random.Range(0, 15);
		// 	LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
		// 	lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		// 	Color color = colors[colorId];
		// 	lineRenderer.startColor = color;
		// 	lineRenderer.endColor = color;
		// 	// lineRenderer.material = (Material)Resources.Load($"material/BGItem {colorId}");
		// 	// MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		// 	lineRenderer.widthMultiplier = 0.05f;
		// 	lineRenderer.positionCount = segments + 1;

		// 	float x = Random.Range((float)area.x, (float)area.x + (float)area.width);
		// 	float y = Random.Range((float)area.y, (float)area.y - (float)area.height);
		// 	float r = Random.Range(1.0f, 3.0f);
		// 	float deltaTheta = 2.0f * Mathf.PI / segments;
		// 	float theta = 0.0f;

		// 	for (int j = 0; j < segments + 1; j++) {
		// 		float _x = r * Mathf.Cos(theta) + x;
		// 		float _y = r * Mathf.Sin(theta) + y;
		// 		Vector3 pos = new Vector3(_x, _y, 0f);
		// 		lineRenderer.SetPosition(j, pos);
		// 		theta += deltaTheta;
		// 	}
		// 	Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
		// 	rigidbody.AddForce(new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.0f), ForceMode.Impulse);
		// }
	}

	public static void BeatIn()
	{
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		CreateCube();
	}

	private void NoteOn(MidiChannel channel, int note, float velocity) {
		// Debug.Log("NoteOn: " + channel + ", " + note + ", " + velocity);
		if (note == parameter.NoteWallTypeRect) {
			type = Parameter.WallType.Rectangle;
		} else if (note == parameter.NoteWallTypeCircle) {
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
