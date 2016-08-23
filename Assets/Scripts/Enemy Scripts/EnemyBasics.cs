using UnityEngine;
using System.Collections;

public class EnemyBasics : MonoBehaviour {

    public bool basicWalking = true;
    public float walkSpeed = 4;
    public float walkRarity = 30;

    public float sightRadius = 10;

    public MySprite idleSprite;
    public MySprite runSprite;
    public MySprite knockbackSprite;

    Rigidbody2D rb;

    bool agro = false;
    bool grounded = true;
    bool stunned = true;
    float x = 0;
    float y = 0;

    //Animation stuff
    MySprite currentSprite;
    float spriteIndex = 0;
    SpriteRenderer renderor;
    void Animation()
    {
        renderor.sprite = currentSprite.sprites[Mathf.RoundToInt(spriteIndex)];
        if (spriteIndex < currentSprite.sprites.Length - 1) spriteIndex += currentSprite.speed;
        else if (currentSprite.loop) spriteIndex = 0;
    }
    void SetSprite(MySprite sprite)
    {
        if (currentSprite != sprite)    //Prevents getting stuck on the first frame
        {
            //Debug.Log("Changing sprite");
            currentSprite = sprite;
            spriteIndex = 0;
        }
    }
    [System.Serializable]
    public class MySprite
    {
        public bool loop;
        public float speed;
        public Sprite[] sprites;
    }

    // Use this for initialization
    void Start ()
    {
        currentSprite = idleSprite;
        rb = GetComponent<Rigidbody2D>();
        renderor = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (Random.Range(0, walkRarity) >= walkRarity - 2)
        {
            float i = Random.value;
            if (i < 0.3f)
            {
                x = -1;
            }
            else if (i < 0.6f)
            {
                x = 1;
            }
            else x = 0;
        }

        y = rb.velocity.y;

        if (!stunned) rb.velocity = new Vector2(x * walkSpeed, y);

        Vector3 ls = transform.localScale;
        if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        if (!stunned) { if (x != 0) SetSprite(runSprite); else SetSprite(idleSprite); }

        if (stunned)
        {
            rb.velocity = new Vector2(rb.velocity.x / 1.1f, y);
            if (Mathf.Abs(rb.velocity.x) < 0.1f && Mathf.Abs(rb.velocity.y) < 0.2f) stunned = false;
            if (x > 0) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
            if (x < 0) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        }

        Animation();

    }

    public void ApplyHit(Vector2 knockback)
    {
        stunned = true;
        rb.velocity = knockback;
        currentSprite = knockbackSprite;
        spriteIndex = 0;

    }
}
