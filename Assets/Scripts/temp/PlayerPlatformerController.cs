
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public bool canMoveLeft, canMoveRight, canJump;
    private GameObject respawnPoint;
    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded && canJump)
        {
            //animator.SetTrigger("Jump");
            StartCoroutine(Jump());
            currGravity = lowGravModifier;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            currGravity = gravityModifier;
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.05f) : (move.x < -0.05f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        animator.SetBool("Moving", (Math.Abs(move.x) > 0.05f));
        //animator.SetBool("Grounded", grounded);

        targetVelocity = move * maxSpeed;
    }

    IEnumerator Jump()
    {
        //GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(.07f);

        if (grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
    }
    //private void OnTriggerEnter2D( Collider2D other )
    //{
    //    if (other.gameObject.tag.Equals("EnviromentalDeath"))
    //    {
    //        this.transform.position = respawnPoint.transform.position;
    //    }
    //    if (other.gameObject.tag.Equals("RespawnPoint"))
    //    {
    //        this.respawnPoint = other.gameObject;
    //    }
    //}

}