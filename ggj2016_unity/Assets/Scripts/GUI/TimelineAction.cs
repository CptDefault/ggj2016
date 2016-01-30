using UnityEngine;
using System.Collections;

public class TimelineAction : MonoBehaviour
{

    public float speed = 1;

    private UISprite _sprite;

    private Color _originalColor;
    public Color successColor;
    public Color failColor;

    private TweenColor _tweenColor;

    private bool _failed = false;

	// Use this for initialization
	void Start ()
	{
	    _tweenColor = GetComponent<TweenColor>();
	    _sprite = GetComponent<UISprite>();
	    _originalColor = _sprite.color;
	}
	
	// Update is called once per frame
	void Update () {

	    transform.localPosition = transform.localPosition + Vector3.left * speed * Time.deltaTime;

        // become the active
        if (PlayerInput.Instance.CurrentTimelineAction == this && transform.localPosition.x < -50)
        {
            PlayerInput.Instance.CurrentTimelineAction = null;
        }
	    else if (transform.localPosition.x < 100 && !_failed)
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
        if (Mathf.Abs(transform.localPosition.x) < 35)
        {
            _tweenColor.to = successColor;
            _tweenColor.PlayForward();
            return true;
        }
        else
        {
            _tweenColor.to = failColor;
            _tweenColor.PlayForward();
            _failed = true;
            return false;
        }
    }
}
