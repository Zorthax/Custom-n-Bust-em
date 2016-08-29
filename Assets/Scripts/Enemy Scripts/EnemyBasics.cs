﻿using UnityEngine;
using System.Collections;

public class EnemyBasics : MonoBehaviour {

	public float hpTotal;
	float hp;
	float stunTime;

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

	Texture2D red;
	Texture2D darkRed;

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

		red = new Texture2D(1, 1);
		red.SetPixel (0, 0, new Color(1, 0, 0, 1));
		red.Apply ();

		darkRed = new Texture2D(1, 1);
		darkRed.SetPixel (0, 0, new Color(0.5f, 0, 0, 1));
		darkRed.Apply ();

		hp = hpTotal;
    }

	void FixedUpdate()
	{
		//On death
		if (hp <= 0) {
			Instantiate (Resources.Load ("DeathSmoke"), transform.position, new Quaternion (0, 0, 0, 0));
			Destroy (gameObject);
		}

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

		if (stunTime > 0)
			stunTime -= Time.deltaTime;
        y = rb.velocity.y;

        if (!stunned) rb.velocity = new Vector2(x * walkSpeed, y);

        Vector3 ls = transform.localScale;
        if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        if (!stunned) { if (x != 0) SetSprite(runSprite); else SetSprite(idleSprite); }

        if (stunned)
        {
            rb.velocity = new Vector2(rb.velocity.x / 1.1f, y);
			if (stunTime <= 0)
				stunned = false;
            if (x > 0) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
            if (x < 0) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        }



        Animation();

    }

	public void ApplyHit(Vector2 knockback, float damage, float stun)
    {
		if (stunTime <= 0) {
			stunned = true;
			rb.velocity = knockback;
			currentSprite = knockbackSprite;
			spriteIndex = 0;
			hp -= damage;
			stunTime = stun;
		}


    }

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (Camera.main.WorldToScreenPoint(transform.position).x - 25, 
			(Screen.height - Camera.main.WorldToScreenPoint(transform.position).y) -30, 50, 5), darkRed);

		GUI.DrawTexture (new Rect (Camera.main.WorldToScreenPoint(transform.position).x - 25, 
			(Screen.height - Camera.main.WorldToScreenPoint(transform.position).y) -30, 50 / (hpTotal / hp), 5), red);
		

	}

	public void DrawHealth()
	{

	}
}
