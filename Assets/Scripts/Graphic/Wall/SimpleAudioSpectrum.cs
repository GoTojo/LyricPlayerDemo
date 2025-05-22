using UnityEngine;

public class SimpleAudioSpectrum : MonoBehaviour
{
    public AudioSource audioSource;
    public ParticleSystem particles;
    public int band = 2; // 0〜63くらいまで
    public float multiplier = 50f;

    float[] spectrum = new float[64];

    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float value = spectrum[band] * multiplier;

        var main = particles.main;
        main.startSize = 1f + value;
    }
}