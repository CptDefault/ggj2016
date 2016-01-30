using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;

public class SteerForBoss : Steering2D
{
    public bool meleeRange;

    protected override Vector2 CalculateForce()
    {
        var bossPosition = GuildCreator.Instance.Boss.position;

        if (!meleeRange)
        {
            var toBoss = transform.position - bossPosition;
            var dist = toBoss.magnitude;

            if (dist < 2)
                dist = 0.3f;

            bossPosition += toBoss * 3 / dist;
        }

        return Vehicle.GetSeekVector(bossPosition);

        //return Vector2.zero;
    }
}
