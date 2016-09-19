using UnityEngine;
using System.Collections;

using Image = UnityEngine.UI.Image;

public class AttackSelectable : MonoBehaviour 
{
	public string prefabName;
	public int attackIndex;
	public float animationSpeed;
	public Sprite stillSprite;
	public Sprite[] animatedSprites;
	public float scale;
	public Vector2 position;
	Image rend;

	public bool selected;
	float spriteIndex;

	// Use this for initialization
	void Start () 
	{
		Image[] rends;
		rends = GetComponentsInChildren<Image> ();

		foreach (Image i in rends) 
		{
			if (i.transform != this.transform)
				rend = i;
		}

		rend.rectTransform.localPosition = position;
		rend.rectTransform.localScale = new Vector2 (scale, scale);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		rend.rectTransform.localPosition = position;
		rend.rectTransform.localScale = new Vector2 (scale, scale);
		if (!selected) 
		{
			rend.sprite = stillSprite;
			spriteIndex = 0;
		} 
		else 
		{
			Animation ();

		}
	}

	void Animation()
	{
		rend.sprite = animatedSprites [Mathf.FloorToInt(spriteIndex)];
		if (spriteIndex < animatedSprites.Length - 1)
			spriteIndex += animationSpeed;
		else
			spriteIndex = 0;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Selector")
			selected = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Selector")
			selected = false;
	}
}
