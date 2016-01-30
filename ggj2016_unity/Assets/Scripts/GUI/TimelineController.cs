using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineController : MonoBehaviour
{

    public GameObject TimelineActionPrefab;

    private List<TimelineAction> _actions;
    private UIRoot _uiRoot;

    private float _actionStartingX = 982;

    public float beatsPerMinute = 162;

    private float _actionSpawnTimer = 0;
    private float _actionSpawnInterval;

	// Use this for initialization
	void Start () {
	    _actions = new List<TimelineAction>();
	    _uiRoot = FindObjectOfType<UIRoot>();

	    _actionSpawnInterval = 60f/beatsPerMinute;

        CreateNewAction();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time >= _actionSpawnTimer)
	    {
	        CreateNewAction();
	    }
	}

    private void CreateNewAction()
    {
        GameObject action = (GameObject)Instantiate(TimelineActionPrefab, Vector3.zero, Quaternion.identity);

        action.transform.parent = transform;
        action.transform.localScale = Vector3.one;
        action.transform.localPosition = new Vector2(_actionStartingX, 0);

        _actions.Add(action.GetComponent<TimelineAction>());

        _actionSpawnTimer = Time.time + _actionSpawnInterval;
    }
}
