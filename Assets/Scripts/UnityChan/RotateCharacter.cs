using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
	// Start is called before the first frame update
	private bool fMidiTrigger = false;
	public Material silhouetteMaterial; // 影絵用マテリアル
	private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
	public bool isSilhouette = true;
	private uint beatCount = 0;

	void Start()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
		midiWatcher.onBeatIn += BeatIn;
		foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
			originalMaterials[renderer] = renderer.sharedMaterials;
			originalMaterials[renderer] = renderer.materials;
		}
		if (isSilhouette) {
			var renderers = GetComponentsInChildren<Renderer>();
			foreach (var rend in renderers) {
				var mats = new Material[rend.sharedMaterials.Length];
				for (int i = 0; i < mats.Length; i++)
					mats[i] = silhouetteMaterial;
				rend.sharedMaterials = mats;
			}
		}
		transform.parent.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		Transform transform = this.gameObject.transform;
		if (fMidiTrigger) {
			transform.rotation = Quaternion.Euler( 0f, 180f, 0f);
			fMidiTrigger = false;
		} else if (transform.eulerAngles.y <= 120 || transform.eulerAngles.y >= 180) {
			transform.Rotate( 0f, 5f, 0f);
		}
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		// fMidiTrigger = true;
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		beatCount++;
		if (beatCount % 2 == 0) {
			fMidiTrigger = true;
		}
	}
}
