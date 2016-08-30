using UnityEngine;
using System.Collections;

public class EnemyDrop : MonoBehaviour {

	float waitTime = 0.75f;
	Vector2 direction;
	Transform player;


	// Use this for initialization
	void Start () 
	{
		direction = new Vector2 (Random.Range (-0.01f, 0.01f), Random.Range (-0.01f, 0.01f));
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (waitTime > 0) {
			transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y, 0);
			waitTime -= Time.deltaTime;
		} else
			transform.position = Vector2.MoveTowards (transform.position, player.position, 0.1f);
	}
}
