using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    const float groundedRadius = .2f;
    private bool grounded;
    private Rigidbody2D rigidBody;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

   public void ScrollForward(float speed)
    {

    }

    public void Jump()
    {

    }

   public void ApplyGravity(float gravity)
    {

    }
}
