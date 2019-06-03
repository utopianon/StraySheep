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

    private float gravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;
    private Vector3 velocity;
    float movementSmoothing;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        //calculate gravity and velocity from wanted jump height and time to peak
        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpPeak, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (controller.collisions.below)
            velocity.y = maxJumpVelocity;
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeInAir);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
