using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Waveform : MonoBehaviour
{
	public AudioSource audioSource;
	private float[] audioData;
	[HideInInspector] public float[] spectrumData = null;
	private int dataOffset = 0;

	int sampleStep = 0;
	Vector3[] samplingLinePoints = null;
	public float waveLength = 20f;
	public float yLength = 3f;
	public float yOffset = 3f;
	public bool active = false;
	private LineRenderer lineRenderer;
	private float zLine = 0.1f;

	void Start()
	{
		GameObject gameObject = this.gameObject;
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		Material material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.material = material;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRenderer.widthMultiplier = 0.05f;
		yLength = 3.0f;
		var clip = audioSource.clip;
		audioData = new float[clip.channels * clip.samples];
		clip.GetData(audioData, dataOffset);
		spectrumData = new float[512];
		Prepare();
	}
	private void Prepare()
	{
		var fps = Mathf.Max(60f, 1f / Time.fixedDeltaTime);
		var clip = audioSource.clip;
		sampleStep = (int) (clip.frequency / fps);
		samplingLinePoints = new Vector3[sampleStep];
	}

	private void FixedUpdate()
	{
		if (active) {
			if (audioSource.isPlaying && audioSource.timeSamples < audioData.Length) {
				var startIndex = audioSource.timeSamples;
				var endIndex = audioSource.timeSamples + sampleStep;
				Inflate(
					audioData, startIndex, endIndex,
					samplingLinePoints,
					waveLength, -waveLength / 2f, yLength
				);
				Render(samplingLinePoints);
			} else {
				Reset();
			}
		} else {
			lineRenderer.positionCount = 0;
		}
	}

	private void Render(Vector3[] points)
	{
		if (points == null) return;
		lineRenderer.positionCount = points.Length;
		lineRenderer.SetPositions(points);
	}

	private void Reset()
	{
		var x = -waveLength / 2;
		Render(new[]
		{
			new Vector3(-x, yOffset, zLine) + this.transform.position,
			this.transform.position,
			new Vector3(x, yOffset, zLine) + this.transform.position,
		});
	}

	public void Inflate(
		float[] target, int start, int end,
		Vector3[] result,
		float xLength, float xOffset, float yLength
	)
	{
		var samples = Mathf.Max(end - start, 1f);
		var xStep = xLength / samples;
		var j = 0;

		for (var i = start; i < end; i++, j++)
		{
			var x = xOffset + xStep * j;
			var y = i < target.Length ? target[i] * yLength : 0f;
			var p = new Vector3(x, y + yOffset, zLine) + this.transform.position;
			result[j] = p;
		}
	}

	// Update is called once per frame
	void Update()
	{
		audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);
	}

}