using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


public class LightningEffect : MonoBehaviour
{
    public float lightnFreqMin, lightnFreqMax;
    public float lightnLenghtMin, lightnLenghtMax;

    private PostProcessingBehaviour PP;
    private float timeSinceLastLight;
    private bool isLightn;
    private float timeForNextLight, lenghtForNextLight;

    private void Start()
    {
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

    IEnumerator LightningStrike()
    {
        float timer = 0;
        PP.profile.colorGrading.enabled = true;
        ColorGradingModel.Settings PPb = PP.profile.colorGrading.settings;
        while (timer <= lenghtForNextLight)
        {
            PPb.basic.postExposure = Mathf.Lerp(-4,0,(timer/lenghtForNextLight));
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
