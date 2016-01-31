using UnityEngine;
using System.Collections;
using FMOD;

public class MainMenu : MonoBehaviour
{


    public UISprite strictlySprite;
    public UISprite bossroomSprite;

    public TweenAlpha pressAnyLbel;

    public UILabel starring;

    public UISprite knight;

    private bool _ready;

	// Use this for initialization
	IEnumerator Start ()
	{
//	    strictlySprite.GetComponent<TweenPosition>().PlayForward();
//        bossroomSprite.GetComponent<TweenPosition>().PlayForward();

	    _ready = false;

        yield return new WaitForSeconds(4.5f);

        Camera.main.backgroundColor = Color.white;
        yield return new WaitForSeconds(0.05f);
        // flash

        strictlySprite.color = Color.yellow;
        bossroomSprite.color = Color.yellow;

	    knight.enabled = true;

	    starring.gameObject.SetActive(true);

        Camera.main.backgroundColor = new Color(238f/255, 32f/255, 0);

        yield return new WaitForSeconds(2f);

        pressAnyLbel.PlayForward();

	    _ready = true;
	}
	
	// Update is called once per frame
	void Update () {
//	    if (_ready && Input.anyKey)
//	    {
//	        Application.LoadLevel(1);
//	    }
	}
}
