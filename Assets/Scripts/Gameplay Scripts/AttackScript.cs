using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour {

    Animator anim;
    int neutralGround1;
    int frame = 0;
    public float animationSpeed = 0.005f;
    PolygonCollider2D localCollider;
    bool secondAttack;
    bool attackBuffer;
    KeyCode attackType;

    [System.Serializable]
    public class Attack
    {
        public Sprite[] sprites;
        public PolygonCollider2D[] frameHitboxes;
        public int firstFrameOfAttack;
        public int lastFrameOfAttack;
        public bool twoHits;
        public float secondHitFrame;
        public Sprite[] secondSprites;
        public PolygonCollider2D[] secondFrameHitboxes;
        public int secondFirstFrameOfAttack;
        public int secondLastFrameOfAttack;
    }
    public Attack[] attackList;

    //Animation stuff
    Attack currentAttack;
    float spriteIndex = 0;
    SpriteRenderer renderor;
    void Animation()
    {
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        if (!secondAttack) renderor.sprite = currentAttack.sprites[Mathf.RoundToInt(spriteIndex)];
        else if (secondAttack) renderor.sprite = currentAttack.secondSprites[Mathf.RoundToInt(spriteIndex)];
        if (secondAttack && spriteIndex <= currentAttack.secondSprites.Length - 1) //Second attack's sprites
            spriteIndex += animationSpeed;
        else if (!secondAttack && spriteIndex <= currentAttack.sprites.Length - 1) //First attack's sprites
            spriteIndex += animationSpeed;
        else if (!secondAttack && attackBuffer)
        {
            spriteIndex = 0;
            secondAttack = true;
        }
        else
        {
            currentAttack = null;
            secondAttack = false;
        }
    }

    static Controls con;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponentInParent<Animator>();
        con = GameObject.FindGameObjectWithTag("Controls").GetComponent<Controls>();
        renderor = GetComponentInParent<SpriteRenderer>();

        neutralGround1 = 0;
        currentAttack = null;
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true;
        localCollider.pathCount = 0;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (currentAttack != null)
        {
            NextAttack();
            Animation();
            if (currentAttack != null)
            {
                SetHitBox();
                
            }
            else
            {
                attackBuffer = false;
            }
        }
        else
            GetComponentInParent<PlayerMovement>().action = false;
	}

    public void InputAttack(KeyCode attackKey, int direction, bool onGround)
    {
        if (attackKey == con.attack1)
        {
            if (direction == 0) { if (onGround) currentAttack = attackList[neutralGround1]; }
            spriteIndex = 0;
            secondAttack = false;
            attackType = attackKey;
        }
    }

    void SetHitBox()
    {
        
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true;
        if (secondAttack && spriteIndex >= currentAttack.secondFirstFrameOfAttack && spriteIndex <= currentAttack.secondLastFrameOfAttack)//Second attack's hitboxes
        {
            localCollider.SetPath(0, currentAttack.secondFrameHitboxes[Mathf.RoundToInt(spriteIndex) - currentAttack.secondFirstFrameOfAttack].GetPath(0));
            return;
        }
        if (!secondAttack && spriteIndex >= currentAttack.firstFrameOfAttack && spriteIndex <= currentAttack.lastFrameOfAttack) //First attack's hitboxes
        {
            localCollider.SetPath(0, currentAttack.frameHitboxes[Mathf.RoundToInt(spriteIndex) - currentAttack.firstFrameOfAttack].GetPath(0));
            return;
        }
        localCollider.pathCount = 0;
    }
    void NextAttack()
    {
        if (!secondAttack && spriteIndex >= currentAttack.secondHitFrame && Input.GetKeyDown(attackType))
        { attackBuffer = true; }
    }
}
