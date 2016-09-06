using UnityEngine;
using System.Collections;

public class ThrownSword : MonoBehaviour {

	Rigidbody2D rb;
	Vector3 player;
	public float speed = 6;
	public float radius = 6;
	public float manaCost = 2;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody2D> ();
		player = GameObject.FindGameObjectWithTag ("Player").transform.position;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().mp -= manaCost;
	}
	
	// Update is called once per frame
	void Update () 
	{
		rb.velocity = new Vector2 (speed * transform.lossyScale.x, 0);

		if (Mathf.Abs (player.x - transform.position.x) >= radius)
			Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy") 
		{
			other.GetComponent<EnemyBasics> ().ApplyHit (Vector2.zero, 2, 0.2f);
		}
	}
}
