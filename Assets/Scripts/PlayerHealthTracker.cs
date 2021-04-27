using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthTracker : MonoBehaviour
{
    public int Lives = 3;
    public float IFramesTime;
    public bool IFrames = false;
    private Animator animator;
    private PlayerSoundManager soundPlayer;

    private void Awake()
    {
        animator = this.GetComponentInChildren<Animator>();
        soundPlayer = GetComponentInChildren<PlayerSoundManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IFrames && collision.gameObject.CompareTag("Death"))
        {
            Lives -= 1;
            HealthUI.OnHealthChanged(Lives);

            if (Lives == 0)
            {
                animator.SetTrigger("Die");
                soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.Die);
            }
            else
            {
                Vector2 knockBackVector;
                if (collision.collider.GetType() == typeof(CompositeCollider2D)) // or TilemapCollider2D
                {
                    //Debug.Log("Static");
                    knockBackVector = new Vector2(collision.relativeVelocity.x, collision.relativeVelocity.y).normalized;
                }
                else
                {
                    //Debug.Log("Not Static");
                    knockBackVector = new Vector2(this.transform.position.x - collision.gameObject.transform.position.x, this.transform.position.y - collision.gameObject.transform.position.y).normalized;
                }
                //Vector3 contactPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
                //Vector3 knockBackVector = (this.transform.position - contactPoint).normalized;
                collision.otherRigidbody.AddForce(knockBackVector * 12, ForceMode2D.Impulse);
                StartCoroutine(IFrameCountdown());

                if (collision.collider.gameObject.GetComponent<Ghosty>())
                {
                    soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.HitGhost);
                }
                else
                {
                    soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.HitSpike);
                }
            }
        }
    }

    public void Spiked(GameObject spike)
    {
        Lives -= 1;
        HealthUI.OnHealthChanged(Lives);

        if (Lives == 0)
        {
            animator.SetTrigger("Die");
            soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.Die);
        }
        else
        {
            Vector2 knockBackVector = new Vector2(spike.gameObject.transform.position.x - this.transform.position.x, spike.gameObject.transform.position.y - this.transform.position.y).normalized;
            gameObject.GetComponent<Rigidbody2D>().AddForce(knockBackVector * 12, ForceMode2D.Impulse);
            StartCoroutine(IFrameCountdown());

            soundPlayer.PlaySound(PlayerSoundManager.PlayerSound.HitSpike);
        }
    }


    private IEnumerator IFrameCountdown()
    {
        IFrames = true;
        yield return new WaitForSeconds(IFramesTime);
        IFrames = false;
    }

}
