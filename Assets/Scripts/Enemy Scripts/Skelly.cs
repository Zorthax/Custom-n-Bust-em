using UnityEngine;
using System.Collections;

public class Skelly : MonoBehaviour {

	public Attack groundAttack;
	public float attackDistance;
	Transform parent;
	Rigidbody2D rb;
	EnemyBasics basics;
	bool attacking = false;
	Transform player;
	float endLag = 0;

	PolygonCollider2D localCollider;

	//Attack and animation stuff
	Attack currentAttack;
	float spriteIndex = 0;
	SpriteRenderer renderor;
	void Animation()
	{
		Destroy(gameObject.GetComponent<PolygonCollider2D>());
		renderor.sprite = currentAttack.sprites[Mathf.RoundToInt(spriteIndex)];

		if (spriteIndex < currentAttack.sprites.Length - 1)
			spriteIndex += currentAttack.animationSpeed;
		else 
		{
			endLag = currentAttack.endLag;
			attacking = false;
			currentAttack = null;
			spriteIndex = 0;

		}
	}
	[System.Serializable]
	public class Attack
	{
		public float animationSpeed;
		public Sprite[] sprites;
		public PolygonCollider2D[] frameHitboxes;
		public int firstFrameOfAttack;
		public int lastFrameOfAttack;
		public float damage;
		public float stun;
		public Vector2 knockback;
		public float endLag;
	}

	// Use this for initialization
	void Start () 
	{
		renderor = GetComponentInParent<SpriteRenderer> ();
		basics = GetComponentInParent<EnemyBasics> ();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		parent = GetComponentInParent<Transform> ();
		rb = GetComponentInParent<Rigidbody2D> ();

		localCollider = gameObject.AddComponent<PolygonCollider2D>();
		localCollider.isTrigger = true;
		localCollider.pathCount = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (basics.stunned) 
		{
			attacking = false;
			basics.SetAttackState (attacking);
			Destroy(gameObject.GetComponent<PolygonCollider2D>());
		}
		if (attacking && currentAttack != null) 
		{
			Animation ();
			if (currentAttack != null)
				SetHitBox ();
			basics.SetAttackState (attacking);
		}

		if (!attacking && endLag <= 0)
			AttackPlayer ();
		else
			endLag -= Time.deltaTime;
	}

	void AttackPlayer()
	{
		if (!basics.stunned && Vector2.Distance (player.position, transform.position) < attackDistance) 
		{
			if ((parent.lossyScale.x < 0 && player.position.x < transform.position.x)
			    || (parent.lossyScale.x >= 0 && player.position.x >= transform.position.x)) 
			{
				currentAttack = groundAttack;
				attacking = true;
				basics.SetAttackState (attacking);
				spriteIndex = 0;
				rb.velocity = new Vector2 (0, 0);
			}
		}
	}

	void SetHitBox()
	{

		localCollider = gameObject.AddComponent<PolygonCollider2D>();
		localCollider.isTrigger = true;
		if (spriteIndex >= currentAttack.firstFrameOfAttack && spriteIndex <= currentAttack.lastFrameOfAttack) //First attack's hitboxes
		{
			localCollider.SetPath(0, currentAttack.frameHitboxes[Mathf.RoundToInt(spriteIndex) - currentAttack.firstFrameOfAttack].GetPath(0));
			return;
		}
		localCollider.pathCount = 0;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			player.GetComponent<PlayerMovement> ().ApplyHit (new Vector2 (currentAttack.knockback.x * parent.lossyScale.x, currentAttack.knockback.y), currentAttack.stun, currentAttack.damage);
		}
	}
}
