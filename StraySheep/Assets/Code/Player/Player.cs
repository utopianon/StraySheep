using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController charControl;

    [SerializeField] private float gravity;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        charControl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMovementUpdate()
    {
        charControl.ApplyGravity(gravity);
        charControl.ScrollForward(speed);
    }
}
