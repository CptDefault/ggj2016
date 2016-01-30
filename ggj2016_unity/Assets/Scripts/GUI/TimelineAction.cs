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

    private bool _succeeded = false;
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
        if (PlayerInput.Instance.CurrentTimelineAction == this && transform.localPosition.x < -30)
        {
            PlayerInput.Instance.CurrentTimelineAction = null;
            if(!_succeeded)
                IsValid();
        }
        else if (transform.localPosition.x < 100 && transform.localPosition.x > 0 && !_failed)
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
        var actionPositionX = Mathf.Abs(transform.localPosition.x);

        if (actionPositionX < 30)
        {
            _tweenColor.to = successColor;
            _tweenColor.PlayForward();

            // increase fun when you make a good hit
            // the lower the value, the more fun
            if((int)actionPositionX == 0)
                FunSystem.Instance.IncrementFunPerSecond(100);      // prevent divide by zero
            else
                FunSystem.Instance.IncrementFunPerSecond(100 / (int)actionPositionX);


            _succeeded = true;
            return true;
        }
        else
        {
            _tweenColor.to = failColor;
            _tweenColor.PlayForward();
            _failed = true;

            FunSystem.Instance.IncrementFunPerSecond(-(int)actionPositionX/2);
            return false;
        }
    }
}
