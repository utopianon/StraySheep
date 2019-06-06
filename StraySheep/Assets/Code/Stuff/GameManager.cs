using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    // audio
    private float _soundNoice, _musicNoice;
    private Slider[] _sliders;

    // scoring
    [HideInInspector] public int currentScore, maxScore;
    [HideInInspector] public float levelTimer, distance;

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
        _sliders = FindObjectsOfType<Slider>();

        // make fmod event and set parameter value(s)
        levelMusic = RuntimeManager.CreateInstance("event:/Music/LevelTheme/LevelThemeViral");
        levelMusic.setParameterValue("GamePause", 1);
        levelMusic.start();
    }

    FMOD.Studio.EventInstance levelMusic;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // setting parameter value used to filter music
            levelMusic.setParameterValue("GamePause", 0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

    #region Static methods

    public static Vector2 GetBoxCastSize(BoxCollider2D boxCol)
    {
        return new Vector2(boxCol.transform.localScale.x * boxCol.size.x, boxCol.transform.localScale.y * boxCol.size.y);
    }

    #endregion

    #region Buttons

    public void Continue()
    {
        Debug.Log("TODO continue");
    }

    public void ReloadActiveScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int buildIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

    #region Sliders

    public void SoundVolume(Slider slider)
    {
        _soundNoice = slider.value;

        UpdateSliders(false, slider.name);
    }

    public void MusicVolume(Slider slider)
    {
        _musicNoice = slider.value;

        UpdateSliders(true, slider.name);
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
