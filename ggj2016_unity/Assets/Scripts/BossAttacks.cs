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


    [FMODUnity.EventRef]
    public string bossOverheadSound = "event:/Boss/Boss_OHSmash";
    [FMODUnity.EventRef]
    public string bossWhirlwindSound = "event:/Boss/Boss_Whirlattack";
    [FMODUnity.EventRef]
    public string bossDashSound = "event:/Boss/Boss_Dash";

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
        _characterController.WeaponAnimation = CharacterController.AnimType.Smash + (int)attack;

        var damage = 50;
        switch (attack)
        {
            case Attacks.Smash:
                damage = 50*13; //Team damage; expect to hit about 13
                yield return new WaitForSeconds(TimelineController.OffBeatBy());
                yield return new WaitForSeconds(OneBeat * 2f);
                FMODUnity.RuntimeManager.PlayOneShot(bossOverheadSound, Camera.main.transform.position);
                yield return new WaitForSeconds(OneBeat * 1f);
                aoe.DealDamage(damage);

                ScreenShakeManager.ScreenShake(3);
                yield return new WaitForSeconds(0.2f);
                break;
            case Attacks.Whirlwind:
                int tickAmount = 15;
                FMODUnity.RuntimeManager.PlayOneShot(bossWhirlwindSound, Camera.main.transform.position);
                for (int i = 0; i < tickAmount; i++)
                {
                    yield return new WaitForSeconds((i == 0 ? TimelineController.OffBeatBy() : 0) + OneBeat/4);
                    ScreenShakeManager.ScreenShakeContinuous(.7f);
                    aoe.DealDamage(damage / tickAmount);
                }
                break;
            case Attacks.Dash:
                heading.Normalize();
                float goUntil = Time.time + TimelineController.OffBeatBy() + OneBeat;
                FMODUnity.RuntimeManager.PlayOneShot(bossDashSound, Camera.main.transform.position);
                while (Time.time < goUntil)
                {
                    _characterController.SetDesiredSpeed(heading, true);
                    ScreenShakeManager.ScreenShakeContinuous(.7f);
                    yield return null;
                }
                break;
            case Attacks.Throw:
                FMODUnity.RuntimeManager.PlayOneShot(bossDashSound, Camera.main.transform.position);
                Debug.Log("Start throw");
                yield return new WaitForSeconds(TimelineController.OffBeatBy() + OneBeat * 2);
                Debug.Log("End throw");

                break;
            default:
                yield return new WaitForSeconds(TimelineController.OffBeatBy() + OneBeat);
                aoe.DealDamage(damage);
                break;
        }

        if (_characterController.WeaponAnimation == CharacterController.AnimType.Smash + (int)attack)
            _characterController.WeaponAnimation = CharacterController.AnimType.Idle;
        aoe.gameObject.SetActive(false);
    }
}
