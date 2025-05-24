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
	private bool lastSilhouette = true;

	void Start()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
		foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
			originalMaterials[renderer] = renderer.sharedMaterials;
			originalMaterials[renderer] = renderer.materials;
		}
		lastSilhouette = !isSilhouette;
		// if (isSilhouette) {
		// 	var renderers = GetComponentsInChildren<Renderer>();
		// 	foreach (var rend in renderers) {
		// 		var mats = new Material[rend.sharedMaterials.Length];
		// 		for (int i = 0; i < mats.Length; i++)
		// 			mats[i] = silhouetteMaterial;
		// 		rend.sharedMaterials = mats;
		// 	}
		// }
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
		ChangeMaterial();
	}

	private void ChangeMaterial()
	{
		if (lastSilhouette == isSilhouette) return;
		if (isSilhouette) {
			// すべてのマテリアルを黒一色に
			// Material[] blackMats = new Material[GetComponent<Renderer>().sharedMaterials.Length];
			// for (int i = 0; i < blackMats.Length; i++) {
			// 	blackMats[i] = silhouetteMaterial;
			// }
			// GetComponent<Renderer>().sharedMaterials = blackMats;
			var renderers = GetComponentsInChildren<Renderer>();
			foreach (var rend in renderers) {
				var mats = new Material[rend.sharedMaterials.Length];
				for (int i = 0; i < mats.Length; i++)
					mats[i] = silhouetteMaterial;
				rend.sharedMaterials = mats;
			}
		} else {
			// 元に戻す
			GetComponent<Renderer>().sharedMaterials = originalMaterials[GetComponent<Renderer>()];
		}
		lastSilhouette = isSilhouette;
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		fMidiTrigger = true;
	}
}
