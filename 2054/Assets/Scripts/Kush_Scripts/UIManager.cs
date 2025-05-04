using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    [Header("Memory UI")]
    [SerializeField] RawImage memoryImg;

    [Header("Memory Textures")]
    [SerializeField] List<Texture> memoryTextures = new();

    [Header("Main UI")]
    [SerializeField] GameObject mainMenuCanvasParent;
    [SerializeField] GameObject mainMenuScreenParent;
    [SerializeField] GameObject pauseMenuScreenParent;
    [SerializeField] GameObject settingsScreenParent;
    [SerializeField] GameObject creditsScreenParent;

    [Header("UI Fields")]
    [SerializeField] GameObject resumeButton;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] UnityEngine.UI.Slider masterVolumeSlider;
    [SerializeField] UnityEngine.UI.Slider jerryVolumeSlider;

    [Header("Audio")]
    [SerializeField] AudioMixer masterAudioMixer;

    [Header("UI Guide Objs")]
    [SerializeField] GameObject keyToPress;
    [SerializeField] TextMeshProUGUI helperTxt;

    //internal variables
    Transform bigImg;
    Transform guideImg;
    Transform img1;
    Transform img2;
    Transform helpImg;

    public enum KeyType
    {
        E,
        R,
        Q,
        MOUSE,
        TOTAL
    };

    public enum HelpType
    {
        INSPECT,
        MOVE_UP_DOWN,
        MOVE_CLOSE_FAR,
        TOTAL
    };

    public enum MemoryType
    {
        TODDLER_PAINTING,
        TODDLER_2054,
        CHILD_VOLCANO_SUCCESS,
        CHILD_VOLCANO_FAIL,
        TEEN_LAPTOP,
        ADULT_JERRY,
        ADULT_WEDDING,
        ADULT_GRADUATION,
    }

    //internal variables
    Resolution[] resolutions;


    public void SetKeyToPress(KeyType key)
    {
        keyToPress.GetComponent<RawImage>().enabled = true;
        keyToPress.GetComponent<UISpriteAnimation>().StartKeyAnimation(key);
    }

    public void SetKeyToPress()
    {
        keyToPress.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        keyToPress.GetComponent<UISpriteAnimation>().SetAllBools(false);
        keyToPress.GetComponent<RawImage>().enabled = false;
    }

    public void SetHelperText(KeyType key1 = KeyType.TOTAL, KeyType key2 = KeyType.TOTAL, HelpType help = HelpType.TOTAL)
    {
        bigImg.gameObject.SetActive(false);
        guideImg.gameObject.SetActive(false);
        helperTxt.enabled = false;

        if(key1 != KeyType.TOTAL)
        {
            img1.gameObject.SetActive(true);
            img1.GetComponent<UISpriteAnimation>().StartKeyAnimation(key1);
        }

        if(key2 != KeyType.TOTAL)
        {
            img2.gameObject.SetActive(true);

            img2.GetComponent<UISpriteAnimation>().StartKeyAnimation(key2);
        }

        if(help != HelpType.TOTAL)
        {
            helpImg.gameObject.SetActive(true);

            helpImg.GetComponent<UISpriteAnimation>().StartHelpAnimation(help);
        }
    }

    public void SetHelperText(Texture img)
    {
        img1.gameObject.SetActive(false);
        img1.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        img1.GetComponent<UISpriteAnimation>().SetAllBools(false);

        img2.gameObject.SetActive(false);
        img2.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        img2.GetComponent<UISpriteAnimation>().SetAllBools(false);

        helpImg.gameObject.SetActive(false);
        helpImg.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        helpImg.GetComponent<UISpriteAnimation>().SetAllBools(false);

        helperTxt.enabled = false;

        bigImg.gameObject.SetActive(true);

        bigImg.GetComponent<RawImage>().texture = img;
    }

    public void SetGuideImage(Texture img)
    {

        guideImg.gameObject.SetActive(true);

        guideImg.GetComponent<RawImage>().texture = img;
    }

    public void SetHelperText()
    {
        helperTxt.text = "";
        helperTxt.enabled = false;

        bigImg.gameObject.SetActive(false);

        guideImg.gameObject.SetActive(false);

        img1.gameObject.SetActive(false);
        img1.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        img1.GetComponent<UISpriteAnimation>().SetAllBools(false);

        img2.gameObject.SetActive(false);
        img2.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        img2.GetComponent<UISpriteAnimation>().SetAllBools(false);

        helpImg.gameObject.SetActive(false);
        helpImg.GetComponent<UISpriteAnimation>().StopAllCoroutines();
        helpImg.GetComponent<UISpriteAnimation>().SetAllBools(false);
    }

    public void SetHelperText(string text, bool append = false)
    {
        helperTxt.enabled = true;

        if (!append) { 
            helperTxt.text = text; 
        }
        else helperTxt.text += text;
    }

    void LoadSave()
    {
        PlayerBehaviour.instance.playerAge = (PlayerBehaviour.PlayerAge)GameManager.instance.saveValue;
        PlayerBehaviour.instance.SetPlayerHeight();
        RoomManager.instance.currentRoomType = (RoomManager.Room)GameManager.instance.saveValue;
        RoomManager.instance.UpdateRoom();

        switch (GameManager.instance.saveValue)
        {
            case 0:
                PuzzleManager.instance.SetupPuzzleJigsaw2054();
                break;
        }
    }

    

    void LoadDefaultAge()
    {
        PlayerBehaviour.instance.playerAge = (PlayerBehaviour.PlayerAge)0;
        PlayerBehaviour.instance.SetPlayerHeight();
        RoomManager.instance.currentRoomType = (RoomManager.Room)0;
        RoomManager.instance.UpdateRoom();

        PuzzleManager.instance.SetupPuzzleJigsaw2054();
    }

    public void StartGame(bool loadSave)
    {

        if (loadSave && GameManager.instance.saveValue != -1)
        {
            LoadSave();
        }
        else
        {
            GameManager.instance.saveValue = -1;

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            LoadDefaultAge();
        }

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
        if(GameManager.instance.saveValue != -1)
        {
            resumeButton.SetActive(true);
        }

        SetAudioSliders();
        SetQualitySettingDropDown();
        SetResolutionDropdown();

        SetUIChilds();
    }

    void SetUIChilds()
    {
        img1 = helperTxt.gameObject.transform.GetChild(0);
        img2 = helperTxt.gameObject.transform.GetChild(1);
        helpImg = helperTxt.gameObject.transform.GetChild(2);
        bigImg = helperTxt.gameObject.transform.GetChild(3);
        guideImg = helperTxt.gameObject.transform.GetChild(4);
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
            var bg = mainMenuCanvasParent.transform.GetChild(0);
            bg.GetComponent<UISpriteAnimation>().BGAnim();
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

    public void ShowMemory(MemoryType memType, out float time)
    {
        time = 3f;
        StartCoroutine(ShowMemoryImg(memoryTextures[(int)memType], time));
    }

    IEnumerator ShowMemoryImg(Texture img, float time)
    {
        memoryImg.enabled = true;
        memoryImg.texture = img;
        yield return new WaitForSeconds(time);
        memoryImg.enabled = false;
    }
}
