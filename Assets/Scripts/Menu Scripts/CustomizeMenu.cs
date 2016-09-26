using UnityEngine;
using System.Collections;

public class CustomizeMenu : MonoBehaviour {

	public GameObject player;
	public GameObject[] customizeMenuItems;
	public GameObject[] inputSlots;
	AttackScript attacks;

	// Use this for initialization
	void Start () 
	{
		attacks = player.GetComponentInChildren<AttackScript> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void MenuOn()
	{
		foreach (GameObject g in customizeMenuItems) 
		{
			g.SetActive (true);
		}
		player.SetActive (false);
	}

	public void MenuOff()
	{
		//Assign attacks from each input slot
		foreach (GameObject g in inputSlots) 
		{
			InputSlot slot = g.GetComponent<InputSlot> ();
			if (slot != null) 
			{
				slot.SetIndex (attacks);
			}
		}

		foreach (GameObject g in customizeMenuItems) 
		{
			g.SetActive (false);
		}

		player.SetActive (true);
	}
}
