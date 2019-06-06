using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const int CANVAS = 0, MAINMENU = 0, PAUSEMENU = 1, ENDMENU = 2, SCORE_TEXT = 2;

    public static GameManager GM;

    // audio
    private float _soundNoice, _musicNoice;
    private Slider[] _sliders;

    // scoring
    [HideInInspector] public int currentScore;
    //[HideInInspector] public float levelTimer, distance;

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
        _sliders = GetComponentsInChildren<Slider>(includeInactive: true);

        _mainMenu = transform.GetChild(CANVAS).GetChild(MAINMENU).gameObject;
        _pauseMenu = transform.GetChild(CANVAS).GetChild(PAUSEMENU).gameObject;
        _endMenu = transform.GetChild(CANVAS).GetChild(ENDMENU).gameObject;

        _mainMenu.SetActive(SceneManager.GetActiveScene().buildIndex == MAINMENU);
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

        // TODO: muffle level music
    }

    public void EndScreen()
    {
        _endMenu.SetActive(true);
        _endMenu.transform.GetChild(SCORE_TEXT).GetComponent<TMPro.TextMeshProUGUI>().text = " " + currentScore;

        // TODO: level end music
    }

    #endregion

    #region Buttons

    public void Continue()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);

        // TODO: unmuffle level music
    }

    public void ReloadActiveScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);

        if (buildIndex == MAINMENU)
        {
            _mainMenu.SetActive(true);
            // TODO: play menu music
        }
        else
        {
            _mainMenu.SetActive(false);
            // TODO: restart level music ?
            // + we can play different level musics if wanted
        }
        _pauseMenu.SetActive(false);
        _endMenu.SetActive(false);

        currentScore = 0;
    }

    public void ExitGame()
    {
        // TODO: Clear FMOD cache

        Application.Quit();
    }

    #endregion

    #region Sliders

    public void SoundVolume(Slider slider)
    {
        _soundNoice = slider.value;

        UpdateSliders(false, slider.name);

        // TODO: set volume to FMOD VDA
    }

    public void MusicVolume(Slider slider)
    {
        _musicNoice = slider.value;

        UpdateSliders(true, slider.name);

        // TODO: set volume to FMOD VDA
    }

    private void UpdateSliders(bool isMusic, string name)
    {
        foreach(Slider s in _sliders)
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
}
