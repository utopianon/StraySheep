using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    CharacterController controller;
    Animator anim;

    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float timeToJumpPeak = .4f;
    [SerializeField] private float accelerationTimeInAir = .2f;
    [SerializeField] private float accelerationTimeGrounded = .1f;
    [SerializeField] private float minimumVelocity = 4;
    [SerializeField] private float maximumVelocity = 12;

    private float gravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;
    private Vector3 velocity;
    float movementSmoothing;

    enum SpeedLevel { slow, medium, fast };
    SpeedLevel speedLevel;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        //starts with medium speed
        speedLevel = SpeedLevel.medium;
        //calculate gravity and velocity from wanted jump height and time to peak
        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpPeak, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    // Update is called once per frame
    private void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (speedLevel < SpeedLevel.fast)
            {
                speedLevel++;
                
            }
        }

        //speeding up
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            speedLevel = SpeedLevel.medium;            
        }

        //speeding down
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (speedLevel > SpeedLevel.slow)
            {
                speedLevel--;               
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (controller.collisions.below)
                velocity.y = maxJumpVelocity;
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }

    }
    void FixedUpdate()
    {
        RunnerMovement();

        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    void StandartMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeInAir);

    }

    void SpeedUp()
    {

    }

    void SpeedDown()
    {

    }

    void RunnerMovement()
    {
        float targetVelocityX;

        switch (speedLevel)
        {
            case SpeedLevel.slow:
                targetVelocityX = minimumVelocity;
                break;
            case SpeedLevel.medium:
                targetVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
            case SpeedLevel.fast:
                targetVelocityX = maximumVelocity;
                break;
            default:
                targetVelocityX = velocity.x;
                break;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeInAir);
    }

}
