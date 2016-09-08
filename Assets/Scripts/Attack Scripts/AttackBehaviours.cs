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
		case "RisingKick":
			attack = new RisingKick ();
			break;
		case "SwordThrow":
			attack = new SwordThrow ();
			break;
		default:
			attack = new Attack ();
			break;
		}
		attack._player = player;
		attack._rb = rb;
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
	public Rigidbody2D _rb;
	public virtual float Update(float spriteIndex, Rigidbody2D rb)
	{
		return spriteIndex;
	}

	public virtual void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
	}

	protected bool OnGround(float spriteIndex)
	{
		if (Mathf.Floor (spriteIndex) >= 1 && _player.canJump
			&& Mathf.Abs(_rb.velocity.y) < 0.2f)
			return true;
		else
			return false;
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

public class DropKick : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (OnGround(spriteIndex))
			return 50;
		if (Mathf.Floor (spriteIndex) == 1)
			rb.velocity = new Vector2(4 * rb.transform.lossyScale.x, -8);
		if (rb.velocity.y >= 0 && rb.velocity.y < 0.2f && spriteIndex >= 1 && spriteIndex < 4) {
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
			rb.velocity = new Vector2 (1.5f * rb.transform.lossyScale.x, 4);

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

public class RisingKick : Attack
{
    bool risen = false;
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
        if (spriteIndex > 1.0f && risen == false)
        {
            rb.velocity = new Vector2(0, 20);
            risen = true;
        }
		if (Mathf.Floor (spriteIndex) > 1)
			rb.velocity = new Vector2 (0, 1);

		return spriteIndex;
	}

	public override void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
		//Debug.Log ("Dropkick hit");
		if (Mathf.Floor (spriteIndex) == 1)
		enemyRb.GetComponent<EnemyBasics> ().ApplyHit (new Vector2 (0, 25), 2, 0.6f);
	}
}

public class SwordThrow : Attack
{
	bool thrown = false;
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (Mathf.Floor (spriteIndex) == 1 && !thrown) 
		{
			if (_player.GetComponent<PlayerMovement> ().mp >= 2) 
			{
				GameObject sword = Object.Instantiate (Resources.Load ("Sword"), rb.transform.position + new Vector3((1 * rb.transform.lossyScale.x), -0.12f, 0), new Quaternion (0, 0, 0, 0)) as GameObject;
				sword.transform.localScale = rb.transform.lossyScale;
			}
			thrown = true;
		}

		return spriteIndex;
	}
}