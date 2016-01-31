using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;

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

//    public TweenScale needleTween;

    [FMODUnity.EventRef]
    public string music = "event:/Music/InGame";
    FMOD.Studio.EventInstance musicEv;
    FMOD.Studio.ParameterInstance musicEndParam;
    public string musicOutOfGame = "event:/Music/Idling";

    public float ActionSpawnInterval
    {
        get { return _actionSpawnInterval; }
    }

    public static float TimeInBeats
    {
        get { return Instance.AudioTime / 60 * Instance.beatsPerMinute * 4; }
    }

    private AudioSource _audio;
    private int _skipCount;
    public int SkipBeats;
    private float _audioStartTime = -1;

    public AudioClip hitSound;

    public void PlayHitSound()
    {
        _audio.PlayOneShot(hitSound);
    }

    public void PlayOneshot(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    public static float OffBeatBy()
    {
        var time = Instance.AudioTime;
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


        musicEv = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEv.getParameter("End", out musicEndParam);

        //StartBeats();
	}

//    public void ResetNeedle()
//    {
//        needleTween.ResetToBeginning();        
//    }

    public void StartBeats()
    {
        //_audio.Play();
        //_audio.volume = 0.1f;
        _beatsPlayed = 0;
        _skipCount = SkipBeats + 1;
        //FMODUnity.RuntimeManager.PlayOneShot(music, Camera.main.transform.position);
        musicEv.start();

        _audioStartTime = Time.time;
        CreateNewAction();
    }
	
	// Update is called once per frame
	void Update () {
        if (AudioTime >= 0 && (_actions.Count > 0 || _skipCount >= 0) && AudioTime >= _actionSpawnTimer && PlayerInput.Instance.Health > 0)
	    {
	        CreateNewAction();
	    }
	}

    private float AudioTime
    {
        get
        {
            return _audioStartTime >= 0 ? Time.time - _audioStartTime : -1;
            //_audio.time; 
        }
    }

    private void CreateNewAction()
    {
//        needleTween.PlayForward();

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

    public void StopBeats()
    {
        print("Stop the audio?");
        //_audio.Stop();
        musicEv.stop(STOP_MODE.ALLOWFADEOUT);

        _audioStartTime = -1;
        //FMODUnity.RuntimeManager.PlayOneShot(musicOutOfGame, Camera.main.transform.position);

    }
}
