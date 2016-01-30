using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineController : MonoBehaviour
{

    public GameObject TimelineActionPrefab;

    private List<TimelineAction> _actions;

    private float _actionStartingX = 960;

    public float beatsPerMinute = 162;

    private float _actionSpawnTimer = 0;
    private float _actionSpawnInterval;
    private int _beatsPlayed = 0;

    private AudioSource _audio;

	// Use this for initialization
	void Start () {
	    _actions = new List<TimelineAction>();
	    _audio = GetComponent<AudioSource>();

	    _actionSpawnInterval = 60f/beatsPerMinute;

        StartBeats();
	}

    public void StartBeats()
    {
        _audio.Play();

        _beatsPlayed = 0;

        CreateNewAction();
    }
	
	// Update is called once per frame
	void Update () {
	    if (_actions.Count > 0 && _audio.time >= _actionSpawnTimer)
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

        _beatsPlayed++;

        _actionSpawnTimer = _beatsPlayed * _actionSpawnInterval;
    }
}
