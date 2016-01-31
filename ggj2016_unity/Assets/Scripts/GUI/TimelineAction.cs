using UnityEngine;
using System.Collections;

public class TimelineAction : MonoBehaviour
{

    public float speed = 1;

    protected UISprite _sprite;

    protected Color _originalColor;
    public Color successColor;
    public Color failColor;

    protected TweenColor _tweenColor;

    protected bool _succeeded = false;
    protected bool _failed = false;

    public bool IsFixed;
    public int fixedLength;
    public int fixedAction;
    public UILabel mashLabels;

	// Use this for initialization
	void Start ()
	{
	    _tweenColor = GetComponent<TweenColor>();
	    _sprite = GetComponent<UISprite>();
	    _originalColor = _sprite.color;

	    if (IsFixed)
	    {
	        _sprite.height = 60;
	        _sprite.width = fixedLength;

            // get a random action
	        fixedAction = Random.Range(0, 3);

	        switch (fixedAction)
	        {
	            case 0:
                    _sprite.color = new Color(92f / 255, 176f / 255, 72f / 255);
	                mashLabels.text = "MASH (A)";
	                break;
	            case 1:
                    _sprite.color = new Color(192f / 255, 74f / 255, 55f / 255);
                    mashLabels.text = "MASH (B)";

	                break;
	            case 2:
	                _sprite.color = new Color(58f/255, 152f/255, 196f/255);
                    mashLabels.text = "MASH (X)";
	                break;
	            case 3:
                    _sprite.color = new Color(204f / 255, 169f / 255, 51f / 255);
                    mashLabels.text = "MASH (Y)";
	                break;
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {

	    transform.localPosition = transform.localPosition + Vector3.left * speed * Time.deltaTime;

        // become the active
        if (PlayerInput.Instance.CurrentTimelineAction == this && ((!IsFixed && transform.localPosition.x < -30) || (IsFixed && transform.localPosition.x < -fixedLength)))
        {
            PlayerInput.Instance.CurrentTimelineAction = null;
            if(!_succeeded && !IsFixed)
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
        if (IsFixed)
        {
            if (_failed)
                return false;
            
            var positionX = transform.localPosition.x;

            if (positionX > -fixedLength && positionX <= 0)
            {
                _tweenColor.from = _sprite.color;
                _tweenColor.to = successColor;
                _tweenColor.ResetToBeginning();
                _tweenColor.PlayForward();

                FunSystem.Instance.IncrementFunPerSecond(50);
                return true;
            }
            else
            {
                _tweenColor.to = failColor;
                _tweenColor.PlayForward();

                _failed = true;
                FunSystem.Instance.IncrementFunPerSecond(-10);

                return false;
            }
        }

        var actionPositionX = Mathf.Abs(transform.localPosition.x);
        
        if (actionPositionX < 30)
        {
            _tweenColor.to = successColor;
            _tweenColor.PlayForward();

            // increase fun when you make a good hit
            // the lower the value, the more fun
            if((int)actionPositionX == 0)
                FunSystem.Instance.IncrementFunPerSecond(50);      // prevent divide by zero
            else
                FunSystem.Instance.IncrementFunPerSecond((int)(100f /actionPositionX));

            _succeeded = true;
            return true;
        }
        else
        {
            _tweenColor.to = failColor;
            _tweenColor.PlayForward();
            _failed = true;

            FunSystem.Instance.IncrementFunPerSecond(-(int)actionPositionX/2);

            PlayerInput.Instance.CurrentTimelineAction = null;

            return false;
        }
    }
}
