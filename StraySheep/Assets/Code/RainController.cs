using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{

    Camera mainCamera;
    float offsetX;
    public float angleMedium;
    public float angleFast;    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        offsetX = mainCamera.transform.position.x - transform.position.x;        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainCamera.transform.position.x + offsetX, transform.position.y, transform.position.z);
    }

    public void SetAngle (float speedMode)
    {
        switch (speedMode)
        {
            case 0:
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 0);
                break;
            case 1:
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, -angleMedium);
                break;
            case 2:
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, -angleFast);
                break;
            default:
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 0);
                break;
        }
    }
}
