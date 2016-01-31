using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AOE : MonoBehaviour
{
    public static readonly List<AOE> ActiveAoes = new List<AOE>();

    public Collider2D Collider;

    public bool HeavyAoe;
    public bool TeamAoe;

    protected void Awake()
    {
        if(Collider == null) Collider = GetComponent<Collider2D>();
    }

    protected void OnEnable()
    {
        ActiveAoes.Add(this);
    }
    protected void OnDisable()
    {
        ActiveAoes.Remove(this);
    }

    public void DealDamage(int amount)
    {
        if (TeamAoe && GuildMember.Members.Count > 12)
        {
            DealTeamDamage(amount);
            return;
        }
        for (int index = 0; index < GuildMember.Members.Count; index++)
        {
            var guildie = GuildMember.Members[index];
            if (Collider.IsTouching(guildie.Collider))
            {
                guildie.TakeDamage(amount);
            }
        }
    }

    public void DealTeamDamage(int amount)
    {
        var guildiesHit = new List<GuildMember>();
        foreach (var guildie in GuildMember.Members)
        {
            if (Collider.IsTouching(guildie.Collider))
            {
                guildiesHit.Add(guildie);
            }
        }
        var count = guildiesHit.Count;
        if(count == 0)
            return;
        //Debug.LogFormat( "dealing {0} damage to {1} targets", amount/count, count);
        foreach (var guildie in guildiesHit)
        {
            guildie.TakeDamage(amount / count);
        }
    }
}
