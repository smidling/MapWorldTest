using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuCtrl : MonoBehaviour
{
    public static SettingsMenuCtrl Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    [SerializeField]
    private Toggle musicToggle;
    [SerializeField]
    private Toggle soundsToggle;
    [SerializeField]
    private Toggle debugToggle;
    [SerializeField]
    private Text levelSizeText;
    [SerializeField]
    private Slider levelSizeSlider;
    [SerializeField]
    private Text obstaclesText;
    [SerializeField]
    private Slider obstaclesSlider;
    [SerializeField]
    private Button buttonMenu;

    private bool debugEnabled = false;

    [SerializeField]
    private AudioMixer GameAudioMixer;

    void Start()
    {
        buttonMenu.onClick.AddListener(delegate { ButtonClickedMenu(); });
        musicToggle.onValueChanged.AddListener(delegate { MuteMusic(); });
        debugToggle.onValueChanged.AddListener(delegate { DebugToggle(); });
        soundsToggle.onValueChanged.AddListener(delegate { MuteSounds(); });
        levelSizeSlider.onValueChanged.AddListener(delegate { LevelSizeChanged(); });
        obstaclesSlider.onValueChanged.AddListener(delegate { HouseCountChanged(); });
        LevelSizeChanged();
        HouseCountChanged();
    }

    void OnEnable()
    {
        debugEnabled = debugToggle.isOn;
        musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        soundsToggle.isOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        levelSizeSlider.value = PlayerPrefs.GetFloat("LevelSize", 0);
        obstaclesSlider.value = PlayerPrefs.GetFloat("House", 0.5f);
    }
    

    public void ButtonClickedMenu()
    {
        // go to menu
        GameUiOverlord.Instance.MainMenu();
    }

    public void RefreshSettings()
    {
        levelSizeSlider.value = PlayerPrefs.GetFloat("LevelSize", 0f);
        obstaclesSlider.value = PlayerPrefs.GetFloat("House", 0.5f);
        if (debugEnabled)
        {
            int levelSize = (int)(LevelGenerator.minLevelSize 
                + levelSizeSlider.value * (LevelGenerator.maxLevelSize - LevelGenerator.minLevelSize));
            LevelGenerator.Instance.SetLevelParams(
                levelSize, levelSize, (int)(obstaclesSlider.value * 100));
        }
        else
        {
            LevelGenerator.Instance.SetLevelParams(
                GetMyMapData.Instance.MapWidth, GetMyMapData.Instance.MapHeight,
                GetMyMapData.Instance.HousesNum);
        }
    }



    public void LevelSizeChanged()
    {
        PlayerPrefs.SetFloat("LevelSize", levelSizeSlider.value) ; 
        levelSizeText.text = (LevelGenerator.minLevelSize 
            + levelSizeSlider.value*(LevelGenerator.maxLevelSize - LevelGenerator.minLevelSize)).ToString("F0");
    }
    public void HouseCountChanged()
    {
        PlayerPrefs.SetFloat("House", obstaclesSlider.value);
        obstaclesText.text = (obstaclesSlider.value * 100).ToString("F0");
    }


    public void MuteMusic()
    {
        if (musicToggle.isOn)
        {
            PlayerPrefs.SetInt("MusicOn", 1);
            GameAudioMixer.SetFloat("MUSIC_vol", 0f);
        }
        else
        {
            PlayerPrefs.SetInt("MusicOn", 0);
            GameAudioMixer.SetFloat("MUSIC_vol", -80f);
        }
    }

    public void DebugToggle()
    {
        debugEnabled = debugToggle.isOn;
    }

    public void MuteSounds()
    {
        if (soundsToggle.isOn)
        {
            PlayerPrefs.SetInt("SoundOn", 1);
            GameAudioMixer.SetFloat("SFX_vol", 0f);
        }
        else
        {
            PlayerPrefs.SetInt("SoundOn", 0);
            GameAudioMixer.SetFloat("SFX_vol", -80f);
        }
    }
}
