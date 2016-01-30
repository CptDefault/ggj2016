using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;

public class SteerForAOE : Steering2D
{
    private GuildMember _guildMember;

    protected override void Awake()
    {
        _guildMember = GetComponent<GuildMember>();
        base.Awake();
    }

    protected override Vector2 CalculateForce()
    {
        foreach (var activeAoe in AOE.ActiveAoes)
        {
            if (activeAoe.TeamAoe)
            {

                return (Vector2)(activeAoe.transform.position - transform.position);
                continue;
            }
            

            if (!activeAoe.HeavyAoe && _guildMember.Config.IgnoreLightAoe)
                continue;

            var aoe = activeAoe.Collider;
            var position = transform.position;
            var closestPoint = aoe.transform.position;//bounds.ClosestPoint(transform.position);
            var desiredVelocity = position - closestPoint;
            desiredVelocity /= desiredVelocity.sqrMagnitude;
            if (aoe.IsTouching(Vehicle.Collider))
            {
                desiredVelocity *= 10;
                //desiredVelocity = -desiredVelocity.normalized * 100;
            }
                /*
            else
            {
                desiredVelocity /= desiredVelocity.sqrMagnitude;
            }*/

            return (Vector2)desiredVelocity;
        }

        return Vector2.zero;
    }
}
