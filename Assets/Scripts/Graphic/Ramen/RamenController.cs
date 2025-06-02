using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class RamenController : MonoBehaviour
{
	private GameObject ramen;
	private Vector3 orgScale = new Vector3(3, 3, 3);
	public Rect area = new Rect(-10, 4, 20, 8);
	private int grid = 0;
	public int maxGrid = 8;
	private bool is1st = false;
	private int numOfItem = 1;
	private List<GameObject> objs = new List<GameObject>();
	// Start is called before the first frame update
	void Start()
	{
		MidiMaster.noteOnDelegate += NoteOn;
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn += BeatIn;
		numOfItem = 0;
		Create();
	}
	public void Release()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onBeatIn -= BeatIn;
	}
	// Update is called once per frame
	void Update()
	{
		if (grid < 0) grid = 0;
		if (grid > 16) grid = 16;
		numOfItem = grid * grid;
		if (objs.Count != numOfItem) {
			for (var i = 0; i < objs.Count; i++) {
				Destroy(objs[i]);
			}
			objs.Clear();
			Create();
		}
	}

	private Vector3 GetPosition(int numOfItem, int num)
	{
		int row = num % grid;
		int numOfRow = grid;
		int line = num / grid;
		int numOfLine = grid;
		float width = area.width / numOfRow;
		float height = area.height / numOfLine;
		float x = area.x + (width * row) + (width / 2);
		float y = area.y - ((height * line) + (height / 2));
		Vector3 pos = new Vector3(x, y, 0);
		return pos;
	}

	private void Create()
	{
		if (grid == 0) return;
		Vector3 scale = orgScale / grid;
		for (var i = 0; i < numOfItem; i++) {
			GameObject obj = (GameObject)Resources.Load("Prefab/TiltedRamen");
			GameObject instantiatedObj = Instantiate(obj, this.transform);
			instantiatedObj.transform.localPosition = GetPosition(numOfItem, i);
			instantiatedObj.transform.localScale = scale;
			objs.Add(instantiatedObj);
		}
	}

	public void CreateRamen()
	{
		grid = 1;
		is1st = true;
	}

	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		if (note == Parameter.NoteRamenStart) {
			CreateRamen();
		}
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		// if (is1st) {
		// 	is1st = false;
		// } else
		if (grid == maxGrid) {
			grid = 0;
		} else if (grid > 0) {
			grid++;
		}
	}
}
