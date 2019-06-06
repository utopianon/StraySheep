using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Controller options")]
    public bool platformerMovement;
    public bool variableJumping;


    CharacterController controller;
    Animator anim;

    [Space(10f)]
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float medJumpHeight = 1;
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpDist = 2;
    [SerializeField] private float medJumpDist = 2;
    [SerializeField] private float maxJumpDist = 6;
    [SerializeField] private float timeToJumpPeak = .4f;
    [SerializeField] private float jumpFallModifier = 2;
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
    bool died = false;

    // casting stuff
    Vector2 castSize;

    enum SpeedLevel { slow, medium, fast };
    SpeedLevel speedLevel;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        //starts with medium speed
        speedLevel = SpeedLevel.slow;
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
        Debug.Log("grounded is " + controller.collisions.below);
        if (!died)
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
                {
                    FastFallingJump();
                }
            }
            if (variableJumping)
            {
                if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    if (velocity.y > (maxJumpVelocity / 2))
                        velocity.y = (maxJumpVelocity / 2);
                }
            }
        }
    }

    void FixedUpdate()
    {
        //for test
        if (!platformerMovement)
            RunnerMovement();
        else
            StandartMovement();

        velocity.y += gravity * Time.deltaTime;

        DoDangerAndPickup();

        controller.Move(velocity * Time.deltaTime);

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
        float jumpVelocityX;
        float jumpHeight;
        float jumpDist;

        switch (speedLevel)
        {
            case SpeedLevel.slow:
                jumpHeight = minJumpHeight;
                jumpDist = minJumpDist;
                jumpVelocityX = minimumVelocity;
                break;
            case SpeedLevel.medium:
                jumpHeight = medJumpHeight;
                jumpDist = medJumpDist;
                jumpVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
            case SpeedLevel.fast:
                jumpHeight = maxJumpHeight;
                jumpDist = maxJumpDist;
                jumpVelocityX = maximumVelocity;
                break;
            default:
                jumpHeight = (minJumpHeight + maxJumpHeight) / 2;
                jumpDist = (minJumpDist + minJumpDist) / 2;
                jumpVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
        }

        maxJumpVelocity = (2 * (jumpHeight * jumpVelocityX) / (jumpDist / 2));
        gravity = (-2 * jumpHeight * Mathf.Pow(jumpVelocityX, 2)) / Mathf.Pow((jumpDist / 2), 2);
        baseGravity = gravity;
        timeToJumpPeak = (jumpDist / 2) / jumpVelocityX;
        velocity.y = maxJumpVelocity;
        StartCoroutine(DoFallJump());

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

        if (died)
        {
            //enabled = false;
            controller.colliding = false;
            GameManager.GM.EndScreen();
        }
    }

    private IEnumerator DoFallJump()
    {

        while (velocity.y > 0)
        {
            yield return null;
        }
        gravity = gravity * jumpFallModifier;
        while (!controller.collisions.below)
        {
            yield return null;

        }
        gravity = baseGravity;
        yield return null;
    }

}
