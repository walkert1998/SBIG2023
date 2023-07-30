using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider gameVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider voiceVolumeSlider;
    public Slider fovSlider;
    public Slider mouseSensitivitySlider;
    public Toggle cameraTiltToggle;
    public Toggle vsyncToggle;
    public TMP_Text fovValueDisplay;
    public TMP_Text MouseSensitivityValueDisplay;
    private static int fov;
    private static int resolutionIndex = 0;
    private static float effectsVolume = 0;
    private static float musicVolume = 0;
    private static float voiceVolume = 0;
    private static float mouseSensitivity = 0.05f;
    private static bool cameraTilt = true;
    private static bool vsyncEnabled = true;
    public Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;
    public TMP_Dropdown graphicQualityDropDown;
    public Camera playerCamera;

    // Start is called before the first frame update
    void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        if (resolutionDropDown != null)
        {
            resolutionDropDown.ClearOptions();
            PopulateResolutionOptions();
            ChangeResolution();
        }
        if (graphicQualityDropDown != null)
        {
            graphicQualityDropDown.value = PlayerPrefs.GetInt("QualityLevel", 5);
            SetGraphicQualityLevel();
        }
        if (gameVolumeSlider != null)
        {
            if (!PlayerPrefs.HasKey("FXVolume"))
            {
                PlayerPrefs.SetFloat("FXVolume", 0.75f);
            }
            gameVolumeSlider.value = PlayerPrefs.GetFloat("FXVolume", 0.75f);
            audioMixer.SetFloat("FXVolume", Mathf.Log10(gameVolumeSlider.value) * 20);
        }
        if (musicVolumeSlider != null)
        {
            if (!PlayerPrefs.HasKey("MusicVolume"))
            {
                PlayerPrefs.SetFloat("MusicVolume", 0.25f);
            }
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.25f);
            Debug.Log(musicVolumeSlider.value + " " + PlayerPrefs.GetFloat("MusicVolume", 0.25f));
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolumeSlider.value) * 20);
        }
        if (voiceVolumeSlider != null)
        {
            if (!PlayerPrefs.HasKey("VoiceVolume"))
            {
                PlayerPrefs.SetFloat("VoiceVolume", 0.25f);
            }
            voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 0.25f);
            Debug.Log(voiceVolumeSlider.value + " " + PlayerPrefs.GetFloat("VoiceVolume", 0.25f));
            audioMixer.SetFloat("VoiceVolume", Mathf.Log10(voiceVolumeSlider.value) * 20);
        }
        if (fovSlider != null)
        {
            fov = PlayerPrefs.GetInt("FOV", 90);
            fovSlider.value = fov;
            //Debug.Log(fov);
            playerCamera.fieldOfView = fov;
            fovValueDisplay.text = fov.ToString();
        }
        if (cameraTiltToggle != null)
        {
            int val = PlayerPrefs.GetInt("CameraTilt", 1);
            if (val == 1)
            {
                cameraTilt = true;
            }
            else
            {
                cameraTilt = false;
            }
            cameraTiltToggle.isOn = cameraTilt;
        }
        if (vsyncToggle != null)
        {
            int val = PlayerPrefs.GetInt("VsyncEnabled", 1);
            if (val == 1)
            {
                vsyncEnabled = true;
                QualitySettings.vSyncCount = 1;
                // Application.targetFrameRate = 60;
            }
            else
            {
                vsyncEnabled = false;
                QualitySettings.vSyncCount = 0;
                // Application.targetFrameRate = 120;
            }
            vsyncToggle.isOn = cameraTilt;
        }
    }

    public void PopulateResolutionOptions()
    {
        List<string> options = new List<string>();
        int i = 0;
        int startResolution = i;
        foreach (Resolution res in resolutions)
        {
            string option = res.width + "x" + res.height;
            if (res.width == Screen.width && res.height == Screen.height)
            {
                startResolution = i;
            }
            options.Add(option);
            i++;
        }
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = startResolution;
    }

    public void SetFOV(int amount)
    {
        fov = amount;
    }

    public static int GetFOV()
    {
        return fov;
    }

    public void SetMouseSensitivity()
    {
        // Dividing by 1000 so you can get 0.05 (actual value) from 50 (slider value)
        mouseSensitivity = mouseSensitivitySlider.value / 1000f;
        // playerCamera.m_VerticalAxis.m_MaxSpeed = mouseSensitivity;
        Debug.Log(mouseSensitivity);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        MouseSensitivityValueDisplay.text = mouseSensitivitySlider.value.ToString();
    }

    public static float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetVolume()
    {
        audioMixer.SetFloat("FXVolume", Mathf.Log10(gameVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("FXVolume", gameVolumeSlider.value);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(voiceVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolumeSlider.value);

    }

    public void ChangeResolution()
    {
        int index = resolutionDropDown.value;
        int width = resolutions[index].width;
        int height = resolutions[index].height;
        Screen.SetResolution(width, height, true);
    }

    public void SetFOV()
    {
        fov = (int)fovSlider.value;
        playerCamera.fieldOfView = fov;
        PlayerPrefs.SetInt("FOV", fov);
        fovValueDisplay.text = fov.ToString();
    }

    public void ToggleCameraTilt()
    {
        cameraTilt = cameraTiltToggle.isOn;
        int val = 0;
        if (cameraTilt)
        {
            val = 1;
        }
        PlayerPrefs.SetInt("CameraTilt", val);
    }

    public static bool IsCameraTiltOn()
    {
        return cameraTilt;
    }

    public void ToggleVsync()
    {
        vsyncEnabled = vsyncToggle.isOn;
        int val = 0;
        if (vsyncEnabled)
        {
            Application.targetFrameRate = 60;
            // QualitySettings.vSyncCount = 1;
            val = 1;
        }
        else
        {
            // QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }
        PlayerPrefs.SetInt("VsyncEnabled", val);
    }

    public static bool IsVsyncEnabled()
    {
        return vsyncEnabled;
    }

    public void SetGraphicQualityLevel()
    {
        QualitySettings.SetQualityLevel(graphicQualityDropDown.value);
        PlayerPrefs.SetInt("QualityLevel", graphicQualityDropDown.value);
    }
}
