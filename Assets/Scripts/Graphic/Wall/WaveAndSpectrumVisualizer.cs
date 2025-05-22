using UnityEngine;

public class WaveAndSpectrumVisualizer : MonoBehaviour
{
    public LineRenderer waveformLine;
    public LineRenderer spectrumLine;
    public int sampleSize = 512;
    public float waveformHeight = 5f;
    public float spectrumHeight = 20f;

    public AudioSource audioSource;
    private float[] waveform;
    private float[] spectrum;

    void Start()
    {
        waveform = new float[sampleSize];
        spectrum = new float[sampleSize];

        waveformLine.positionCount = sampleSize;
        spectrumLine.positionCount = sampleSize;
    }

    void Update()
    {
        // 波形（時間領域）
        audioSource.GetOutputData(waveform, 0);

        for (int i = 0; i < sampleSize; i++)
        {
            float x = (float)i / sampleSize;
            float y = waveform[i] * waveformHeight;
            waveformLine.SetPosition(i, new Vector3(x, y, 0));
        }

        // スペクトラム（周波数領域）
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < sampleSize; i++)
        {
            float x = (float)i / sampleSize;
            float y = spectrum[i] * spectrumHeight;
            spectrumLine.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}