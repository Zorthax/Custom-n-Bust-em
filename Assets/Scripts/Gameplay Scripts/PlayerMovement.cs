using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float walkingSpeed = 5;
    public float jumpForce = 10;
    public float jumpMin = 2;
    public Vector3 cameraPosition;
    Animator anim;


    static Controls con;
    Rigidbody2D rb;
    BoxCollider2D col;
    bool grounded;
    bool canJump;

    // Use this for initialization
    void Start ()
    {
        con = GameObject.FindGameObjectWithTag("Controls").GetComponent<Controls>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        GroundCheck();

        float xMovement = CalculateXMovement();
        float yMovement = CalculateYMovement();

        //Apply velocity
        rb.velocity = new Vector2(xMovement * walkingSpeed, yMovement);

        //Keep camera looking at player
        Camera.main.transform.position = transform.position + cameraPosition;
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
        if (grounded)
        {
            anim.SetBool("Crouch", Input.GetKey(con.down));
            if (Input.GetKey(con.down)) return 0;
        }

        //Horizontal movement
        if (Input.GetAxis("Horizontal") != 0) //Controller movement takes priority over keyboard
        { xMovement = Input.GetAxis("Horizontal"); }
        else //keyboard movement
        {
            if (Input.GetKey(con.left)) xMovement -= 1;
            if (Input.GetKey(con.right)) xMovement += 1;
        }

        //Flip sprite to face direction of movement
        Vector3 ls = transform.localScale;
        if (xMovement < 0) transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
        if (xMovement > 0) transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        if (xMovement != 0 && canJump) anim.SetBool("Running", true); else anim.SetBool("Running", false);

        return xMovement;
    }

    float CalculateYMovement()
    {
        float yMovement = rb.velocity.y;

        if (canJump && (Input.GetKeyDown(con.jump) || Input.GetKeyDown(con.jumpALT)))
        {
            yMovement = jumpForce;
        }
        if (yMovement > jumpMin && (Input.GetKeyUp(con.jump) || Input.GetKeyUp(con.jumpALT)))
        { yMovement = jumpMin; }
        if (grounded) yMovement -= 0.4f;
        anim.SetBool("Grounded", canJump);
        if (!grounded && (yMovement < -0.5f || yMovement > 0.0f) && canJump) anim.SetBool("Grounded", false);

        return yMovement;
    }
}
