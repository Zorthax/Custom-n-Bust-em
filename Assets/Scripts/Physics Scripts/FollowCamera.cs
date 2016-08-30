using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    public float followRatio = 1;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(Camera.main.transform.position.x / followRatio, Camera.main.transform.position.y, transform.position.z), 1);
        
	}
}
