using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    [Header("Basics")]
    public float walkingSpeed = 5;
    public float jumpForce = 10;
    public float jumpMin = 2;
    public Vector3 cameraPosition;
    AttackScript atk;

    [Space(5)]
    [Header("Sprites")]
    public MySprite idleSprites;
    public MySprite runSprites;
    public MySprite crouchSprites;
    public MySprite jumpSprites;
    MySprite currentSprite;
    float spriteIndex = 0;

    static Controls con;
    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer renderor;
    bool grounded;
    bool canJump;
    bool action;
    
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

        AttackControl();
        GroundCheck(); 

        float xMovement = CalculateXMovement();
        float yMovement = CalculateYMovement();

        //Apply velocity
        rb.velocity = new Vector2(xMovement * walkingSpeed, yMovement);

        //Keep camera looking at player
        Camera.main.transform.position = transform.position + cameraPosition;
        Animation();
	}

    void GroundCheck()
    {
        grounded = false; //Being grounded will apply extra gravity for smooth movement on slopes
        canJump = false;
        col.isTrigger = false;

        //Receive list of every collision
        RaycastHit2D[] hits;
        hits = Physics2D.BoxCastAll(transform.position - new Vector3(0, 0.65f), new Vector2(col.size.x, 0.1f), 0, Vector2.down, 0.01f); 

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
            if (h.rigidbody.tag != "Player" && h.rigidbody.tag != "NPC" && h.rigidbody.tag != "Passable")
            {
                grounded = true;
                canJump = true;
                break;
            }
        }
    }

    float CalculateXMovement()
    {
        float xMovement = 0;

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
        { xMovement = Input.GetAxis("Horizontal"); }
        else //keyboard movement
        {
            if (Input.GetKey(con.left) && (!action)) xMovement -= 1;
            if (Input.GetKey(con.right) && (!action)) xMovement += 1;
        }

        //Flip sprite to face direction of movement
        Vector3 ls = transform.localScale;
        if (xMovement < 0 && !action) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (xMovement > 0 && !action) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        if (!action) { if (xMovement != 0) SetSprite(runSprites); else SetSprite(idleSprites); }

        return xMovement;
    }

    float CalculateYMovement()
    {
        float yMovement = rb.velocity.y;
        if (!action && !canJump) SetSprite(jumpSprites);

        if (!action && canJump && (Input.GetKeyDown(con.jump) || Input.GetKeyDown(con.jumpALT)))
        {
            yMovement = jumpForce;
        }
        if (yMovement > jumpMin && (Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
        { yMovement = jumpMin; }
        if (grounded) yMovement -= 0.4f;
        if (!action && !grounded && yMovement > 0.1f && canJump) SetSprite(jumpSprites); //Prevents animation change when jumping up platforms


        return yMovement;
    }

    void AttackControl()
    {
    }

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
            currentSprite = sprite;
            spriteIndex = 0;
        }
        
    }
}
