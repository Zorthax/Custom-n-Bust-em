using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour {

    Animator anim;
    int neutralGround1;
    int frame = 0;
    float animationSpeed = 0.005f;
    PolygonCollider2D localCollider;

    [System.Serializable]
    public class Attack
    {
        public Sprite[] sprites;
        public PolygonCollider2D[] frameHitboxes;
        public int firstFrameOfAttack;
        public int lastFrameOfAttack;
    }
    public Attack[] attackList;

    //Animation stuff
    Attack currentAttack;
    float spriteIndex = 0;
    SpriteRenderer renderor;
    void Animation()
    {
        renderor.sprite = currentAttack.sprites[Mathf.RoundToInt(spriteIndex)];
        if (spriteIndex <= currentAttack.sprites.Length - 1) spriteIndex += animationSpeed;
        else currentAttack = null;
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
	void Update ()
    {
        if (currentAttack != null)
        {
            Animation();
            if (currentAttack != null) SetHitBox();
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
        }
    }

    void SetHitBox()
    {
        if (spriteIndex >= currentAttack.firstFrameOfAttack && spriteIndex <= currentAttack.lastFrameOfAttack)
        {
            localCollider.SetPath(0, currentAttack.frameHitboxes[Mathf.RoundToInt(spriteIndex)].GetPath(0));
            return;
        }
        localCollider.pathCount = 0;
    }
}
