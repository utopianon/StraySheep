using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Debug")]
    public bool stationary;
    public bool variableJumping;

    CharacterController controller;
    Animator anim;

    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpDist = 2;
    [SerializeField] private float maxJumpDist = 6;
    [SerializeField] private float timeToJumpPeak = .4f;
    [SerializeField] private float accelerationTimeInAir = .2f;
    [SerializeField] private float accelerationTimeGrounded = .1f;
    [SerializeField] private float minimumVelocity = 4;
    [SerializeField] private float maximumVelocity = 12;

    private float gravity;
    private float baseGravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;
    private Vector3 velocity;
    float movementSmoothing;

    // casting stuff
    Vector2 castSize;

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
        baseGravity = gravity;       

      
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);

        castSize = GameManager.GetBoxCastSize(GetComponent<BoxCollider2D>());
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

        if (variableJumping)
            VariableJumping();
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (controller.collisions.below)
                    FastFallingJump();
            }
        }

    }

    void FixedUpdate()
    {
        //for test
        if (!stationary)
            RunnerMovement();

        velocity.y += gravity * Time.fixedDeltaTime;

        DoDangerAndPickup();

        controller.Move(velocity * Time.fixedDeltaTime);
    }

    void StandartMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeInAir);

    }

    void VariableJumping()
    {
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

    void FastFallingJump()
    {
        //calculate gravity and velocity from wanted jump height and distance
        maxJumpVelocity = (2 * (maxJumpHeight * velocity.x))/(maxJumpDist/2);
        gravity = (-2 * maxJumpHeight * Mathf.Pow(velocity.x,2)) / Mathf.Pow((maxJumpDist / 2),2);
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
        velocity.y = maxJumpVelocity;

    }

    public float TimeToJumpPeak()
    {
        return (maxJumpDist / 2) / velocity.x;
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

    public void DoDangerAndPickup()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, castSize, transform.eulerAngles.z, velocity.normalized, velocity.magnitude * Time.fixedDeltaTime);

        bool died = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Pickup pickup = hits[i].collider.GetComponent<Pickup>();
            if (pickup != null)
            {
                GameManager.GM.currentScore += pickup.score;
                pickup.Die();
            }

            Danger danger = hits[i].collider.GetComponent<Danger>();
            if (danger != null)
            {
                died = true;
            }
        }

        if (died) enabled = false;
    }

}
