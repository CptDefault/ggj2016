using UnityEngine;

namespace UnitySteer2D.Behaviors
{
    /// <summary>
    /// Steers a vehicle to match velocity (speed + heading) with detected neighbors
    /// </summary>
    /// <remarks> This behavior serves a similar purpose to SteerForAlignment. However,
    /// SteerForAlignment has the deficiency that it returns a pure orientation vector,
    /// whereas SteerForEvasion and SteerForCohesion return a *distance* from the vehicle's
    /// position.  Steering to match the neighbors velocity is more consistent with the others.
    /// </remarks>
    /// <seealso cref="SteerForAlignment"/>
    [AddComponentMenu("UnitySteer2D/Steer/... for Matching Velocity (Neighbour)")]
    [RequireComponent(typeof (SteerForNeighborGroup2D))]
    public class SteerForMatchingVelocity2D : SteerForNeighbors2D
    {
        private GuildMember _guildMember;

        protected override void Awake()
        {
            _guildMember = GetComponent<GuildMember>();
            base.Awake();
        }

        public override Vector2 CalculateNeighborContribution(Vehicle2D other)
        {
            if (_guildMember != null && GuildMember.CachedGroups.ContainsKey(other.gameObject))
            {
                var otherGroup = GuildMember.CachedGroups[other.gameObject];
                if (otherGroup != _guildMember.Grouping)
                    return Vector2.zero;
            }

            // accumulate sum of neighbors' velocities
            return other.Velocity;
        }
    }
}