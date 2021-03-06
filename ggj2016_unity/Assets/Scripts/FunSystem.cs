﻿using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class FunSystem : MonoBehaviour
{

    public static FunSystem Instance;

    public static int TotalFun;
    public static int FunPerSecond;

    public UILabel totalfunLabel;
    public UILabel funPerSecondLabel;

    private float _increaseFunTime;

    // popup
    public UILabel fpsPopupLabel;
    public TweenAlpha fpsTweenAlpha;
    public TweenPosition fpsTweenPositionPositive;
    public TweenPosition fpsTweenPositionNegative;

    // incoming raid
    public UILabel incomingRaid;
    public TweenAlpha incomingRaidAlpha;
    public UILabel guildNameLabel;
    public TweenAlpha guildAlpha;
    public static float RaidStartTime { get; set; }

    public void Awake()
    {
        Instance = this;
        incomingRaid.enabled = false;
        guildNameLabel.enabled = false;
    }

	// Use this for initialization
	void Start () {
	    StartFun();
	}

    public void StartFun()
    {
        TotalFun = 0;
        FunPerSecond = 0;//10;
        funPerSecondLabel.text = String.Format("FUN P/S: {0}", FunPerSecond);
        _increaseFunTime = Time.time + 1;
    }

    public void IncrementFunPerSecond(int increment)
    {
        FunPerSecond += increment;

        funPerSecondLabel.text = String.Format("FUN P/S: {0}", FunPerSecond);

        fpsPopupLabel.text = string.Format("{0}{1} FP/S", increment > 0 ? "+" : "", increment);
        fpsTweenAlpha.ResetToBeginning();
        fpsTweenAlpha.PlayForward();

        if (increment > 0)
        {
            fpsTweenPositionPositive.ResetToBeginning();
            fpsTweenPositionPositive.PlayForward();
        }
        else
        {
            fpsTweenPositionNegative.ResetToBeginning();
            fpsTweenPositionNegative.PlayForward();
        }
    }

    public void IncreaseFun()
    {
        TotalFun += FunPerSecond;

        totalfunLabel.text = String.Format("TOTAL FUN: {0}", TotalFun);

        _increaseFunTime = Time.time + 1;
    }

    public static string GetFinalGrade()
    {
//        if (TotalFun > 20000)
//            return "A+";
//        else if(TotalFun > 10000)
//            return "A";
//        else
//        {
//            return "F";
//        }

        string[] grades =
        {
            "A+", "A", "B+", "B", "C+", "C", "D+", "D", "E+", "F"
        };

        return grades[Random.Range(0, grades.Length - 1)];
    }
	
	// Update is called once per frame
	void Update () {

	    if (Time.time > _increaseFunTime && PlayerInput.Instance.Health > 0)
	    {
	        IncreaseFun();
	    }
	}

    public void IncomingRaid(string guildName)
    {
        return;
        
        guildNameLabel.text = guildName;
        guildAlpha.ResetToBeginning();
        guildNameLabel.alpha = 0;
        incomingRaid.alpha = 0;

        StartCoroutine(IncomingRaidRoutine());
    }

    private IEnumerator IncomingRaidRoutine()
    {
        incomingRaid.enabled = true;
//        for (int i = 0; i < 10; i++)
//        {
//            incomingRaid.enabled = true;
//            yield return new WaitForSeconds(0.1f);
//            incomingRaid.enabled = false;
//        }
//        incomingRaid.enabled = true;

        incomingRaidAlpha.ResetToBeginning();
        incomingRaidAlpha.PlayForward();

//        yield return new WaitForSeconds(1f);

        guildNameLabel.enabled = true;
//        guildAlpha.PlayForward();

        yield return new WaitForSeconds(2.5f);

        incomingRaidAlpha.PlayReverse();

        yield return new WaitForSeconds(0.5f);

//        guildAlpha.PlayReverse();

        yield return new WaitForSeconds(2f);

        incomingRaid.enabled = false;
        guildNameLabel.enabled = false;
    }
}
