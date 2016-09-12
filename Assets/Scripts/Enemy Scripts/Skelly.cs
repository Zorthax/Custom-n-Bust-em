using UnityEngine;
using System.Collections;

public class Skelly : EnemyBaseAI {

	public Attack groundAttack;
	public float attackDistance;

	// Use this for initialization
	void Start () 
	{
		SetBaseVariables ();
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


}
