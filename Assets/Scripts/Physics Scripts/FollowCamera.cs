using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    public float followRatio = 1;

    Rigidbody2D rb;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(Camera.main.transform.position.x / followRatio, Camera.main.transform.position.y, transform.position.z), 1);
        
	}
}
