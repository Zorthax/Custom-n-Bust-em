using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    [Header("Basics")]
	public float hpTotal;
	float hp;
    public float walkingSpeed = 5;
    public float jumpForce = 10;
    public float jumpMin = 2;
	public float jumpFrame = 2;
    public Vector3 cameraPosition;
	public float endLag;
	float stunTime;
    AttackScript atk;

	public float spTotal;
	float sp;
	public float shieldRechargeDelay;
	public float shieldBreakDelay;
	public float shieldThickness;
	float shieldDelay;
	bool shieldBroken;

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

    static Controls con;
    Rigidbody2D rb;
    BoxCollider2D col;
    
    bool onSlope;
    bool canJump;
	bool shielding;
    public bool action;
    float xMovement;
    float yMovement;
    bool slowDown = false;
	PlayerSprites sprites;
    

    // Use this for initialization
    void Start ()
    {
        con = GameObject.FindGameObjectWithTag("Controls").GetComponent<Controls>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        atk = GetComponentInChildren<AttackScript>();
        renderor = GetComponent<SpriteRenderer>();
		sprites = GetComponent<PlayerSprites> ();
        currentSprite = sprites.idleSprites;
        action = false;
		shielding = false;
		hp = hpTotal; 
		sp = spTotal;
		shieldDelay = 0;
    }

	void FixedUpdate()
	{
		if (hp > hpTotal)
			hp = hpTotal;
		if (sp > spTotal)
			sp = spTotal;
	}
	void Update ()
    {
		if (!action)
			CheckShield ();

		if (stunTime <= 0) {

			if (!action) 
			{
				AttackControl ();

			}
			GroundCheck ();

			xMovement = CalculateXMovement ();
			yMovement = CalculateYMovement ();
			EnemyCheck ();

			//Apply velocity
			if (!action)
				rb.velocity = new Vector2 (xMovement * walkingSpeed, yMovement);
		} else {
			stunTime -= Time.deltaTime;
			SetSprite (sprites.knockbackSprites);
		}
        //Keep camera looking at player
        Camera.main.transform.position = transform.position + cameraPosition;
        if (!action) Animation();
		sprites.SetStats (hpTotal, hp, spTotal, sp);
	}

    void GroundCheck()
    {
        onSlope = false; //will apply extra gravity for smooth movement on slopes
        canJump = false;
        col.isTrigger = false;

        //Receive list of every collision
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position - new Vector3(0, 0.5f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.02f); 

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
			if (h.transform.tag == "Passable")
            {
                canJump = true;
				if (Input.GetKey(con.down) && yMovement <= 0) //Jump down from platform
                {
                    col.isTrigger = true;
                    transform.position -= new Vector3(0, 0.1f, 0);
                    canJump = false;
                }
            }
			if (h.transform.tag != "Player" && h.transform.tag != "NPC" && h.transform.tag != "Slope" && h.transform.tag != "Passable" && h.transform.tag != "Enemy")
            {
                canJump = true;
                break;
            }
        }

        SlopeCheck();

		if (currentSprite == sprites.jumpSprites && spriteIndex < jumpFrame)
			canJump = false;
    }

    void SlopeCheck()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position - new Vector3(0, 0.5f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.03f);

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
            if (h.transform.tag == "Slope")
            {
                onSlope = true;
                canJump = true;
                break;
            }
        }
    }
    float CalculateXMovement()
    {
        float x = 0;
		if (shielding && !canJump)
			x = xMovement;

        //Horizontal movement
        if (Input.GetAxis("Horizontal") != 0 && (!action && !shielding)) //Controller movement takes priority over keyboard
        { x = Input.GetAxis("Horizontal"); }
		else if (!action && !shielding) //keyboard movement
        {
            if (Input.GetKey(con.left)) x -= 1;
            if (Input.GetKey(con.right)) x += 1;
        }

        //Flip sprite to face direction of movement
        Vector3 ls = transform.localScale;
        if (x < 0 && !action) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (x > 0 && !action) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
		if (!action && !shielding && canJump) { if (x != 0) SetSprite(sprites.runSprites); else SetSprite(sprites.idleSprites); }

		if (slowDown) x /= 3;
		if (shielding) x /= 1.02f;
        return x;
    }

    float CalculateYMovement()
    {
        float y = rb.velocity.y;

		//Smooth jump animations
        if (!action && canJump && (Input.GetKeyDown(con.jump) || Input.GetKeyDown(con.jumpALT)))
        {
			SetSprite(sprites.jumpSprites);
            if ((Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
            { y = jumpMin; }
        }
		if (!action && !canJump && currentSprite == sprites.jumpSprites) 
		{
            if (Mathf.RoundToInt(spriteIndex) == jumpFrame && rb.velocity.y < jumpMin / 2)
            { y = jumpForce; }
            else if (Mathf.RoundToInt(spriteIndex) >= sprites.jumpSprites.sprites.Length)
                SetSprite(sprites.airSprites);
            //Hold to jump higher
            if ((Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)) && (y == 0))
            { y = jumpMin; }

        }
		//Hold to jump higher
        if (y > jumpMin && (Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
        { y = jumpMin; canJump = false; }

		if (onSlope) y -= 0.5f;//Smooth slope walking
        if (!action && !shielding && !onSlope && y > 0.1f && canJump) SetSprite(sprites.airSprites); //Prevents animation change when jumping up platforms
		if (!action && !shielding &&  !canJump && currentSprite != sprites.jumpSprites) SetSprite(sprites.airSprites); //Set air sprite when falling

        return y;
    }

    void AttackControl()
    {
		float yInput = 0;
		if (Input.GetAxis("Vertical") != 0) //Controller movement takes priority over keyboard
		{ yInput = Input.GetAxis("Vertical"); }
		else //keyboard movement
		{
			if (Input.GetKey(con.down)) yInput -= 1;
			if (Input.GetKey(con.up)) yInput += 1;
		}

		if (endLag <= 0 && (Input.GetKeyDown (con.attack1) || Input.GetKeyDown (con.attack1ALT))) 
		{
			SetSprite(sprites.idleSprites);
			atk.InputAttack (con.attack1, yInput, canJump);
			action = true;
			shielding = false;
			if (canJump)
				rb.velocity = new Vector2 (0, 0);
		} else if (endLag > 0)
			endLag -= Time.deltaTime;

    }

    void EnemyCheck()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position, col.size, 0, rb.velocity, 0.0f);

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
            slowDown = false;
			if (h.transform.tag == "Enemy")
            {
                slowDown = true;
                break;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
		if (other.transform.tag == "Enemy") 
		{
			slowDown = true;
		} 
		else if (other.transform.tag == "HealthBit") 
		{
			hp++;
			Destroy (other.gameObject);
		}
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            slowDown = false;
        }
    }

	public void ApplyHit(Vector2 knockback, float stun, float damage)
	{
		if (stunTime <= 0 && !shielding) {
			action = false;
			rb.velocity = knockback;
			stunTime = stun;
			SetSprite (sprites.knockbackSprites);
			hp -= damage;
		}
		if (shielding) 
		{
			sp -= damage / shieldThickness;
			//stunTime = stun;
		}
	}

	void CheckShield()
	{
		if (sp < -0.01f)
			sp = -0.01f;

		if (shieldDelay > 0) 
		{
			shieldDelay -= Time.deltaTime;
		}

		if (sp < 0 && shieldDelay <= 0) 
		{
			shieldDelay = shieldBreakDelay;
			sp = 0.001f;
			shieldBroken = true;
		}

		if (sp > spTotal - 0.5f) 
		{
			shieldBroken = false;
		}

		if (stunTime <= 0 && !shieldBroken && sp > 0 && (Input.GetKey (con.shield) || Input.GetKey (con.shieldALT))) 
		{
			shielding = true;
			SetSprite(sprites.shieldSprites);
			sp -= Time.deltaTime;
		} 
		else 
		{
			if (stunTime <= 0) shielding = false;
			if (shieldDelay <= 0) 
			{
				if (sp < spTotal)
					sp += Time.deltaTime;
			}
		}

		if (shieldDelay <= 0 && (Input.GetKeyUp (con.shield) || Input.GetKeyUp (con.shieldALT))) 
		{
			if (!shieldBroken) shieldDelay = shieldRechargeDelay;
			if (sp < 0) sp = 0.001f;
		}
	}
}
