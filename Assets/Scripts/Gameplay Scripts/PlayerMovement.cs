using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    [Header("Basics")]
    public float walkingSpeed = 5;
    public float jumpForce = 10;
    public float jumpMin = 2;
	public float jumpFrame = 2;
    public Vector3 cameraPosition;
    AttackScript atk;

    [Space(5)]
    [Header("Sprites")]
    public MySprite idleSprites;
    public MySprite runSprites;
    public MySprite crouchSprites;
    public MySprite jumpSprites;
	public MySprite airSprites;

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


    static Controls con;
    Rigidbody2D rb;
    BoxCollider2D col;
    
    bool grounded;
    bool canJump;
    public bool action;
    float xMovement;
    float yMovement;
    bool slowDown = false;
    
    

    // Use this for initialization
    void Start ()
    {
        con = GameObject.FindGameObjectWithTag("Controls").GetComponent<Controls>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        atk = GetComponentInChildren<AttackScript>();
        renderor = GetComponent<SpriteRenderer>();
        currentSprite = idleSprites;
        action = false;
        
        
    }
	
	// Update is called once per frame

	void Update ()
    {
        if (!action) AttackControl();
        GroundCheck();

        xMovement = CalculateXMovement();
        yMovement = CalculateYMovement();

        //Apply velocity
        rb.velocity = new Vector2(xMovement * walkingSpeed, yMovement);

        //Keep camera looking at player
        Camera.main.transform.position = transform.position + cameraPosition;
        if (!action) Animation();
	}

    void GroundCheck()
    {
        grounded = false; //Being grounded will apply extra gravity for smooth movement on slopes
        canJump = false;
        col.isTrigger = false;

        //Receive list of every collision
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position - new Vector3(0, 0.5f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.02f); 

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
            if (h.rigidbody.tag == "Passable")
            {
                canJump = true;
                if (Input.GetKey(con.down)) //Jump down from platform
                {
                    col.isTrigger = true;
                    transform.position -= new Vector3(0, 0.1f, 0);
                    canJump = false;
                }
            }
            if (h.rigidbody.tag != "Player" && h.transform.tag != "NPC" && h.rigidbody.tag != "Slope" && h.rigidbody.tag != "Passable" && h.transform.tag != "Enemy")
            {
                canJump = true;
                break;
            }
        }

        SlopeCheck();

		if (currentSprite == jumpSprites && spriteIndex < jumpFrame)
			canJump = false;
    }

    void SlopeCheck()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position - new Vector3(0, 0.5f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.03f);

        //Find specific collision tags
        foreach (RaycastHit2D h in hits)
        {
            if (h.rigidbody.tag == "Slope")
            {
                grounded = true;
                canJump = true;
                break;
            }
        }
    }
    float CalculateXMovement()
    {
        float x = 0;

        //Exit early if crouching
        if (grounded && !action)
        {
            if (Input.GetKey(con.down))
            {
                SetSprite(crouchSprites);
                return 0;
            }
        }

        //Horizontal movement
        if (Input.GetAxis("Horizontal") != 0 && (!action)) //Controller movement takes priority over keyboard
        { x = Input.GetAxis("Horizontal"); }
        else //keyboard movement
        {
            if (Input.GetKey(con.left) && (!action)) x -= 1;
            if (Input.GetKey(con.right) && (!action)) x += 1;
        }

        //Flip sprite to face direction of movement
        Vector3 ls = transform.localScale;
        if (x < 0 && !action) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (x > 0 && !action) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        if (!action && canJump) { if (x != 0) SetSprite(runSprites); else SetSprite(idleSprites); }

        if (slowDown) x /= 7;
        return x;
    }

    float CalculateYMovement()
    {
        float y = rb.velocity.y;

		//Smooth jump animations
        if (!action && canJump && (Input.GetKeyDown(con.jump) || Input.GetKeyDown(con.jumpALT)))
        {
			SetSprite(jumpSprites);
            if ((Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
            { y = jumpMin; }
        }
		if (!action && !canJump && currentSprite == jumpSprites) 
		{
            if (Mathf.RoundToInt(spriteIndex) == jumpFrame && rb.velocity.y < jumpMin / 2)
            { y = jumpForce; }
            else if (Mathf.RoundToInt(spriteIndex) >= jumpSprites.sprites.Length)
                SetSprite(airSprites);
            //Hold to jump higher
            if ((Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)) && (y == 0))
            { y = jumpMin; }

        }
		//Hold to jump higher
        if (y > jumpMin && (Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
        { y = jumpMin; canJump = false; }

		if (grounded) y -= 0.5f;//Smooth slope walking
        if (!action && !grounded && y > 0.1f && canJump) SetSprite(airSprites); //Prevents animation change when jumping up platforms
		if (!action && !canJump && currentSprite != jumpSprites) SetSprite(airSprites); //Set air sprite when falling

        return y;
    }

    void AttackControl()
    {
        if (Input.GetKeyDown(con.attack1) || Input.GetKeyDown(con.attack1ALT))
        {
            atk.InputAttack(con.attack1, 0, canJump);
            action = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            slowDown = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            slowDown = false;
        }
    }

}
