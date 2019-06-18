using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{

    Camera mainCamera;
    float offsetX;
    public float angleMedium;
    public float angleFast;

    private Transform PSTransform;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        offsetX = mainCamera.transform.position.x - transform.position.x;
        PSTransform = transform.GetChild(0);

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(mainCamera.transform.position.x + offsetX, transform.position.y, transform.position.z);
    }

    public void SetAngle(float speedMode)
    {

        switch (speedMode)
        {
            case 0:
                PSTransform.eulerAngles = new Vector3(0, 0, 0);

                break;
            case 1:
                PSTransform.eulerAngles = new Vector3(0, 0, -angleMedium);

                break;
            case 2:
                PSTransform.eulerAngles = new Vector3(0, 0, -angleFast);

                break;
            default:

                PSTransform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }


    }



}
