using UnityEngine;
using System.Collections;

public class AttackPool : MonoBehaviour {

	public GameObject[] allAttacks;
	public float xSeperation;
	public Vector3 firstAttackPosition;
	GameObject[] attacks;


	int attackCount;
	bool active;

	// Use this for initialization
	void Awake () 
	{
		InstantiatePool ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (!active && gameObject.activeSelf) 
		{
			active = true;
			foreach (GameObject g in attacks) 
			{
				g.SetActive (active);
			}
		}


	}

	void InstantiatePool()
	{
		attacks = allAttacks;
		//attacks = new GameObject[allAttacks.Length];
		for (int i = 0; i < allAttacks.Length; i++) 
		{
			attacks[i] = Instantiate (allAttacks [i], transform.position + firstAttackPosition + new Vector3 (xSeperation * i, 0, 0), new Quaternion(0, 0, 0, 0), transform.parent) as GameObject;
			attacks [i].SetActive(false);
		}
	}

	public void Deactivate()
	{
		active = false;
		foreach (GameObject g in attacks) 
		{
			g.SetActive (false);
			gameObject.SetActive (false);
			g.GetComponent<AttackSelectable> ().selected = false;
		}
	}
}
