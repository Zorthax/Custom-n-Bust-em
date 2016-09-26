using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour {

	public static AttackScript attackScript;

    int neutralGround1;
	int downGround1;
    int upGround1;
    int neutralAir1;
	int downAir1;
	int upAir1;
	int neutralGround2;
	int downGround2;
	int upGround2;
	int neutralAir2;
	int downAir2;
	int upAir2;

    PolygonCollider2D localCollider;
    bool secondAttack;
    bool attackBuffer;
    KeyCode attackType;
	KeyCode attackTypeALT;
	AttackBehaviours behaviour;
	PlayerMovement player;

    [System.Serializable]
    public class Attack
    {
        public float animationSpeed;
        public Sprite[] sprites;
        public PolygonCollider2D[] frameHitboxes;
        public int firstFrameOfAttack;
        public int lastFrameOfAttack;
        public float firstFrameBuffer;
		public float damage;
		public float stun;
        public Vector2 knockback;
		public string uniqueType = "default";
		public float endLag;
		public bool moveAndAttack;

        public bool hasSecondHit;

    }
    [System.Serializable]
    public class OriginalAttack : Attack { public SecondAttack secondAttack; }
    [System.Serializable]
    public class SecondAttack : Attack { }
    public OriginalAttack[] attackList;

    //Animation stuff
    OriginalAttack currentAttack;
    float spriteIndex = 0;
    SpriteRenderer renderor;
    void Animation()
    {
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
		spriteIndex = behaviour.MyUpdate (spriteIndex);

		if (spriteIndex <= currentAttack.sprites.Length || (currentAttack.hasSecondHit && spriteIndex <= currentAttack.secondAttack.sprites.Length)) 
		{
			//Change physical sprite
			if (!secondAttack) 
			{
				renderor.sprite = currentAttack.sprites [Mathf.RoundToInt (spriteIndex)];
			} 
			else if (secondAttack) 
			{
				renderor.sprite = currentAttack.secondAttack.sprites [Mathf.RoundToInt (spriteIndex)];
			}
		}

        if (secondAttack && spriteIndex <= currentAttack.secondAttack.sprites.Length - 1) //Second attack's sprites
        { spriteIndex += currentAttack.secondAttack.animationSpeed; }
        else if (!secondAttack && spriteIndex <= currentAttack.sprites.Length - 1) //First attack's sprites
        { spriteIndex += currentAttack.animationSpeed; }
        else if (!secondAttack && attackBuffer && currentAttack.hasSecondHit)
        {
            spriteIndex = 0;
            secondAttack = true;
        }
        else
        {
			player.endLag = currentAttack.endLag;
			player.action = false;
            currentAttack = null;
            secondAttack = false;
        }
    }

    // Use this for initialization
    void Start ()
    {
		if (attackScript == null) {
			attackScript = this;
		} else if (attackScript != this) {
			Destroy (this.gameObject);
		}
        renderor = GetComponentInParent<SpriteRenderer>();
		behaviour = GetComponent<AttackBehaviours> ();
		player = GetComponentInParent<PlayerMovement> ();

        neutralGround1 = 0;
        neutralAir1 = 5;
		downGround1 = 2;
		upGround1 = 3;
		downAir1 = 4;

		neutralGround2 = 7;
		upGround2 = 6;
		neutralAir2 = 7;
		downAir2 = 1;
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
            BufferNextAttack();
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
        //else
        //    player.action = false;
	}

    public void InputAttack(KeyCode attackKey, float direction, bool onGround)
    {
		if (attackKey == Controls.attack1) {
			if (direction == 0) {
				if (onGround)
					currentAttack = attackList [neutralGround1];
				else
					currentAttack = attackList [neutralAir1];
			}
			if (direction < 0) {
				if (onGround)
					currentAttack = attackList [downGround1];
				else
					currentAttack = attackList [downAir1];
			}
			if (direction > 0) {
				if (onGround)
					currentAttack = attackList [upGround1];
				else
					currentAttack = attackList [neutralAir1];
			}
			spriteIndex = 0;
			secondAttack = false;
			attackType = attackKey;
			attackTypeALT = Controls.attack1ALT;
			behaviour.SetType (currentAttack.uniqueType);
		} else if (attackKey == Controls.attack2) {
			if (direction == 0) {
				if (onGround)
					currentAttack = attackList [neutralGround2];
				else
					currentAttack = attackList [neutralAir2];
			}
			if (direction < 0) {
				if (onGround)
					currentAttack = attackList [downGround2];
				else
					currentAttack = attackList [downAir2];
			}
			if (direction > 0) {
				if (onGround)
					currentAttack = attackList [upGround2];
				else
					currentAttack = attackList [neutralAir2];
			}
			spriteIndex = 0;
			secondAttack = false;
			attackType = attackKey;
			attackTypeALT = Controls.attack2ALT;
			behaviour.SetType (currentAttack.uniqueType);
		} else
			currentAttack = null;

		if (currentAttack != null) {
			PlayerMovement pm = GetComponentInParent<PlayerMovement> ();
			pm.moveAndAttack = currentAttack.moveAndAttack;
		}
    }

    void SetHitBox()
    {
        
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true;
        if (secondAttack && spriteIndex >= currentAttack.secondAttack.firstFrameOfAttack && spriteIndex <= currentAttack.secondAttack.lastFrameOfAttack)//Second attack's hitboxes
        {
            localCollider.SetPath(0, currentAttack.secondAttack.frameHitboxes[Mathf.RoundToInt(spriteIndex) - currentAttack.secondAttack.firstFrameOfAttack].GetPath(0));
            return;
        }
        if (!secondAttack && spriteIndex >= currentAttack.firstFrameOfAttack && spriteIndex <= currentAttack.lastFrameOfAttack) //First attack's hitboxes
        {
            localCollider.SetPath(0, currentAttack.frameHitboxes[Mathf.RoundToInt(spriteIndex) - currentAttack.firstFrameOfAttack].GetPath(0));
            return;
        }
        localCollider.pathCount = 0;
    }
    void BufferNextAttack()
    {
        //if (!secondAttack && spriteIndex >= currentAttack.secondHitFrame && Input.GetKeyDown(attackType))
        //{ attackBuffer = true; }
		if (!secondAttack && currentAttack.hasSecondHit && spriteIndex >= currentAttack.firstFrameBuffer && (Input.GetKeyDown(attackType) || Input.GetKeyDown(attackTypeALT)))
        { attackBuffer = true; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
			//Debug.Log(currentAttack.knockback);
            float scale = transform.lossyScale.x;
            if (!secondAttack) other.transform.GetComponent<EnemyBasics>().ApplyHit(
				new Vector2(currentAttack.knockback.x * scale, currentAttack.knockback.y), currentAttack.damage, currentAttack.stun);
            else other.transform.GetComponent<EnemyBasics>().ApplyHit(
				new Vector2(currentAttack.secondAttack.knockback.x * scale, currentAttack.secondAttack.knockback.y), currentAttack.secondAttack.damage, currentAttack.secondAttack.stun);
			behaviour.MyTriggerEnter (spriteIndex, other.GetComponent<Rigidbody2D>());
        }
    }

	public void SetAttack(string inputName, int index)
	{
		switch (inputName.ToUpper()) 
		{
		case "NEUTRALGROUND1":
			neutralGround1 = index;
			break;
		case "UPGROUND1":
			upGround1 = index;
			break;
		case "DOWNGROUND1":
			downGround1 = index;
			break;
		case "NEUTRALGROUND2":
			neutralGround2 = index;
			break;
		case "UPGROUND2":
			upGround2 = index;
			break;
		case "DOWNGROUND2":
			downGround2 = index;
			break;
		case "NEUTRALAIR1":
			neutralAir1 = index;
			break;
		case "UPAIR1":
			upAir1 = index;
			break;
		case "DOWNAIR1":
			downAir1 = index;
			break;
		case "NEUTRALAIR2":
			neutralAir2 = index;
			break;
		case "UPAIR2":
			upAir2 = index;
			break;
		case "DOWNAIR2":
			downAir2 = index;
			break;
		}
	}
}
