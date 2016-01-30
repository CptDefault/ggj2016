using System;
using UnityEngine;
using System.Collections;

public class BossAttacks : MonoBehaviour
{
    public const float OneBeat = 60f/126;

    public Transform AoePivot;

    public enum Attacks
    {
        Smash,
        Throw,
        Dash,
        Whirlwind
    }

    public AOE[] NonStrictAoe = new AOE[4];
    private CharacterController _characterController;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();        
    }

    public void Attack(Attacks attack, bool isStrict = false)
    {
        /*if (attack == Attacks.Dash)
            StartCoroutine(PerformDash(isStrict));
        else*/
            StartCoroutine(PerformAoe(attack, isStrict));
    }


    private IEnumerator PerformAoe(Attacks attack, bool isStrict)
    {
        var aoe = NonStrictAoe[(int)attack];
        if(aoe == null)
            yield break;
        //aoe.gameObject.SetActive(true);
        var heading = _characterController.Heading;
        AoePivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(heading.y, heading.x) / Mathf.PI * 180 + 90);

        yield return new WaitForSeconds(TimelineController.OffBeatBy());
        aoe.gameObject.SetActive(true);

        var damage = 50;
        switch (attack)
        {
            case Attacks.Smash:
                damage = 50*13; //Team damage; expect to hit about 13
                yield return new WaitForSeconds(TimelineController.OffBeatBy() + OneBeat * 3f);
                aoe.DealDamage(damage);
                break;
            case Attacks.Whirlwind:
                int tickAmount = 7;
                for (int i = 0; i < tickAmount; i++)
                {
                    yield return new WaitForSeconds((i == 0 ? TimelineController.OffBeatBy() : 0) + OneBeat/4);
                    aoe.DealDamage(damage / tickAmount);
                }
                break;
            case Attacks.Dash:
                heading.Normalize();
                float goUntil = Time.time + TimelineController.OffBeatBy() + OneBeat;
                while (Time.time < goUntil)
                {
                    _characterController.SetDesiredSpeed(heading, true);
                    yield return null;
                }
                break;
            default:
                yield return new WaitForSeconds(TimelineController.OffBeatBy() + OneBeat);
                aoe.DealDamage(damage);
                break;
        }
        aoe.gameObject.SetActive(false);
    }
}
