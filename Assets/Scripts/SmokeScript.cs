using UnityEngine;
using System.Collections;

public class SmokeScript : MonoBehaviour {

	//Animation stuff
	public MySprite currentSprite;
	float spriteIndex = 0;
	SpriteRenderer renderor;
	void Animation()
	{
		renderor.sprite = currentSprite.sprites[Mathf.FloorToInt(spriteIndex)];
		if (spriteIndex < currentSprite.sprites.Length - 1)
			spriteIndex += currentSprite.speed;
		else {
			Destroy (gameObject);
		}
	}
	[System.Serializable]
	public class MySprite
	{
		public float speed;
		public Sprite[] sprites;
	}

	// Use this for initialization
	void Start () 
	{
		renderor = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animation ();
	}
}
