﻿using System;
using UnityEngine;
using System.Collections;

public class FunSystem : MonoBehaviour
{

    public static FunSystem Instance;

    public static int TotalFun;
    public static int FunPerSecond;

    public UILabel totalfunLabel;
    public UILabel funPerSecondLabel;

    private float _increaseFunTime;

    public void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
	    StartFun();
	}

    public void StartFun()
    {
        TotalFun = 0;
        FunPerSecond = 0;

        IncrementFunPerSecond(10);

        _increaseFunTime = Time.time + 1;
    }

    public void IncrementFunPerSecond(int increment)
    {
        FunPerSecond += increment;

        funPerSecondLabel.text = String.Format("FUN P/S: {0}", FunPerSecond);
    }

    public void IncreaseFun()
    {
        TotalFun += FunPerSecond;

        totalfunLabel.text = String.Format("TOTAL FUN: {0}", TotalFun);

        _increaseFunTime = Time.time + 1;
    }
	
	// Update is called once per frame
	void Update () {

	    if (Time.time > _increaseFunTime)
	    {
	        IncreaseFun();
	    }
	}
}
