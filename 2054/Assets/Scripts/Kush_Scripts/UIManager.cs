using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    [Header("Main UI")]
    [SerializeField] GameObject mainMenuCanvasParent;
    [SerializeField] GameObject mainMenuScreenParent;
    [SerializeField] GameObject pauseMenuScreenParent;
    [SerializeField] GameObject settingsScreenParent;
    [SerializeField] GameObject creditsScreenParent;

    [Header("UI Fields")]
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] UnityEngine.UI.Slider masterVolumeSlider;
    [SerializeField] UnityEngine.UI.Slider jerryVolumeSlider;

    [Header("Audio")]
    [SerializeField] AudioMixer masterAudioMixer;

    [Header("UI Guide Objs")]
    [SerializeField] TextMeshProUGUI keyToPress;
    [SerializeField] TextMeshProUGUI helperTxt;

    //internal variables
    Resolution[] resolutions;

    public void SetKeyToPress(string key = "", bool append = false)
    {
        if (!append) keyToPress.text = key;
        else keyToPress.text += key;
    }

    public void SetHelperText(string text = "", bool append = false)
    {
        if (!append) helperTxt.text = text;
        else helperTxt.text += text;
    }

    public void StartGame()
    {
        StartCoroutine(GameManager.instance.TransitionCam());
        mainMenuCanvasParent.SetActive(false);
        GameManager.instance.ActivatePlayer();

        GameManager.instance.StartGlitching();
    }

    public void OpenSettings()
    {
        if (GameManager.instance.pause)
            pauseMenuScreenParent.SetActive(false);
        else
            mainMenuScreenParent.SetActive(false);

        settingsScreenParent.SetActive(true);
    }

    public void Back(int i)
    {
        if (i == 1) //Back from settings screen
        {
            settingsScreenParent.SetActive(false);
            if ((GameManager.instance.pause))
                pauseMenuScreenParent.SetActive(true);
            else
                mainMenuScreenParent.SetActive(true);
        }

        if (i == 2) //Back from credits screen
        {
            creditsScreenParent.SetActive(false);
            mainMenuScreenParent.SetActive(true);
        }
    }

    public void OpenCredits()
    {
        mainMenuScreenParent.SetActive(false);
        creditsScreenParent.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("MasterVolume", volume);
    }
    
    public void SetJerryVolume(float volume)
    {
        masterAudioMixer.SetFloat("JerryVolume", volume);
    }

    void SetQualitySettingDropDown()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAudioSliders();
        SetQualitySettingDropDown();
        SetResolutionDropdown();
    }

    void SetAudioSliders()
    {
        masterAudioMixer.GetFloat("MasterVolume", out float masterVolume);
        masterVolumeSlider.value = masterVolume;

        masterAudioMixer.GetFloat("JerryVolume", out float jerryVolume);
        jerryVolumeSlider.value = jerryVolume;
    }

    void SetResolutionDropdown()
    {
        resolutions = GetMinMaxRefreshRateResolutions();
        resolutionDropdown.ClearOptions();
        List<string> options = new();
        int currentResolutionIndex = 0;
        int index = 0;
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + "x" + resolution.height + "@" + resolution.refreshRateRatio.value + "hz";
            options.Add(option);
            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = index;
            }
            index++;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public static Resolution[] GetMinMaxRefreshRateResolutions()
    {
        return Screen.resolutions
            .GroupBy(res => (res.width, res.height))
            .SelectMany(group =>
            {
                var minRefresh = group.OrderBy(r => r.refreshRateRatio.value).First();
                var maxRefresh = group.OrderByDescending(r => r.refreshRateRatio.value).First();

                return Mathf.Approximately((float)minRefresh.refreshRateRatio.value, (float)maxRefresh.refreshRateRatio.value)
                    ? new[] { minRefresh }
                    : new[] { minRefresh, maxRefresh };
            })
            .ToArray();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Debug.Log(resolution.width + "X" + resolution.height + "@" + resolution.refreshRateRatio.value + "hz");
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        Debug.Log(Screen.width + "X"+ Screen.height);
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void SaveGame()
    {
        GameManager.instance.SaveGame();
    }

    public void PauseSequence(bool p)
    {
        if (p)
        {
            Time.timeScale = 0;

            mainMenuCanvasParent.SetActive(true);
            mainMenuScreenParent.SetActive(false);
            pauseMenuScreenParent.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;

            pauseMenuScreenParent.SetActive(false);
            mainMenuScreenParent.SetActive(true);
            mainMenuCanvasParent.SetActive(false);

            UnityEngine.Cursor.lockState = GameManager.instance.cursorLockMode;

            GameManager.instance.pause = p;
        }
    }
}
