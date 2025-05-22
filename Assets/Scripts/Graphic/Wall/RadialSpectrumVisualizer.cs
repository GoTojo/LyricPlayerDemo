using UnityEngine;

public class RadialSpectrumVisualizer : MonoBehaviour
{
	public int numberOfBars = 64;
	public float radius = 5f;
	public float baseLength = 0.5f;
	public float maxAddLength = 10f;
	public float lengthScale = 200f;
	public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

	private GameObject[] bars;
	private float[] spectrum;
	public AudioSource audioSource;
	public Material material;

	void Start()
	{
		spectrum = new float[1024];
		// audioSource = GetComponent<AudioSource>();
		bars = new GameObject[numberOfBars];
		GameObject barPrefab = Resources.Load<GameObject>("Prefab/Wall/SpectrumVisualizer");

		for (int i = 0; i < numberOfBars; i++)
		{
			GameObject bar = Instantiate(barPrefab, transform);
			bar.transform.SetParent(transform);

			float angle = i * Mathf.PI * 2f / numberOfBars;
			Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
			bar.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);

			// 円周上に配置（中心から一定距離）
			bar.transform.localPosition = dir * radius;

			// 初期サイズ（短いバー）
			bar.transform.localScale = new Vector3(0.1f, baseLength, 0.1f);

			// 影なし（軽量・見やすさ）
			var renderer = bar.GetComponent<MeshRenderer>();
			renderer.sharedMaterial = material;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;

			bars[i] = bar;
		}
	}

	void Update()
	{
		audioSource.GetSpectrumData(spectrum, 0, fftWindow);

		for (int i = 0; i < numberOfBars; i++)
		{
			// t は 0〜1 の範囲
			float t = (float)i / numberOfBars;
			int index = GetLogIndex(t);

			float intensity = Mathf.Clamp01(spectrum[index] * lengthScale);
			float totalLength = baseLength + intensity * maxAddLength;

			Vector3 scale = bars[i].transform.localScale;
			scale.y = Mathf.Lerp(scale.y, totalLength, Time.deltaTime * 100);
			bars[i].transform.localScale = scale;
		}
	}

	int GetLogIndex(float t)
	{
		// t = 0..1 の範囲で log スケーリング
		float logMin = Mathf.Log10(1);
		float logMax = Mathf.Log10(spectrum.Length);
		float logIndex = Mathf.Lerp(logMin, logMax, t);
		return Mathf.Clamp((int)Mathf.Pow(10, logIndex), 0, spectrum.Length - 1);
	}
}