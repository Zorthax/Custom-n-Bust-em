using UnityEngine;
using System.Collections;

public class AttackList : MonoBehaviour {

    Animator anim;

    [System.Serializable]
    public class Attack
    {
        public AnimationClip attackAnimation;
        public PolygonCollider2D[] frameHitboxes;
    }

    public Attack[] attackList;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void InputAttack()
    {

    }
}
