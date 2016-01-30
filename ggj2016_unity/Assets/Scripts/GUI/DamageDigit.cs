﻿using UnityEngine;
using System.Collections;

public class DamageDigit : MonoBehaviour
{

    public UILabel label;
    public TweenAlpha tweenAlpha;
    public TweenPosition tweenPosition;


    private int _finalNumber;
    private int _currentNumber = -1;
    private float _tickTimer;

    public void Activate(char character, Color col)
    {
        label.text = "0";
        label.color = col;

        _finalNumber = (int)char.GetNumericValue(character);
        _currentNumber = 0;
        _tickTimer = Time.time + 0.05f;

        tweenPosition.from = transform.localPosition;
        tweenPosition.to = new Vector3(transform.localPosition.x, transform.localPosition.y + 15, 0);

        tweenAlpha.ResetToBeginning();
        tweenPosition.ResetToBeginning();

        tweenAlpha.PlayForward();
        tweenPosition.PlayForward();
    }

    public void Update()
    {
        if (_currentNumber != -1 && Time.time > _tickTimer)
        {
            _currentNumber++;
            _tickTimer = Time.time + 0.01f;
            label.text = "" + _currentNumber;

            if (_currentNumber >= _finalNumber)
            {
                label.text = "" + _finalNumber;
                _currentNumber = -1;
            }
        }
    }

    public void Reset()
    {
        tweenAlpha.ResetToBeginning();
    }
}
