using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghosty : MonoBehaviour, INeedsinitialization, IPlayerInBone
{
    private Vector2 upperBounds, lowerBounds;
    private GameObject player;
    public float speed;
    public float fadeOutTime = 1;
    private SpriteRenderer _renderer;
    private bool run = false;

    public void Init(Vector2 bone, bool vert)
    {
        float minX = bone.x * 32;
        float maxX = (bone.x * 32) + (32 * (vert ? 1 : 2));

        float minY = (bone.y * 32) - (32 * (vert ? 2 : 1));
        float maxY = bone.y * 32;

        upperBounds = new Vector2(maxX, maxY);
        lowerBounds = new Vector2(minX, minY);
    }


    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }
    bool fading = false;
    // Update is called once per frame
    void Update()
    {
        if (!run)
        {
            return;
        }
        Vector3 move = Vector3.Normalize(player.transform.position - this.transform.position) * speed * Time.deltaTime;
        Vector3 temp = this.transform.position + move;
        if(!fading && (temp.x < lowerBounds.x || temp.x > upperBounds.x || temp.y < lowerBounds.y || temp.y > upperBounds.y))
        {
            StartCoroutine(FadeOut());
            fading = true;
        }
        

        this.transform.position = temp;
    }

    private IEnumerator FadeOut()
    {
        float counter = 0;
        while(counter < fadeOutTime)
        {
            Color temp = _renderer.color;
            temp.a = Mathf.Lerp(1, 0, counter / fadeOutTime);
            _renderer.color = temp;
            counter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }

    public void PlayerInBone(GameObject player)
    {
        run = true;
        this.player = player;
    }
}
