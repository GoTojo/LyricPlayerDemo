using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
	// Start is called before the first frame update
	private bool fMidiTrigger = false;
	public Material silhouetteMaterial; // 影絵用マテリアル
	private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
	public bool isSilhouette = true;
	private uint beatCount = 0;
	private float targetTime = 0;
	private bool fRotate = false;

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
		if (fRotate) {
			// 1/4小節で300度回す
			float deltaAngle = 300 * Time.deltaTime / targetTime;
			transform.Rotate(0f, deltaAngle, 0f);
			float angle = transform.eulerAngles.y;
			if (angle >= 120 && angle < 180) {
				transform.rotation = Quaternion.Euler(0f, 120f, 0f);
				fRotate = false;
			}
		}
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		// 1/4小節
		transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		targetTime = (float)measureInterval / 4000;
		fRotate = true;
		Debug.Log($"targetTime: {targetTime}");
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		beatCount++;
	}
}
