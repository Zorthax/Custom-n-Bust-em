using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour {

    Animator anim;
    int neutralAttack1;
    int frame = 0;

    [System.Serializable]
    public class Attack
    {
        public Sprite[] sprites;
        public PolygonCollider2D[] frameHitboxes;
        public int firstFrameOfAttack;
        public int lastFrameOfAttack;
    }

    public Attack[] attackList;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void InputAttack(KeyCode attackKey, int direction, bool onGround)
    {
        
    }

    public void SetHitBox(int val)
    {

    }
}
