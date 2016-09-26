using UnityEngine;
using System.Collections;

public class OpenCustomizeMenu : MonoBehaviour {

	public GameObject customMenuMain;
	bool menu = false;
	bool pressed = false;
	bool playerContact = true;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (playerContact) 
		{
			if (Controls.InteractPressed())
				pressed = true;

			//turn off menu
			if (menu && pressed && Controls.InteractReleased ()) 
			{
				customMenuMain.GetComponent<CustomizeMenu> ().MenuOff();
				menu = false;
			}
				
			//Turn on menu
			else if (!menu && pressed && Controls.InteractReleased ()) 
			{
				customMenuMain.GetComponent<CustomizeMenu> ().MenuOn();
				menu = true;
				pressed = false;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerContact = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerContact = false;
		}
	}
}
