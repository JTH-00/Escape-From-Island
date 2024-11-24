using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManageMent : MonoBehaviour
{
    public GameObject MainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullScreenToggle;
    public int[] screenWidths;
    int activeScreenResIndex;



    // Start is called before the first frame update
    void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("Screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;
        
        //��ü ���� ����
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        //���� �Ҹ� ����
        volumeSliders[2].value = AudioManager.instance.musicVolumePercent;
        //����Ʈ ���� ����
        volumeSliders[1].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        fullScreenToggle.isOn = isFullscreen;

    }

    // Update is called once per frame
    void Update()
    {

    }
    //���� ��ư ����
    public void Play()
    {   
        SceneManager.LoadScene("GameScene");
    }
    //ȯ�漳�� ��ư ����
    public void OptionsMenu()
    {
        MainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }
    //������ ��ư ����
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("���������� �׽�Ʈ �ϱ�");
    }
    public void MainMenu()
    {
        MainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }
    public void SetscreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }
    public void SetFullscreen(bool isFullscreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }
        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetscreenResolution(activeScreenResIndex);
        }
        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}
    
