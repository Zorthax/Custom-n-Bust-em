using UnityEngine;
using System.Collections;

public class InGameManu : MonoBehaviour {

	[System.Serializable]
	class AttackSelectable
	{
		public Sprite stillSprite;
		public Sprite[] animationSprites;
		public bool selected;
		public int index;
		public Vector3 position;
	}

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
