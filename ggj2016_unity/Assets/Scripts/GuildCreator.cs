using UnityEngine;
using System.Collections;

public class GuildCreator : MonoBehaviour
{
    public static GuildCreator Instance { get; private set; }

    public GameObject Prefab;

    public Transform Boss;

    public int TankCount = 2;
    public int MeleeDpsCount = 3;
    public int RangedDpsCount = 3;
    public int HealerCount = 2;

    public float MemberSpacing = 1.5f;

    private int _membersSpawned;
    public bool EnablePUG;

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
    {
        if(!EnablePUG)
            SpawnGuild();
        else
            SpawnPickUpGuild();
    }

    public void SpawnGuild()
    {
        _membersSpawned = 0;
        SpawnGuildMembers(GuildMember.GuildMemberType.Tank, TankCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.MeleeDps, MeleeDpsCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.RangedDps, RangedDpsCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.Healer, HealerCount);

        //GuildMember.LeeroyJenkins = GuildMember.Members[TankCount + 1];
        //GuildMember.LeeroyJenkins.SetGroup(-1);
    }

    public void SpawnPickUpGuild()
    {
        for (int i = 0; i < TankCount + MeleeDpsCount + RangedDpsCount + HealerCount; i++)
        {
            var range = Random.Range(0, 25);
            var goalTanks = TankCount;
            var goalDps = MeleeDpsCount;
            var goalRanged = RangedDpsCount;
            var goalHealer = HealerCount;
            if (range < goalTanks)
            {
                SpawnGuildMembers(GuildMember.GuildMemberType.Tank, 1, true);
                continue;
            }
            range -= goalTanks;
            if (range < goalDps)
            {
                SpawnGuildMembers(GuildMember.GuildMemberType.MeleeDps, 1, true);
                continue;
            }
            range -= goalDps;
            if (range < goalRanged)
            {
                SpawnGuildMembers(GuildMember.GuildMemberType.RangedDps, 1, true);
                continue;
            }
            range -= goalRanged;
            if (range < goalHealer)
            {
                SpawnGuildMembers(GuildMember.GuildMemberType.Healer, 1, true);
                continue;
            }
            range -= goalHealer;

        }
    }

    private void SpawnGuildMembers(GuildMember.GuildMemberType type, int count, bool pug = false)
    {
        for (int i = 0; i < count; i++)
        {
            var position = transform.position;
            position.y += (_membersSpawned % 2) == 0 ? 0 : MemberSpacing + Random.Range(-0.1f, 0.1f);
            position.x += (_membersSpawned / 2) * MemberSpacing + Random.Range(-0.3f, 0.3f); 
            var memberGo = (GameObject)Instantiate(Prefab, position, Quaternion.identity);
            var guildMember = memberGo.GetComponent<GuildMember>();
            guildMember.MemberType = type;
            _membersSpawned++;
            if (pug)
                guildMember.PickUpGroup();
        }
    }
}
