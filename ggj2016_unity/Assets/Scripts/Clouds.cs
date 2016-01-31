using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {
    public float Speed= 0.5f;

    // Update is called once per frame
	void Update ()
	{
	    transform.position += Vector3.left*Time.deltaTime * Speed;

	    if (transform.position.x < -18)
	        transform.position += Vector3.right*(18*3);
	}
}
