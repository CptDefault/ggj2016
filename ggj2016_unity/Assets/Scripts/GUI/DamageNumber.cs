using System;
using UnityEngine;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    // include at least 6 digits
    public DamageDigit[] digits;

    public TweenAlpha tweenAlpha;
    public TweenPosition tweenPosition;

    public bool Active;

    public void DisplayNumber(int number, Vector3 position)
    {
        StartCoroutine(DisplayNumberCoroutine(number, position));
    }

    private IEnumerator DisplayNumberCoroutine(int number, Vector3 position)
    {
        transform.localPosition = position;

        Active = true;

        var chars = number.ToString().ToCharArray();
        Array.Reverse(chars);

        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();

        // break up into parts
        for (int i = 0; i < chars.Length; i++)
        {
            var c = chars[i];

            if ((i <= (digits.Length - 1)) && digits[i] != null)
            {
                digits[i].Activate(c, Color.red);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(1f);

        tweenAlpha.PlayReverse();

        yield return new WaitForSeconds(0.5f);

        foreach (var damageDigit in digits)
        {
            damageDigit.Reset();
        }

        Active = false;
        DamageNumberManager.Instance.Repool(this);
    }



//    public void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.K)) 
//            DisplayNumber(1219871, Vector3.zero);
//    }
}
