using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
