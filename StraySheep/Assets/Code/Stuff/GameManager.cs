using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class GameManager : MonoBehaviour
{
    const int CANVAS = 0, MAINMENU = 0, PAUSEMENU = 1, ENDMENU = 2, SCORE_TEXT = 2;
    const string SOUND_STR = "Sound", MUSIC_STR = "Music", SPEED_STR = "Speed", WIN_STR = "Victory", PAUSE_STR = "GamePause";

    public static GameManager GM;
    public CameraFollow levelCamera;

    // audio
    private float _soundNoice, _musicNoice;
    private Slider[] _sliders;
    private EventInstance _menuMusic, _levelMusic;
    private VCA _musicVCA, _soundVCA;

    // scoring
    [HideInInspector] public int currentScore, currentScene;
    [HideInInspector] public float distanceScore;//levelTimer, distance;

    // menu & UI
    private GameObject _mainMenu, _pauseMenu, _endMenu;

    private void Awake()
    {
        #region Singleton

        if (GM == null)
            GM = this;

        if (GM != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        #endregion
    }

    private void Start()
    {
        // sliders
        _sliders = GetComponentsInChildren<Slider>(includeInactive: true);
        _soundNoice = PlayerPrefs.GetFloat(SOUND_STR, 0.6f);
        UpdateSliders(false, SOUND_STR);
        _musicNoice = PlayerPrefs.GetFloat(MUSIC_STR, 0.4f);
        UpdateSliders(true, MUSIC_STR);

        // menu
        _mainMenu = transform.GetChild(CANVAS).GetChild(MAINMENU).gameObject;
        _pauseMenu = transform.GetChild(CANVAS).GetChild(PAUSEMENU).gameObject;
        _endMenu = transform.GetChild(CANVAS).GetChild(ENDMENU).gameObject;

        // music
        _menuMusic = RuntimeManager.CreateInstance("event:/Music/MainMenu/MainMenuSong");
        _levelMusic = RuntimeManager.CreateInstance("event:/Music/LevelTheme/LevelThemeViral");

        // vca (fmod)
        _musicVCA = RuntimeManager.GetVCA("vca:/Music");
        _soundVCA = RuntimeManager.GetVCA("vca:/PlayerSounds");

        if (SceneManager.GetActiveScene().buildIndex == MAINMENU)
        {
            _mainMenu.SetActive(true);
            _menuMusic.start();
        }
        else
        {
            _mainMenu.SetActive(false);
            _levelMusic.start();
        }
    }

    private void Update()
    {
        // pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_mainMenu.activeSelf || _endMenu.activeSelf)
            {
                // ignore
            }
            else
            {
                Pause();
            }
        }
    }

    #region Static methods

    public static Vector2 GetBoxCastSize(BoxCollider2D boxCol)
    {
        return new Vector2(boxCol.transform.localScale.x * boxCol.size.x, boxCol.transform.localScale.y * boxCol.size.y);
    }

    #endregion

    #region UI methods

    public void Pause()
    {
        Time.timeScale = 0f;
        _pauseMenu.SetActive(true);

        _levelMusic.setParameterValue(PAUSE_STR, 0);
    }

    public void EndScreen(bool win)
    {
        if (_endMenu.activeSelf) return;

        Time.timeScale = 0.75f;
        _endMenu.SetActive(true);
        levelCamera.DeathCamera();

        if (win)
        {
            _endMenu.transform.GetChild(SCORE_TEXT).GetComponent<TMPro.TextMeshProUGUI>().text = " " + currentScore;
            _levelMusic.setParameterValue(WIN_STR, 1);
        }
        else
        {
            _endMenu.transform.GetChild(SCORE_TEXT).GetComponent<TMPro.TextMeshProUGUI>().text = " D:";
            _levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    #endregion

    #region Buttons

    public void Continue()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        _endMenu.SetActive(false);

        _levelMusic.setParameterValue(PAUSE_STR, 1);
    }

    public void ReloadActiveScene()
    {       
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int buildIndex)
    {
        currentScene = buildIndex;
        SceneManager.LoadScene(buildIndex);

        if (buildIndex == MAINMENU)
        {
            _mainMenu.SetActive(true);
            _levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _menuMusic.start();
        }
        else
        {
            _mainMenu.SetActive(false);
            _menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _levelMusic.start();
        }

        _pauseMenu.SetActive(false);
        _endMenu.SetActive(false);

        Time.timeScale = 1;
        currentScore = 0;
    }

    public void LoadNextScene()
    {
        if (currentScene < 4)
        {
            LoadScene(currentScene + 1);
        }
        else
            LoadScene(0);        
    }

    public void ExitGame()
    {
        _menuMusic.release();
        _levelMusic.release();

        Application.Quit();
    }

    #endregion

    #region Sliders

    public void SoundVolume(Slider slider)
    {
        _soundNoice = slider.value;

        UpdateSliders(false, slider.name);

        PlayerPrefs.SetFloat(SOUND_STR, _soundNoice);

        // TODO: set volume to FMOD VDA

    }

    public void MusicVolume(Slider slider)
    {
        _musicNoice = slider.value;

        UpdateSliders(true, slider.name);

        PlayerPrefs.SetFloat(MUSIC_STR, _musicNoice);

        _musicVCA.setVolume(_musicNoice);
    }

    private void UpdateSliders(bool isMusic, string name)
    {
        foreach (Slider s in _sliders)
        {
            // music & "Music"
            if (isMusic && s.name == name)
                s.value = _musicNoice;
            // not music & "Sound"
            else if (!isMusic && s.name == name)
                s.value = _soundNoice;
        }
    }

    #endregion

    #region UI sounds

    public void PlayBackwardsButton()
    {
        RuntimeManager.PlayOneShot("event:/MainMenu/MenuSelectBack", transform.position);
    }

    public void PlayForwardsButton()
    {
        RuntimeManager.PlayOneShot("event:/MainMenu/MenuSelectIn", transform.position);
    }

    public void PlaySliderSound()
    {
        RuntimeManager.PlayOneShot("event:/MainMenu/Scroll", transform.position);
    }

    public void UpdateMusicSpeed(int playerSpeed)
    {
        _levelMusic.setParameterValue(SPEED_STR, playerSpeed * 0.5f);
    }

    #endregion
}
