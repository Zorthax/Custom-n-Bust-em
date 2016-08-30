using UnityEngine;
using System.Collections;

public class AttackBehaviours : MonoBehaviour 
{

	Attack attack;
	Rigidbody2D rb;

	public void Start()
	{
		rb = GetComponentInParent<Rigidbody2D> ();
	}

	public void SetType(string uniqueType)
	{
		switch (uniqueType) 
		{
		case "DropKick":
			attack = new DropKick ();
			break;
		case "Thrust":
			attack = new Thrust ();
			break;
		default:
			attack = new Attack ();
			break;
		}
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
	public virtual float Update(float spriteIndex, Rigidbody2D rb)
	{
		return spriteIndex;
	}

	public virtual void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
	}
}

public class DropKick : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (Mathf.Floor (spriteIndex) == 0)
			rb.velocity = new Vector2 (4 * rb.transform.localScale.x, -5);
		if (rb.velocity.y >= 0 && rb.velocity.y < 0.2f && spriteIndex < 3) {
			rb.velocity = new Vector2 (-3 * rb.transform.localScale.x, 0); 
			return 4.0f;
		}
		else
			return spriteIndex;
	}

	public override void OnTriggerEnter(float spriteIndex, Rigidbody2D rb, Rigidbody2D enemyRb)
	{
		//Debug.Log ("Dropkick hit");
		rb.velocity = new Vector2 (-3 * rb.transform.localScale.x, 4); 
	}
}

public class Thrust : Attack
{
	public override float Update(float spriteIndex, Rigidbody2D rb)
	{
		if (Mathf.Floor (spriteIndex) == 2)
			rb.velocity = new Vector2 (6 * rb.transform.localScale.x, rb.velocity.y);
		if (Mathf.Floor (spriteIndex) == 6)
			rb.velocity = new Vector2 (0, rb.velocity.y);

		return spriteIndex;
	}
}