using UnityEngine;
using System.Collections;

public class InputSlot : MonoBehaviour 
{
	GameObject currentAttack;
	public string attackInput;

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
		if (currentAttack != null)
			Destroy (currentAttack);
		
		currentAttack = Instantiate (Resources.Load("Selectables/" + attackName), transform) as GameObject;
		currentAttack.transform.localPosition = Vector3.zero;
		currentAttack.transform.localScale = Vector3.one;
	}

	public void SetIndex(AttackScript attacks)
	{
		Debug.Log ("Set Index");
		if (currentAttack != null) {
			AttackSelectable a = currentAttack.GetComponent<AttackSelectable> ();
			if (a != null) {
				attacks.SetAttack (attackInput, a.attackIndex);
			}
		}
	}
}
