using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnitySteer2D.Behaviors;

public class GuildMember : MonoBehaviour
{
    private const float OneBeat = 60f / 126;

    [System.Serializable]
    public class GuildMemberConfig
    {
        public GuildMemberType Type;

        public Color Color;

        public bool MeleeRange;
        public bool IgnoreLightAoe;

        public int MaxHealth = 100;

        public float DamageMultiplyer = 1;
        public int HealAmount = 0;
        public int DamagePerTick = 500;
        public int AttackRange = 1;

        public GuildMemberConfig PickUpGroupVariant()
        {
            var clone = (GuildMemberConfig)MemberwiseClone();
            if (Random.value < 0.07f)
                clone.MeleeRange = true;
            return clone;

        }
    }

    public enum GuildMemberType
    {
        Tank,
        MeleeDps,
        RangedDps,
        Healer
    }

    public GuildMemberConfig[] Configs = new []{
        new GuildMemberConfig{Type = GuildMemberType.Tank}, 
        new GuildMemberConfig{Type = GuildMemberType.MeleeDps}, 
        new GuildMemberConfig{Type = GuildMemberType.RangedDps}, 
        new GuildMemberConfig{Type = GuildMemberType.Healer}, 
    };

    public GuildMemberConfig Config
    {
        get { return _overrideConfig ?? Configs[(int) _type]; }
        set
        {
            _overrideConfig = value;

            Debug.Log("Overriding config");

            if (!Config.MeleeRange) //Worse stats because PUG
                Grouping = Random.Range(0, 10) < 7 ? 1 : 2;
            else
            {
                Grouping = 0;
            }
        }
    }

    public int AmountOfDamage
    {
        get { return Config.MaxHealth - Health; }
    }

    public static Dictionary<GameObject, int> CachedGroups = new Dictionary<GameObject, int>();
    public static List<GuildMember> Members = new List<GuildMember>();

    public Collider2D Collider { get; private set; }

    [SerializeField]
    private GuildMemberType _type;
    
    private SpriteRenderer _spriteRenderer;
    private SteerForBoss _steerForBoss;
    public int Grouping;
    public int Health = 100;
    private GuildMemberConfig _overrideConfig;
    public static GuildMember LeeroyJenkins;
    private Biped2D _biped2D;
    private float _lastFullHealth;

    public Transform HealParticles;

    public GuildMemberType MemberType
    {
        get {return _type;}
        set
        {
            _type = value;
            if (_spriteRenderer != null)
                _spriteRenderer.color = Config.Color;
            if (!Config.MeleeRange)
                Grouping = Random.Range(1, 3);
            CachedGroups[gameObject] = Grouping;
            Health = Config.MaxHealth;
        }
    }

    public void SetGroup(int group)
    {
        Grouping = group;
        CachedGroups[gameObject] = Grouping;
    }

    public void TakeDamage(int damage)
    {
        damage = (int)(damage * Config.DamageMultiplyer);

        if (Health == Config.MaxHealth)
            _lastFullHealth = Time.time;

        Health -= damage;
        damage *= Random.Range(980, 1200);
        DamageNumberManager.DisplayDamageNumber(-damage, transform.position);

        if (Health < 0)
            Dead();
    }

    private void Dead()
    {
        _biped2D.enabled = false;
        _spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
        _biped2D.Rigidbody.isKinematic = true;
        _biped2D.Collider.enabled = false;
        CachedGroups.Remove(gameObject);
        Members.Remove(this);
        enabled = false;
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > Config.MaxHealth)
            Health = Config.MaxHealth;

        HealParticles.gameObject.SetActive(false);
        HealParticles.gameObject.SetActive(true);
        amount *= Random.Range(980, 1200);
        DamageNumberManager.DisplayDamageNumber(amount, transform.position);
    }

    protected void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _biped2D = GetComponent<Biped2D>();
        _steerForBoss = GetComponent<SteerForBoss>();
        Collider = GetComponent<Collider2D>();
        _steerForBoss.enabled = false;

        Members.Add(this);
    }

    protected void OnDestroy()
    {
        CachedGroups.Remove(gameObject);
        Members.Remove(this);
    }

    protected void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        if (LeeroyJenkins != null && LeeroyJenkins != this)
            yield return new WaitForSeconds(3);
        if (LeeroyJenkins == this)
            yield return new WaitForSeconds(6);
        else
            yield return new WaitForSeconds(8 + (float)_type * 0.25f);
        _steerForBoss.meleeRange = Config.MeleeRange;
        _steerForBoss.enabled = true;

        if (Config.HealAmount > 0)
            StartCoroutine(HealRoutine());
        if (Config.DamagePerTick > 0)
            StartCoroutine(DamageRoutine());
    }

    private IEnumerator HealRoutine()
    {
        while (true)
        {
            var seconds = TimelineController.OffBeatBy() + OneBeat * 2;
            Debug.Log("Heal routine waiting for " + seconds);
            yield return new WaitForSeconds(seconds);

            if(!enabled)
                yield break;

            int mostDamaged = 0;
            GuildMember member = null;
            foreach (var guildMember in Members)
            {
                if(guildMember._lastFullHealth + 0.8f > Time.time)
                    continue;

                if (guildMember.AmountOfDamage > mostDamaged)
                {
                    mostDamaged = guildMember.AmountOfDamage;
                    member = guildMember;
                }
            }
            if(member != null)
                member.Heal(Config.HealAmount);
        }
        
    }

    private IEnumerator DamageRoutine()
    {
        if(Random.value > 0.5f)
            yield return new WaitForSeconds(TimelineController.OffBeatBy() + OneBeat);

        while (true)
        {
            var seconds = TimelineController.OffBeatBy() + OneBeat * 2;
            yield return new WaitForSeconds(seconds);

            if (!enabled)
                yield break;

            PlayerInput.Instance.DealDamage(Config.DamagePerTick);
        }
    }

    public void PickUpGroup()
    {
        Config = Config.PickUpGroupVariant();
        Health = Config.MaxHealth;
    }
}
