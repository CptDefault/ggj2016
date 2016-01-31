using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineController : MonoBehaviour
{
    public static TimelineController Instance { get; private set; }
    public GameObject TimelineActionPrefab;
    public GameObject FixedTimelineActionPrefab;

    private List<TimelineAction> _actions;

    private float _actionStartingX = 960;

    public float beatsPerMinute = 162;

    private float _actionSpawnTimer = 0;
    private float _actionSpawnInterval;
    private int _beatsPlayed = 0;

    public float ActionSpawnInterval
    {
        get { return _actionSpawnInterval; }
    }

    private AudioSource _audio;
    private int _skipCount;
    public int SkipBeats;

    public static float OffBeatBy()
    {
        var time = Instance._audio.time;
        time %= 60f/Instance.beatsPerMinute;
        if (time > 60f/Instance.beatsPerMinute/2)
            time -= 60f/Instance.beatsPerMinute;
        return -time;
    }

    protected void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
	    _actions = new List<TimelineAction>();
	    _audio = GetComponent<AudioSource>();

	    _actionSpawnInterval = 60f/beatsPerMinute;

        //StartBeats();
	}

    public void StartBeats()
    {
        _audio.Play();

        _beatsPlayed = 0;
        _skipCount = SkipBeats + 1;
        CreateNewAction();
    }
	
	// Update is called once per frame
	void Update () {
        if (_audio.isPlaying && (_actions.Count > 0 || _skipCount >= 0) && _audio.time >= _actionSpawnTimer)
	    {
	        CreateNewAction();
	    }
	}

    private void CreateNewAction()
    {
        _skipCount--;
        if (_skipCount > 0)
        {
            _beatsPlayed++;
            _actionSpawnTimer = (_beatsPlayed + 0.5f) * _actionSpawnInterval;
            return;
        }

        GameObject action = (GameObject)Instantiate((_beatsPlayed % 10 == 0) ? FixedTimelineActionPrefab : TimelineActionPrefab, Vector3.zero, Quaternion.identity);

        action.transform.parent = transform;
        action.transform.localScale = Vector3.one;
        action.transform.localPosition = new Vector2(_actionStartingX, 0);

        _actions.Add(action.GetComponent<TimelineAction>());

        _beatsPlayed++;

        _actionSpawnTimer = (_beatsPlayed + 0.5f) * _actionSpawnInterval;
    }
}
