using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public List<PressurePlate> triggers;
    public float arrowSpeed, arc;
    public GameObject arrowPrefab;
    private AudioSource source;
    public float cooldown;
    private bool canFire = true;

    public Sprite left, right;
    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        foreach (PressurePlate p in triggers)
        {
            p.OnTriggered += Fire;
        }
        if (triggers.Any())
        {
            _renderer.sprite = triggers.First().LeftOfTrap ? left : right;
        }
        source = GetComponent<AudioSource>();
    }

    public void Fire(bool Left)
    {
        if (canFire)
        {
            _renderer.sprite = Left ? left : right;
            ArrowMove arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity, transform).GetComponent<ArrowMove>();
            arrow.Initialize(arrowSpeed, arc, Left);
            source.Play();
            StartCoroutine(Cooldown(cooldown));
        }
    }

    private IEnumerator Cooldown(float time)
    {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
