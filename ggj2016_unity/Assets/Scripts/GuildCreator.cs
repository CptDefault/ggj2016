using UnityEngine;
using System.Collections;
using FMOD.Studio;

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
    public bool LeeroyJenkins;
    private AudioSource _audioSource;
    public bool EndlessMode;

    [FMODUnity.EventRef]
    public string raiders = "event:/Raiders/Raiders_Loop";
    FMOD.Studio.EventInstance raidersEv;
    FMOD.Studio.ParameterInstance raidersAttackParam;

    protected void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        
        raidersEv = FMODUnity.RuntimeManager.CreateInstance(raiders);
        raidersEv.getParameter("Attack", out raidersAttackParam);

        StartCoroutine(PregameMockup());
    }

    private IEnumerator PregameMockup(float intermission = 1)
    {
        _audioSource.Play();
        _audioSource.volume = 1;
        TimelineController.Instance.StopBeats();
        if (PlayerInput.Instance.Health <= 0)
        {
            yield return new WaitForSeconds(2);
            PatchNotes.Instance.ShowFinalGrade();

            while (PatchNotes.Instance.gameObject.activeSelf)
                yield return null;
            
            yield return new WaitForSeconds(1);
            PlayerInput.Instance.Respawn();
        }

        yield return new WaitForSeconds(intermission);
        TimelineController.Instance.StartBeats();
        yield return new WaitForSeconds(0.5f);

        raidersAttackParam.setValue(0);

        raidersEv.start();

        if (!EnablePUG)
            SpawnGuild();
        else
            SpawnPickUpGuild();

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            _audioSource.volume = 1 - t / 0.5f;
            yield return null;
        }
        _audioSource.Stop();
        LeeroyJenkins = false;
        while (EndlessMode && PlayerInput.Instance.Health > 0)
        {
            while (GuildMember.Members.Count > 35)
                yield return null;
            SpawnGuild();            
        }

        yield return new WaitForSeconds(5);
        raidersAttackParam.setValue(1);
    }

    public void ClearAllDead()
    {
        foreach (var member in GuildMember.DeadMembers)
        {
            if (member != null) Destroy(member.gameObject);
        }
        GuildMember.DeadMembers.Clear();

        raidersEv.stop(STOP_MODE.ALLOWFADEOUT);

        StartCoroutine(PregameMockup(10));
    }

    public void SpawnGuild()
    {

        _membersSpawned = 0;
            SpawnGuildMembers(GuildMember.GuildMemberType.Tank, TankCount);
            SpawnGuildMembers(GuildMember.GuildMemberType.MeleeDps, MeleeDpsCount);
            SpawnGuildMembers(GuildMember.GuildMemberType.RangedDps, RangedDpsCount);
            SpawnGuildMembers(GuildMember.GuildMemberType.Healer, HealerCount);
        

        if (LeeroyJenkins)
        {        
            GuildMember.LeeroyJenkins = GuildMember.Members[TankCount + 1];
            GuildMember.LeeroyJenkins.SetGroup(-1);
        }
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
