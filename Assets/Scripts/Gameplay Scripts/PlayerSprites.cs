using UnityEngine;
using System.Collections;

[System.Serializable]
public class MySprite
{
	public bool loop;
	public float speed;
	public Sprite[] sprites;
}

public class PlayerSprites : MonoBehaviour {

	public MySprite idleSprites;
	public MySprite runSprites;
	public MySprite jumpSprites;
	public MySprite airSprites;
	public MySprite knockbackSprites;
	public MySprite shieldSprites;

	float hpTotal;
	float hp;
	float mpTotal;
	float mp;
	float spTotal;
	float sp;

	[Space(5)]
	[Header("GUI")]
	public Vector2 healthBarPosition;
	public Vector2 healthBarSize;
	public Vector2 manaBarPosition;
	public Vector2 manaBarSize;
	public Texture2D shield;
	public Texture2D shieldBack;
	public Vector2 shieldPosition;
	public float shieldRadius;
	public float boldSize;
	//GUI stuff
	Texture2D red;
	Texture2D darkRed;
	Texture2D green;
	Texture2D darkGreen;
	Texture2D black;
	Sprite shieldBar;

	// Use this for initialization
	void Start () 
	{
		
		SetGUIColors ();
		mpTotal = 10;
		mp = 10;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SetGUIColors()
	{
		red = new Texture2D(1, 1);
		red.SetPixel (0, 0, new Color(1, 0, 0, 1));
		red.Apply ();

		darkRed = new Texture2D(1, 1);
		darkRed.SetPixel (0, 0, new Color(0.4f, 0, 0, 1));
		darkRed.Apply ();

		green = new Texture2D(1, 1);
		green.SetPixel (0, 0, new Color(0, 1, 0, 1));
		green.Apply ();

		darkGreen = new Texture2D(1, 1);
		darkGreen.SetPixel (0, 0, new Color(0, 0.4f, 0, 1));
		darkGreen.Apply ();

		black = new Texture2D(1, 1);
		black.SetPixel (0, 0, new Color(0, 0, 0, 1));
		black.Apply ();
	}

	public void SetStats(float _hpTotal, float _hp, float _spTotal, float _sp)
	{
		hpTotal = _hpTotal;
		hp = _hp;
		spTotal = _spTotal;
		sp = _sp;
	}

	void OnGUI()
	{
		//Health bar
		GUI.DrawTexture (new Rect (healthBarPosition.x - boldSize / 2, healthBarPosition.y - boldSize / 2, healthBarSize.x + boldSize, healthBarSize.y + boldSize), black);
		GUI.DrawTexture (new Rect (healthBarPosition.x, healthBarPosition.y, healthBarSize.x, healthBarSize.y), darkRed);
		GUI.DrawTexture (new Rect (healthBarPosition.x, healthBarPosition.y, healthBarSize.x / (hpTotal / hp), healthBarSize.y), red);

		//Mana bar
		GUI.DrawTexture (new Rect (manaBarPosition.x - boldSize / 2, manaBarPosition.y - boldSize / 2, manaBarSize.x + boldSize, manaBarSize.y + boldSize), black);
		GUI.DrawTexture (new Rect (manaBarPosition.x, manaBarPosition.y, manaBarSize.x, manaBarSize.y), darkGreen);
		GUI.DrawTexture (new Rect (manaBarPosition.x, manaBarPosition.y, manaBarSize.x / (mpTotal / mp), manaBarSize.y), green);

		float scale = shieldRadius / (spTotal / sp);

		//Shield bar
		GUI.DrawTexture (new Rect (shieldPosition.x - ((shieldRadius + boldSize) / 2), shieldPosition.y - ((shieldRadius + boldSize) / 2), shieldRadius + boldSize, shieldRadius + boldSize), shieldBack);
		GUI.DrawTexture (new Rect (shieldPosition.x - (scale / 2), shieldPosition.y - (scale / 2), scale, scale), shield);
	}
}
