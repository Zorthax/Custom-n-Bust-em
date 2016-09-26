using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {

	public GameObject groundInputs;
	public GameObject aerialInputs;
	public GameObject groundPool;
	public GameObject aerialPool;
	GameObject activeSlot;
	GameObject activeAttack;


	bool overAerial;
	bool overGround;
	bool onSlot;
	bool overSelectable;
	string attackName;
	bool attackMode;
	bool slotMode;

	// Use this for initialization
	void Start () 
	{
	
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		float speed = 8.0f;
		if (Mathf.Abs(Controls.Horizontal()) + Mathf.Abs(Controls.Vertical()) <= 1.5f)
			transform.position += new Vector3 (Controls.Horizontal() * speed, Controls.Vertical() * speed, 0);
		else
			transform.position += new Vector3 ((Controls.Horizontal() / 2) * speed, (Controls.Vertical() / 2) * speed, 0);
	}

	void Update()
	{
		SetAttack ();
		if (onSlot && Controls.JumpPressed()) 
		{
			if (overAerial) 
			{
				groundInputs.SetActive (false);
				aerialPool.SetActive (true);
			}
			if (overGround)
			{
				aerialInputs.SetActive(false);
				groundPool.SetActive(true);
			}
		} 
		else  if (!overSelectable && Controls.JumpPressed())
		{
			DeactivatePools ();
			slotMode = false;
			attackMode = false;
			attackName = null;
			activeSlot = null;
			activeAttack = null;

		}


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Aerial")
			overAerial = true;
		if (other.tag == "Ground")
			overGround = true;
		if (other.tag == "PlayerAttack") 
		{
			overSelectable = true;
			if (!attackMode) activeAttack = other.gameObject;
		}
		if (other.tag == "Slot") 
		{
			onSlot = true;
			if (!slotMode) activeSlot = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Aerial")
			overAerial = false;
		if (other.tag == "Ground")
			overGround = false;
		if (other.tag == "PlayerAttack") 
		{
			overSelectable = false;
		}
		if (other.tag == "Slot") 
		{
			onSlot = false;

		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		
	}

	void SetAttack()
	{
		if (onSlot && Controls.JumpPressed()) 
		{
			if (!attackMode) slotMode = true;
		}
		if (overSelectable && Controls.JumpPressed()) 
		{
			if (!slotMode) attackMode = true;
			attackName = activeAttack.GetComponent<AttackSelectable> ().prefabName;
		}
		
		if (slotMode) 
		{
			if (overSelectable && Controls.JumpPressed()) 
			{
				activeSlot.GetComponent<InputSlot> ().SetAttack (attackName);
				slotMode = false;
				DeactivatePools ();
			}
		}

	}

	void DeactivatePools()
	{
		groundInputs.SetActive (true);
		aerialInputs.SetActive (true);
		if (groundPool.activeSelf) groundPool.GetComponent<AttackPool>().Deactivate();
		if (aerialPool.activeSelf) aerialPool.GetComponent<AttackPool>().Deactivate();
		overSelectable = false;
	}
}
