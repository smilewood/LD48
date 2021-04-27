using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float deployTime, retractTime;
    public Sprite retracted, mid, deploy;

    private SpriteRenderer _renderer;
    private BoxCollider2D _collider2D;
    private PlayerHealthTracker player;
    private bool deploying = false;

    public AudioSource sound;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<BoxCollider2D>();
        _renderer.sprite = retracted;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!deploying)
            {

                deploying = true;
                player = collision.gameObject.GetComponent<PlayerHealthTracker>();
            
                StartCoroutine(DeploySpikes());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }

    private IEnumerator DeploySpikes()
    {
        _renderer.sprite = mid;
        yield return new WaitForSeconds(deployTime);
        _renderer.sprite = deploy;
        
        if(player != null)
        {
            player.Spiked(this.gameObject);
        }
        _collider2D.isTrigger = false;
        sound.Play();

        yield return new WaitForSeconds(retractTime);
        _renderer.sprite = deploy;
        _collider2D.isTrigger = true;
        yield return new WaitForSeconds(.1f);
        _renderer.sprite = retracted;

        deploying = false;
    }

}
