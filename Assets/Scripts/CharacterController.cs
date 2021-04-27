using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Public Variables
    public float JumpForce = 16;
    public float CrouchingJumpForce = 12;
    public float JumpControlLength = 0.5f;
    public float JumpInitialAmount = 0.3f;
    public float Speed = 10;
    public float Smoothing = 5f;
    public bool Grounded = false;
    public float CollisionCheckRadius = 1.0f;
    public float CollisionCrouchRadius = 1.0f;
    public bool Crouching = false;
    public Collider2D fullHeightCollider;
    public float CrouchingSpeed = 6f;

    public GameObject JumpGameObject;
    public GameObject ImpactGameObject;

    public Vector2 groundOffset = new Vector2(0, -0.51f);
    public Vector2 headOffset = new Vector2(0, 2);
    public Vector2 bodyOffset = new Vector2(0, 2);

    // Control Variables
    private Vector2 velocity;
    private bool facingRight = true;
    private float currentJumpTimer = 0f;
    private bool shouldTriggerImpact = false;

    // Input Variables
    private float horizontalInput = 0.0f;
    private bool jumpInput = false;
    private bool crouchInput = false;

    // Internal Variables
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D currentRigidbody;
    private ContactFilter2D contactFilter;
    private Animator animator;
    private PlayerSoundManager soundPlayer;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        currentRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        soundPlayer = GetComponentInChildren<PlayerSoundManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetButton("Jump");
        crouchInput = Input.GetButton("Crouch");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector2(this.transform.position.x, this.transform.position.y) + groundOffset, CollisionCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(this.transform.position.x, this.transform.position.y) + headOffset, CollisionCrouchRadius);
        Gizmos.DrawSphere(new Vector2(this.transform.position.x, this.transform.position.y) + bodyOffset, CollisionCrouchRadius);
    }

    private void UpdateGrounded()
    {
        bool lastGrounded = false;
        Grounded = false;
        List<Collider2D> results = new List<Collider2D>();
        int resultCount = Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y) + groundOffset, CollisionCheckRadius, contactFilter, results);
        for (int i = 0; i < resultCount; i++)
        {
            if (results[i].gameObject != gameObject)
            {
                Grounded = true;
                if (!lastGrounded)
                {
                    currentJumpTimer = 0f;
                    if (shouldTriggerImpact)
                    {
                        shouldTriggerImpact = false;
                        Instantiate(ImpactGameObject, this.transform.position, Quaternion.identity);
                        soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.Land);
                    }
                }
                // In the future you should be able to remove this break if you wana add goomba stomping :o (But make sure to only play the sound once)
                break;
            }
        }
        shouldTriggerImpact = shouldTriggerImpact || currentRigidbody.velocity.y < -8f;
    }

    // Fixed update is called on every physics update
    private void FixedUpdate()
    {
        UpdateGrounded();

        Move(horizontalInput, jumpInput, crouchInput);

        currentRigidbody.AddForce(Physics2D.gravity, ForceMode2D.Force);
    }

    void Move(float horizontalIn, bool jumpIn, bool crouchIn)
    {

        // Handle Crouching
        if (!crouchIn && Crouching)
        {
            // We (shouldn't) don't have to worry about the collider hitting the player because they will have the smaller collider
            List<Collider2D> results = new List<Collider2D>();
            if (Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y) + headOffset, CollisionCrouchRadius, contactFilter, results) > 0 ||
                Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y) + bodyOffset, CollisionCrouchRadius, contactFilter, results) > 1)
            {
                crouchIn = true;
            }
        }

        if (crouchIn)
        {
            fullHeightCollider.enabled = false;
            Crouching = true;
        }
        else
        {
            fullHeightCollider.enabled = true;
            Crouching = false;
        }

        // Handle Movement

        float goalSpeed = horizontalIn * (Crouching ? CrouchingSpeed : Speed);

        if (Math.Abs(horizontalIn) <= 0.05f && !Grounded)
        {
            goalSpeed = currentRigidbody.velocity.x;
        }

        float horizontalSpeed = Mathf.Lerp(currentRigidbody.velocity.x, goalSpeed, Time.deltaTime * Smoothing);

        currentRigidbody.velocity = new Vector3(horizontalSpeed, Mathf.Clamp(currentRigidbody.velocity.y, -16, 16), 0);

        if (Grounded && jumpIn && currentJumpTimer <= 0 && currentRigidbody.velocity.y <= 1f)
        {
            currentJumpTimer = JumpControlLength;
            currentRigidbody.AddForce(new Vector2(0f, (Crouching ? CrouchingJumpForce : JumpForce) * JumpInitialAmount), ForceMode2D.Impulse);
            Instantiate(JumpGameObject, this.transform.position, Quaternion.identity);
            soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.Jump);
        }

        // Handle Jump

        if (jumpIn && currentJumpTimer > 0)
        {
            currentJumpTimer = currentJumpTimer - Time.deltaTime;
            currentRigidbody.AddForce(new Vector2(0f, (((Crouching ? CrouchingJumpForce : JumpForce) * (1 - JumpInitialAmount)) / JumpControlLength) * Time.deltaTime), ForceMode2D.Impulse);
        }
        else if (!jumpIn && currentJumpTimer > 0)
        {
            currentJumpTimer = 0;
        }

        // Handle Animations

        bool moving = Math.Abs(horizontalIn) > 0.05f && Math.Abs(horizontalSpeed) > 1f;

        if (moving && Grounded)
        {
            soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.Footsteps);
        }

        animator.SetBool("Moving", moving);
        animator.SetBool("Crouching", Crouching);
        animator.SetBool("Jumping", currentRigidbody.velocity.y > 0 && !Grounded);
        animator.SetBool("Falling", currentRigidbody.velocity.y < 0 && !Grounded);

        if ((facingRight && horizontalIn < 0) || (!facingRight && horizontalIn > 0))
        {
            FlipSprite();
        }
    }

    void FlipSprite()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
