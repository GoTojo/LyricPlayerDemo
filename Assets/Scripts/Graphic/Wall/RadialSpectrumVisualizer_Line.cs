using UnityEngine;

public class RadialSpectrumVisualizer_Line : MonoBehaviour
{
    public int numberOfLines = 64;
    public float radius = 5f;
    public float baseLength = 0.5f;
    public float maxAddLength = 10f;
    public float lengthScale = 200f;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    private LineRenderer[] lines;
    private float[] spectrum;
    public AudioSource audioSource;
    public Material lineMaterial;
	public float startWidth = 0.05f;
	public float endWidth = 0.1f;

    void Start()
    {
        spectrum = new float[1024];
        lines = new LineRenderer[numberOfLines];

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lineObj = new GameObject("SpectrumLine_" + i);
            lineObj.transform.SetParent(transform);

            float angle = i * Mathf.PI * 2f / numberOfLines;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            lineObj.transform.localPosition = dir * radius;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            lr.startWidth = startWidth;
            lr.endWidth = endWidth;
            lr.numCapVertices = 4;

            // 初期位置（中心→短いバー）
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, dir * baseLength);

            lines[i] = lr;
        }
    }

	void Update()
	{
		audioSource.GetSpectrumData(spectrum, 0, fftWindow);

		for (int i = 0; i < numberOfLines; i++)
		{
			float t = (float)i / numberOfLines;
			int index = GetLogIndex(t);
			float intensity = Mathf.Clamp01(spectrum[index] * lengthScale);
			float totalLength = baseLength + intensity * maxAddLength;

			float angle = i * Mathf.PI * 2f / numberOfLines;
			Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

			lines[i].SetPosition(0, -dir * totalLength);
			lines[i].SetPosition(1,  dir * totalLength);
		}
	}

    int GetLogIndex(float t)
    {
        float logMin = Mathf.Log10(1);
        float logMax = Mathf.Log10(spectrum.Length);
        float logIndex = Mathf.Lerp(logMin, logMax, t);
        return Mathf.Clamp((int)Mathf.Pow(10, logIndex), 0, spectrum.Length - 1);
    }
}