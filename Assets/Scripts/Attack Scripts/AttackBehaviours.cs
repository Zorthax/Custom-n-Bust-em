using UnityEngine;
using System.Collections;

public class AttackBehaviours : MonoBehaviour 
{

	Attack attack;
	Rigidbody2D rb;
	PlayerMovement player;

	public void Start()
	{
		rb = GetComponentInParent<Rigidbody2D> ();
		player = GetComponentInParent<PlayerMovement> ();
	}

	public void SetType(string uniqueType)
	{
		switch (uniqueType) 
		{
		case "Air":
			attack = new Air ();
			break;
		case "DropKick":
			attack = new DropKick ();
			break;
		case "Thrust":
			attack = new Thrust ();
			break;
		case "Shoryuken":
			attack = new Shoryuken ();
			break;
		case "Hurricane":
			attack = new Hurricane ();
			break;
		default:
			attack = new Attack ();
			break;
		}
		attack._player = player;
	}

	public float MyUpdate(float spriteIndex)
	{
		return attack.Update (spriteIndex, rb);
	}

	public void MyTriggerEnter(float spriteIndex, Rigidbody2D enemyRb)
	{
		attack.OnTriggerEnter (spriteIndex, rb, enemyRb);
	}



}

public class Attack
{
	public PlayerMovement _player;
	public virtual float Update(float spriteIndex, Rigidbody2D rb)
	{
		return spriteIndex;
	}

	public virtual void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
	}

	protected bool OnGround(float spriteIndex)
	{
		if ((Mathf.Floor (spriteIndex) >= 1 && _player.canJump))
			return true;
		else
			return false;
	}
}

public class DropKick : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (OnGround(spriteIndex))
			return 50;
		if (Mathf.Floor (spriteIndex) == 1)
			rb.velocity = new Vector2(4 * rb.transform.lossyScale.x, -6);
		if (rb.velocity.y >= 0 && rb.velocity.y < 0.2f && spriteIndex > 2 && spriteIndex < 4) {
			rb.velocity = new Vector2 (-3 * rb.transform.lossyScale.x, 0); 
			return 4.0f;
		}
		else
			return spriteIndex;
	}

	public override void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
		//Debug.Log ("Dropkick hit");
		rb.velocity = new Vector2 (-3 * rb.transform.lossyScale.x, 5); 
	}
}

public class Thrust : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (Mathf.Floor (spriteIndex) == 2)
			rb.velocity = new Vector2 (6 * rb.transform.lossyScale.x, rb.velocity.y);
		if (Mathf.Floor (spriteIndex) == 6)
			rb.velocity = new Vector2 (0, rb.velocity.y);

		return spriteIndex;
	}
}

public class Shoryuken : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (Mathf.Floor (spriteIndex) == 3)
			rb.velocity = new Vector2 (0, 4);

		return spriteIndex;
	}
}

public class Air : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (OnGround(spriteIndex))
			return 50;

		return spriteIndex;
	}
}

public class Hurricane : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (OnGround(spriteIndex))
			return 50;

		return spriteIndex;
	}

	public override void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
		//Debug.Log ("Dropkick hit");
		float direc;
		if (rb.transform.position.x < enemyRb.transform.position.x)
			direc = 1;
		else
			direc = -1;
		enemyRb.GetComponent<EnemyBasics> ().ApplyHit (new Vector2 (7 * direc, 1), 2, 0.6f);
	}
}