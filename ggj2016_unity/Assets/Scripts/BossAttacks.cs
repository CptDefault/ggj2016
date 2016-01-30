using System;
using UnityEngine;
using System.Collections;

public class BossAttacks : MonoBehaviour
{
    public const float OneBeat = 60f/126;

    public enum Attacks
    {
        Smash,
        Throw,
        Dash,
        Whirlwind
    }

    public AOE[] NonStrictAoe = new AOE[4];

    public void Attack(Attacks attack, bool isStrict = false)
    {
        StartCoroutine(PerformAoe(attack, isStrict));
    }

    private IEnumerator PerformAoe(Attacks attack, bool isStrict)
    {
        Debug.Log("Attack " + attack);
        var aoe = NonStrictAoe[(int)attack];
        if(aoe == null)
            yield break;
        aoe.gameObject.SetActive(true);

        yield return new WaitForSeconds(OneBeat);
        aoe.gameObject.SetActive(false);
    }
}
