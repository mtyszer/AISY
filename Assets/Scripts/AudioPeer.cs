using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MenuHandler;


[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource AS;
    GameObject ParSystem;
    Camera Cam;
    public static float[] samples = new float[2048];
    public ParticleSystem ParticleSide;
    [Range(0.0f, 800.0f)]
    public float responsiveness;
    public float maxValue;
    public float minValue;
    [Range(0.0f, 3.0f)]
    public float muffler;
    public Slider slider;
    public Slider volumeSlider;
    public TextMeshProUGUI text;
    public TextMesh textMesh;
    public string songName;
    public GameObject dev;
    public static float amp, ampBuffer;
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.clip = song;
        AS.loop = true;
        AS.Play();
        ParSystem = GameObject.Find("PSS");
        Cam = Camera.main;
    }

    void Update()
    {
        GetSpectrumAudioSource();
        SoundFOVChange();
        ParticleSystemTest();
        ParticleMovement(ParticleSide);
        GetSongTime();
        ShowTime();
        GetVolume();
        DevmodToggle();
    }
    void ParticleSystemTest()
    {
        float sampleAveragetion;
        sampleAveragetion = Mathf.Min(AverageSampleValue(samples) * responsiveness, minValue);
        if (sampleAveragetion >= maxValue)
        {
            sampleAveragetion -= muffler;
            Debug.LogWarning($"SampleAvg: {sampleAveragetion}, MaxValue: {maxValue}, MufflerValue: {muffler}");
        }
        ParSystem.transform.localScale = new Vector3(sampleAveragetion, sampleAveragetion, sampleAveragetion);
    }

    void DevmodToggle()
    {
        if (devmode == false)
        {
            dev.SetActive(false);
        }
    }

    void ParticleMovement(ParticleSystem particle)
    {
        float sampleAveragetion;
        sampleAveragetion = 0.5f + (AverageSampleValue(samples) * responsiveness);
        var main = particle.main;
        main.startSpeed = sampleAveragetion * 10;
    }

    void GetSongTime()
    {
        var song = AS.clip;
        var audioLength = song.length;
        slider.maxValue = audioLength;
        if (slider.value >= audioLength)
        {
            AS.time = 0;
        }
        else
        {
            slider.value = AS.time;
        }
    }

    public void ChangeSongTime(float newTime)
    {
        AS.time = newTime;
        if (newTime == slider.maxValue)
        {
            AS.time = 0;
            slider.value = 0;
        }
    }

    void GetVolume()
    {
        volumeSlider.maxValue = 1.0f; 
        volumeSlider.value = AS.volume;
    }

    public void ChangeVolume(float newVolume)
    {
        AS.volume = newVolume;
    }

    void ShowTime()
    {
        float time = (int)AS.time;
        text.text = time.ToString();
    }

    void GetSpectrumAudioSource()
    {
        AS.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void SoundFOVChange()
    {
        float sampleAveragetion;
        if (agressivness == true)
        {
            sampleAveragetion = AverageSampleValue(samples) * responsiveness * 5;
        }
        else
        {
            sampleAveragetion = AverageSampleValue(samples) * responsiveness;
        }
        Cam.fieldOfView = 60 + sampleAveragetion;
    }
    public float AverageSampleValue(params float[] samplesAverage)
    {
        float result = samplesAverage.Average();
        return result;
    }
}
