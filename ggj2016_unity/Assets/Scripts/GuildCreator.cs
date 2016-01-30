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

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
    {
        SpawnGuild();
    }

    public void SpawnGuild()
    {
        _membersSpawned = 0;
        SpawnGuildMembers(GuildMember.GuildMemberType.Tank, TankCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.MeleeDps, MeleeDpsCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.RangedDps, RangedDpsCount);
        SpawnGuildMembers(GuildMember.GuildMemberType.Healer, HealerCount);
    }

    private void SpawnGuildMembers(GuildMember.GuildMemberType type, int count)
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
        }
    }
}
