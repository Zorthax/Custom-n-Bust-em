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
		float speed = 3.0f;
		if (Input.GetKey (Controls.controls.left))
			transform.position -= new Vector3 (speed, 0, 0);
		if (Input.GetKey (Controls.controls.right))
			transform.position += new Vector3 (speed, 0, 0);
		if (Input.GetKey (Controls.controls.down))
			transform.position -= new Vector3 (0, speed, 0);
		if (Input.GetKey (Controls.controls.up))
			transform.position += new Vector3 (0, speed, 0);
	}

	void Update()
	{
		SetAttack ();
		if (onSlot && Input.GetKeyDown (Controls.controls.jump)) 
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
		else  if (!overSelectable && Input.GetKeyDown (Controls.controls.jump))
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
		if (onSlot && Input.GetKeyDown (Controls.controls.jump)) 
		{
			if (!attackMode) slotMode = true;
		}
		if (overSelectable && Input.GetKeyDown (Controls.controls.jump)) 
		{
			if (!slotMode) attackMode = true;
			attackName = activeAttack.GetComponent<AttackSelectable> ().prefabName;
		}
		
		if (slotMode) 
		{
			if (overSelectable && Input.GetKeyDown (Controls.controls.jump)) 
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
