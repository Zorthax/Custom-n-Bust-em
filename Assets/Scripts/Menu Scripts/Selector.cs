using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {

	public GameObject groundInputs;
	public GameObject aerialInputs;

	bool overAerial;
	bool overGround;
	bool onSlot;

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
		if (onSlot && Input.GetKeyDown (Controls.controls.jump)) 
		{
			if (overAerial)
				groundInputs.SetActive (false);
			if (overGround)
				aerialInputs.SetActive (false);
		} 
		else  if (Input.GetKeyDown (Controls.controls.jump))
		{
			groundInputs.SetActive (true);
			aerialInputs.SetActive (true);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Aerial")
			overAerial = true;
		if (other.tag == "Ground")
			overGround = true;
		if (other.tag == "Slot")
			onSlot = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Aerial")
			overAerial = false;
		if (other.tag == "Ground")
			overGround = false;
		if (other.tag == "Slot")
			onSlot = false;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		
	}
}
