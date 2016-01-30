using UnityEngine;
using System.Collections;

public class TimelineAction : MonoBehaviour
{

    public float speed = 1; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	    transform.localPosition = transform.localPosition + Vector3.left * speed * Time.deltaTime;

        // become the active
        if (PlayerInput.Instance.CurrentTimelineAction == this && transform.localPosition.x < -27)
        {
            PlayerInput.Instance.CurrentTimelineAction = null;
        }
	    else if (transform.localPosition.x < 100)
	    {
	        if (PlayerInput.Instance.CurrentTimelineAction == null)
	        {
	            PlayerInput.Instance.CurrentTimelineAction = this;
	        }
        }

	    if (transform.localPosition.x < -982)
	    {
	        Destroy(gameObject);
	    }
	}

    public bool IsValid()
    {
        // within 27 of the origin
        if (Mathf.Abs(transform.localPosition.x) < 27)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
