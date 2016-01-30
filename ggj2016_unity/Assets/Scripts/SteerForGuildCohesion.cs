using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;

public class SteerForGuildCohesion : SteerForNeighbors2D
{
    private GuildMember _guildMember;

    protected override void Awake()
    {
        _guildMember = GetComponent<GuildMember>();
        base.Awake();
    }

    public override Vector2 CalculateNeighborContribution(Vehicle2D other)
    {
        //if (_guildMember.Grouping != 0)
        {
            if (GuildMember.CachedGroups.ContainsKey(other.gameObject))
            {
                var otherGroup = GuildMember.CachedGroups[other.gameObject];
                if (//otherGroup != 0 && 
                    otherGroup != _guildMember.Grouping)
                    return Vector2.zero;
            }
        }

        // accumulate sum of forces leading us towards neighbor's positions
        var distance = other.Position - Vehicle.Position;
        var sqrMag = distance.sqrMagnitude;
        // Provide some contribution, but diminished by the distance to 
        // the vehicle.
        distance *= 1 / sqrMag;
        return distance;
    }

    private void OnDrawGizmos()
    {
        if (_guildMember != null)
        {
            switch (_guildMember.Grouping)
            {
                case 0:
                    Gizmos.color = Color.red;
                    break;
                case 1:
                    Gizmos.color = Color.green;
                    break;
                case 2:
                    Gizmos.color = Color.blue;
                    break;
                default:
                    Gizmos.color = Color.magenta;
                    break;


            }
            Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);
        }
#if DEBUG_COMFORT_DISTANCE
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, ComfortDistance);
#endif
    }
}
