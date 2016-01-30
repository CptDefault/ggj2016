using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

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

    public void DisplayText(string text, Vector3 position)
    {
        StartCoroutine(DisplayTextCoroutine(text, position));
        
    }

    private IEnumerator DisplayNumberCoroutine(int number, Vector3 position)
    {
        var color = number > 0 ? DamageNumberManager.Instance.positiveColor : DamageNumberManager.Instance.negativeColor;
        number = Mathf.Abs(number);

        transform.localPosition = position + Vector3.up * 5;

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
                digits[i].Activate(c, color, position.y + 15);
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(1f);

        tweenAlpha.PlayReverse();

        yield return new WaitForSeconds(1f);

        foreach (var damageDigit in digits)
        {
            damageDigit.Reset();
        }

        Active = false;

        yield return null;

        DamageNumberManager.Instance.Repool(this);
    }

    private IEnumerator DisplayTextCoroutine(string text, Vector3 position)
    {
        var color = Color.white;
        color.a = 1;
        transform.localPosition = position + Vector3.up * 5;
        Active = true;
        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();
        digits[0].Activate(text, color);
        for (int i = 0; i < digits.Length; i++)
        {
            digits[i].Reset();
        }
        print("Showing " + text);

        yield return new WaitForSeconds(2f);
        tweenAlpha.PlayReverse();
        yield return new WaitForSeconds(0.5f);
        Active = false;
        digits[0].Reset();
        DamageNumberManager.Instance.Repool(this);
    }
}
