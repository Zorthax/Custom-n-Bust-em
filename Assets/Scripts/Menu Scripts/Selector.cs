﻿using UnityEngine;
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
	StateMachine stateMachine;

	// Use this for initialization
	void Start () 
	{
	
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		onSlot = false;
		overSelectable = false;
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
			TurnOnPool ();
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
		
		if (other.tag == "Slot") 
		{
			onSlot = true;
			if (!slotMode) activeSlot = other.gameObject;
		}
		if (other.tag == "PlayerAttack") 
		{
			if (!onSlot) overSelectable = true;
			if (!attackMode) activeAttack = other.gameObject;
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
		if (other.tag == "PlayerAttack") 
		{
			if (!onSlot) overSelectable = true;
			if (!attackMode) activeAttack = other.gameObject;
		}
		if (other.tag == "Slot") 
		{
			onSlot = true;
		}
	}

	void SetAttack()
	{
		if (onSlot && Controls.JumpPressed()) 
		{
			if (!attackMode) slotMode = true;
		}
		else if (overSelectable && Controls.JumpPressed()) 
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

	public void DeactivatePools()
	{
		groundInputs.SetActive (true);
		aerialInputs.SetActive (true);
		if (groundPool.activeSelf) groundPool.GetComponent<AttackPool>().Deactivate();
		if (aerialPool.activeSelf) aerialPool.GetComponent<AttackPool>().Deactivate();
		overSelectable = false;
	}

	public void TurnOnPool()
	{
		if (overAerial) 
		{
			groundInputs.SetActive (false);
			aerialPool.SetActive (true);
			aerialInputs.SetActive(true);
			groundPool.SetActive(false);
		}
		if (overGround)
		{
			groundInputs.SetActive (true);
			aerialPool.SetActive (false);
			aerialInputs.SetActive(false);
			groundPool.SetActive(true);
		}
	}

	class StateMachine
	{
		public Selector sel;
		State currentState;

		public void Update()
		{
			currentState.Update (sel, this);
		}

		public void LateUpdate()
		{
			currentState.LateUpdate (sel);
		}

		public void SetState(State newState)
		{
			if (currentState != newState) 
			{
				currentState.End (sel);
				currentState = newState;
				currentState.Start (sel);
			}
		}

		public void OnTrigger(Collider2D other)
		{
			if (currentState != null)
				currentState.OnTrigger (other);
		}
	}

	class State
	{
		public virtual void Start(Selector sel)
		{
		}
		public virtual void Update(Selector sel, StateMachine sm)
		{
		}
		public virtual void LateUpdate(Selector sel)
		{
		}
		public virtual void OnTrigger(Collider2D other)
		{
		}
		public virtual void End(Selector sel)
		{
		}

	}

	class DefaultMode : State
	{
		bool overSlot;
		bool overAttack;

		public override void Update(Selector sel, StateMachine sm)
		{
			Debug.Log ("Default State");
			if (Controls.JumpPressed ()) 
			{
				if (overSlot)
					sm.SetState (new Selector.SlotMode ());
				else if (overAttack)
					sm.SetState (new Selector.AttackMode ());

			}
		}

		public override void OnTrigger(Collider2D other)
		{
			if (other.tag == "PlayerAttack")
				overAttack = true;
			if (other.tag == "Slot")
				overSlot = true; 
		}

		public override void LateUpdate(Selector sel)
		{
			overSlot = false;
			overAttack = false;
		}
	}

	class SlotMode : State
	{
		bool overAttack;
		bool overSlot;

		public override void Start(Selector sel)
		{
			sel.TurnOnPool ();
		}
		public override void Update(Selector sel, StateMachine sm)
		{
			Debug.Log ("Slot State");
			if (Controls.JumpPressed ()) 
			{
				if (overSlot)
					sm.SetState (new Selector.DefaultMode ());
				else if (overAttack)
					sm.SetState (new Selector.DefaultMode ());

			}
		}

		public override void OnTrigger(Collider2D other)
		{
			if (other.tag == "PlayerAttack")
				overAttack = true;
			if (other.tag == "Slot")
				overSlot = true; 
		}

		public override void LateUpdate(Selector sel)
		{
			overAttack = false;
			overSlot = false;
		}
	}

	class AttackMode : State 
	{
		bool overAttack;
		bool overSlot;

		public override void Start(Selector sel)
		{
		}
		public override void Update(Selector sel, StateMachine sm)
		{
			Debug.Log ("Attack State");
			if (Controls.JumpPressed ()) 
			{
				if (overSlot)
					sm.SetState (new Selector.DefaultMode ());
				else if (overAttack)
					sm.SetState (new Selector.DefaultMode ());

			}
		}

		public override void OnTrigger(Collider2D other)
		{
			if (other.tag == "PlayerAttack")
				overAttack = true;
			if (other.tag == "Slot")
				overSlot = true; 

		}

		public override void LateUpdate(Selector sel)
		{
			overAttack = false;
			overSlot = false;
		}
	}
}
