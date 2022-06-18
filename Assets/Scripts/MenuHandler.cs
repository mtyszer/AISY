using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Networking;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        errorPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        AgressiveCheck();
        DevCheck();
    }

    public static bool agressivness;
    public static bool devmode;
    public static bool pyszynski;
    public Toggle agressiveToggle;
    public Toggle devToggle;
    public static AudioClip song;
    public GameObject errorPanel;
    public GameObject optionsPanel;
    public GameObject inputField;
    public TextMeshProUGUI errorText;
    bool songChoosen;
    string path;
    private AudioType audioType;

    public void ErrorCloseButton()
    {
        errorPanel.SetActive(false);
    }
    public void StartButton()
    {
        if (songChoosen)
        {
            SceneManager.LoadScene("Visualizer");
        }
        else
        {
            errorPanel.SetActive(true);
            errorText.text = "Choose a song first";
        }
    }

    void AgressiveCheck()
    {
        agressivness = agressiveToggle.isOn;
    }

    public void AgressivnessChange(bool agr)
    {
        agressivness = agr;
    }
    void DevCheck()
    {
        devmode = devToggle.isOn;
    }
    public void DevChange(bool dev)
    {
        devmode = dev;
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void OptionsButton()
    {
        optionsPanel.SetActive(true);
    }

    public void OptionsCloseButton()
    {
        optionsPanel.SetActive(false);
    }
    public void OpenFileExplorer()
    {
        path = EditorUtility.OpenFilePanel("Show all songs", "", "wav, mp3");
        StartCoroutine(GetSong());
    }

    IEnumerator GetSong()
    {
        if(path.Contains("wav"))
        {
            audioType = AudioType.WAV;
        }
        else if (path.Contains("mp3"))
        {
            audioType = AudioType.MPEG;
        }


        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, audioType);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AudioClip clip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            song = clip;
            songChoosen = true;
        }

    }


}
