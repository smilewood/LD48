using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float graityFactor;
    public GameObject HitPlayerSound, HitWallSound;
    private Vector3 velocity;
    Rigidbody2D rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Initialize(float speed, float arc, bool left)
    {
        velocity = new Vector3(speed * (left ? -1 : 1), arc, 0);
        if (left)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (velocity * Time.deltaTime));
        velocity += ((Vector3)Physics2D.gravity * graityFactor * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Instantiate(HitPlayerSound, transform.position, Quaternion.identity, transform.parent);
        }
        else
        {
            Instantiate(HitWallSound, transform.position, Quaternion.identity, transform.parent);
        }
        Destroy(this.gameObject);
    }
}
