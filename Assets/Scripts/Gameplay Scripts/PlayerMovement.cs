using UnityEngine;
using System.Collections;



public class PlayerMovement : MonoBehaviour {

	public static PlayerMovement PlayerScript;

    [Header("Basics")]
	public float hpTotal;
	float hp;
	public float mpTotal;
	public float mp;
    public float walkingSpeed = 5;
    public float jumpForce = 10;
    public float jumpMin = 2;
	public float jumpFrame = 2;
    public Vector3 cameraPosition;
	public float rollSpeed = 6;
	public float endLag;
	float stunTime;
    AttackScript atk;
	public bool moveAndAttack;
	bool invincible = false;

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
		
    Rigidbody2D rb;
    BoxCollider2D col;
    
    bool onSlope;
    public bool canJump;
	bool shielding;
    public bool action;
    float xMovement;
    float yMovement;
    bool slowDown = false;
	PlayerSprites sprites;

	bool jumpPressed;
	bool jumpReleased;

   // Use this for initialization
    void Start ()
    {
		if (PlayerScript == null) {
			PlayerScript = this;
		} else if (PlayerScript != this) {
			Destroy (this.gameObject);
		}
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

	void Update()
	{
		if (hp > hpTotal)
			hp = hpTotal;
		if (sp > spTotal)
			sp = spTotal;

		jumpPressed = Controls.Jump ();
		jumpReleased = Controls.JumpReleased ();

		if (stunTime <= 0) {

			if (!action) 
			{
				AttackControl ();
			}
			GroundCheck ();
			EnemyCheck ();
			Rolling ();

		} else {
			stunTime -= Time.deltaTime;
			SetSprite (sprites.knockbackSprites);
		}
	}
	void FixedUpdate ()
    {
		//Keep camera looking at player
        Camera.main.transform.position = transform.position + cameraPosition;
        if (!action) Animation();
		sprites.SetStats (hpTotal, hp, spTotal, sp, mpTotal, mp);

		if (!action)
			CheckShield ();
		
		if (stunTime <= 0) 
		{
			if (currentSprite != sprites.forwardRoll)
				xMovement = CalculateXMovement ();
			yMovement = CalculateYMovement ();
			rb.velocity = new Vector2 (xMovement, yMovement);
		}
	}

    void GroundCheck()
    {
        onSlope = false; //will apply extra gravity for smooth movement on slopes
        canJump = false;
        col.isTrigger = false;

        //Receive list of every collision
        RaycastHit2D[] hits;
		hits = Physics2D.BoxCastAll(transform.position - new Vector3(0.05f * transform.lossyScale.x, 0.5f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.02f); 

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
			if (h.transform.tag == "Passable")
            {
                canJump = true;
				if (Controls.Vertical() < 0.4f && yMovement <= 0 ) //Jump down from platform
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
		//if (shielding && !canJump)
		//	x = xMovement;

        //Horizontal movement
		if ((!action || moveAndAttack) && !shielding)
		{ x = Controls.Horizontal() * walkingSpeed; }
		else if (action || (shielding && !canJump))
		{
			x = rb.velocity.x;
		}

        //Flip sprite to face direction of movement
        Vector3 ls = transform.localScale;
        if (x < 0 && !action) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (x > 0 && !action) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
		if (!action && !shielding && canJump) { if (x != 0) SetSprite(sprites.runSprites); else SetSprite(sprites.idleSprites); }

		//if (slowDown) x /= 3;
		if (shielding) x /= 1.02f;
        return x;
    }

    float CalculateYMovement()
    {
        float y = rb.velocity.y;

		//Smooth jump animations
		if (!action && canJump && jumpPressed)
        {
			SetSprite(sprites.jumpSprites);
			if (jumpReleased)
            { y = jumpMin; }
        }
		if (!action && !canJump && currentSprite == sprites.jumpSprites) 
		{
            if (Mathf.RoundToInt(spriteIndex) == jumpFrame && rb.velocity.y < jumpMin / 2)
            { y = jumpForce; }
            else if (Mathf.RoundToInt(spriteIndex) >= sprites.jumpSprites.sprites.Length)
                SetSprite(sprites.airSprites);
            //Hold to jump higher
			if (jumpReleased && (y == 0))
            { y = jumpMin; }

        }
		//Hold to jump higher
		if (y > jumpMin && !jumpPressed)
        { y = jumpMin; canJump = false; }

		if (onSlope) y -= 0.5f;//Smooth slope walking
        if (!action && !shielding && !onSlope && y > 0.1f && canJump) SetSprite(sprites.airSprites); //Prevents animation change when jumping up platforms
		if (!action && !shielding &&  !canJump && currentSprite != sprites.jumpSprites) SetSprite(sprites.airSprites); //Set air sprite when falling

        return y;
    }

    void AttackControl()
    {
		float yInput = Controls.Vertical ();

		if (endLag <= 0 && Controls.Attack1Pressed()) 
		{
			SetSprite(sprites.idleSprites);
			atk.InputAttack (Controls.attack1, yInput, canJump);
			action = true;
			shielding = false;
			if (canJump)
				rb.velocity = new Vector2 (0, 0);
		} 
		else if (endLag <= 0 && Controls.Attack2Pressed()) 
		{
			SetSprite(sprites.idleSprites);
			atk.InputAttack (Controls.attack2, yInput, canJump);
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
		if (!invincible && stunTime <= 0 && !shielding) {
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

		if (endLag <= 0 && stunTime <= 0 && !shieldBroken && sp > 0 && Controls.Shield()) 
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

		if (shieldDelay <= 0 && Controls.ShieldReleased()) 
		{
			if (!shieldBroken) shieldDelay = shieldRechargeDelay;
			if (sp < 0) sp = 0.001f;
		}
	}

	void Rolling ()
	{
		

		//End of roll sprite
		if (currentSprite == sprites.forwardRoll && spriteIndex >= currentSprite.sprites.Length - 1) 
		{
			endLag = 0.25f;
			SetSprite (sprites.idleSprites);
			action = false;
		}

		////Leave roll state if in air
		//if (currentSprite == sprites.forwardRoll && !canJump) 
		//{
		//	endLag = 0.5f;
		//	SetSprite (sprites.idleSprites);
		//	action = false;
		//}

		//Roll input
		if (canJump && shielding && endLag <= 0 && !action) 
		{
			if (Mathf.Abs(Controls.Horizontal()) > 0.8f) 
			{
				float x = 1;
				if (Controls.Horizontal() < 0) x = -1;
				if (Controls.Horizontal() > 0) x = 1;

				Vector3 ls = transform.lossyScale;
				transform.localScale = new Vector3 (x * Mathf.Abs(ls.x), ls.y, ls.z);
				SetSprite (sprites.forwardRoll);
				action = true;
				shielding = false;
			}
		}

		//Roll movement
		if (currentSprite == sprites.forwardRoll) 
		{
			xMovement = rollSpeed * transform.lossyScale.x;
			shielding = false;
			Animation ();
			action = true;

			if (spriteIndex > 2 && spriteIndex < currentSprite.sprites.Length - 3)
				invincible = true;
			else
				invincible = false;
		}
	}
}
