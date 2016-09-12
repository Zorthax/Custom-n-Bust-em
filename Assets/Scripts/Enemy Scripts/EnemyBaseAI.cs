using UnityEngine;
using System.Collections;

public class EnemyBaseAI : MonoBehaviour {

	protected Transform parent;
	protected Rigidbody2D rb;
	protected EnemyBasics basics;
	protected bool attacking = false;
	protected float endLag = 0;
	protected PolygonCollider2D localCollider;
	protected Transform player;

	//Attack and animation stuff
	protected Attack currentAttack;
	protected float spriteIndex = 0;
	protected SpriteRenderer renderor;
	protected void Animation()
	{
		Destroy(gameObject.GetComponent<PolygonCollider2D>());
		renderor.sprite = currentAttack.sprites[Mathf.RoundToInt(spriteIndex)];

		if (currentAttack != null) 
		{
			if (spriteIndex < currentAttack.sprites.Length - 1)
				spriteIndex += currentAttack.animationSpeed;
			else {
				endLag = currentAttack.endLag;
				attacking = false;
				currentAttack = null;
				spriteIndex = 0;
			}
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

	protected void SetBaseVariables()
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			player.GetComponent<PlayerMovement> ().ApplyHit (new Vector2 (currentAttack.knockback.x * parent.lossyScale.x, currentAttack.knockback.y), currentAttack.stun, currentAttack.damage);
		}
	}

	class StateMachine
	{
		State state;

		public void SetState(State _state, EnemyBasics _basics, Attack _attack = null)
		{
			if (state != null)
				state.OnEnd ();
			state = _state;
			state.OnStart (_basics, _attack);
		}

		public void StateUpdate()
		{
			state.OnUpdate ();
		}

		public State GetState()
		{
			return state;
		}
	}

	class State
	{
		EnemyBasics basics;
		public virtual void OnStart(EnemyBasics _basics, Attack _attack = null)
		{
			basics = _basics;
		}
		public virtual void OnUpdate()
		{

		}
		public virtual void OnEnd()
		{

		}

	}

}
