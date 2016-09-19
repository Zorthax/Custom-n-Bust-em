using UnityEngine;
using System.Collections;

public class InputSlot : MonoBehaviour 
{

	GameObject currentAttack;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SetAttack(string attackName)
	{
		currentAttack = Instantiate (Resources.Load("Selectables/" + attackName), transform) as GameObject;
		currentAttack.transform.localPosition = Vector3.zero;
		currentAttack.transform.localScale = Vector3.one;
	}
}
