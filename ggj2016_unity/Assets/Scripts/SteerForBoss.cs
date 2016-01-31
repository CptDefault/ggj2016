using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;

public class SteerForBoss : Steering2D
{
    public enum BossFightStage
    {
        GoFightHim,
        GoLootBody,
        GoRescuePerson,
        GoToBridge,
        Leave
    }
    public bool meleeRange;
    private BossFightStage bossFightStage;
    private float _goToBridgeStart;

    protected override Vector2 CalculateForce()
    {
        var bossPosition = GuildCreator.Instance.Boss.position;

        switch (bossFightStage)
        {
            case BossFightStage.GoFightHim:
                if (!meleeRange)
                {
                    var toBoss = transform.position - bossPosition;
                    var dist = toBoss.magnitude;

                    if (dist < 2)
                        dist = 0.3f;

                    bossPosition += toBoss * 3 / dist;
                }
                if (PlayerInput.Instance.Health <= 0)
                {
                    StartCoroutine(GameOverRoutine());
                }

                return Vehicle.GetSeekVector(bossPosition);
            case BossFightStage.GoLootBody:
                return Vehicle.GetSeekVector(bossPosition);
            case BossFightStage.GoRescuePerson:
                break;
            case BossFightStage.GoToBridge:
                return ((new Vector3(6,.6f)) - transform.position).normalized * 5;
            case BossFightStage.Leave:
                return ((new Vector3(12, .6f)) - transform.position).normalized * 5;
        }


        return Vector2.zero;
    }

    private IEnumerator GameOverRoutine()
    {
        bossFightStage = BossFightStage.GoLootBody;


        yield return new WaitForSeconds(5 + Random.value);
        GetComponent<SteerForTether2D>().enabled = false;
        bossFightStage = BossFightStage.GoToBridge;


        var sqrMagnitude = (transform.position - new Vector3(6, .6f)).sqrMagnitude;
        float timeout = Time.time + 10;
        while (sqrMagnitude > 0.5f && timeout > Time.time)
        {
            sqrMagnitude = (transform.position - new Vector3(6, .6f)).sqrMagnitude;

            yield return null;
        }
        PlayerInput.Instance.GetComponent<Collider2D>().enabled = false;
        bossFightStage = BossFightStage.Leave;
        timeout = Time.time + 5;
        while (transform.position.x < 12 && timeout > Time.time)
            yield return null;

        if (GuildMember.Members.Count <= 1)
        {
            GuildCreator.Instance.ClearAllDead();
        }

        Destroy(gameObject);

    }
}
