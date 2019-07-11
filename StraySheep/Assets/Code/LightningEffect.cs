using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;


public class LightningEffect : MonoBehaviour
{
    public float lightnFreqMin, lightnFreqMax;
    public float lightnLenghtMin, lightnLenghtMax;
    public AudioSource thunder;

    private PostProcessingBehaviour PP;
    private float timeSinceLastLight;
    private bool isLightn;
    private float timeForNextLight, lenghtForNextLight;

    private void Start()
    {
        thunder = GetComponent<AudioSource>();
        PP = GetComponent<PostProcessingBehaviour>();
        timeForNextLight = Random.Range(lightnFreqMin, lightnFreqMax);
        isLightn = false;
    }

    private void Update()
    {
        if (!isLightn)
        {
            if (timeSinceLastLight < timeForNextLight)
            {
                timeSinceLastLight += Time.deltaTime;
            }
            else
            {
                isLightn = true;
                lenghtForNextLight = Random.Range(lightnLenghtMin, lightnLenghtMax);
                StartCoroutine(LightningStrike());
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (!PP)
            PP = GetComponent<PostProcessingBehaviour>();
        PP.profile.colorGrading.enabled = false;
        timeForNextLight = Random.Range(lightnFreqMin, lightnFreqMax);
        isLightn = false;
    }

    IEnumerator LightningStrike()
    {
        float timer = 0;
        PP.profile.colorGrading.enabled = true;
        ColorGradingModel.Settings PPb = PP.profile.colorGrading.settings;
        thunder.Play(0);
        while (timer <= lenghtForNextLight)
        {
            PPb.basic.postExposure = Mathf.Lerp(-4, 0, (timer / lenghtForNextLight));
            PP.profile.colorGrading.settings = PPb;
            timer += Time.deltaTime;
            yield return null;
        }
        timeSinceLastLight = 0;
        PP.profile.colorGrading.enabled = false;
        isLightn = false;
        timeForNextLight = Random.Range(lightnFreqMin, lightnFreqMax);
    }
}
