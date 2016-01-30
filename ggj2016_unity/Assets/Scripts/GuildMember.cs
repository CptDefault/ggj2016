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

    public UISprite healthSprite;

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
            UpdateHealthBar();
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
        {
            if (LeeroyJenkins != null)
                Health /= 10;
            _lastFullHealth = Time.time;
        }

        Health -= damage;
        damage *= Random.Range(980, 1200);
        DamageNumberManager.DisplayDamageNumber(-damage, transform.position);

        UpdateHealthBar();
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

        StartCoroutine(DeadMessage());

        healthSprite.gameObject.SetActive(false);
    }

    private IEnumerator DeadMessage()
    {
        yield return new WaitForSeconds(Random.Range(1, 3f));

        if (Members.Count == 0)
        {
            string[] messages = new[] { "you guys suck.", "wow. that went great.", "worst group na.", "this game sucks", "you ar the worst players ive ever scene" };
            DamageNumberManager.DisplayMessage(messages, transform);  

            yield break;
        }

        if (Random.value > 0.5f)
        {
            string[] messages = new[] { "wtf noobs", "res plz", "how i die?", "healer you noob", "wtf learn to tank", "wheres the heals", "filthy casuals", "bilzard plz nerf","At least I have chicken." };
            DamageNumberManager.DisplayMessage(messages, transform);            
        }
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
        UpdateHealthBar();
    }

    protected void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _biped2D = GetComponent<Biped2D>();
        _steerForBoss = GetComponent<SteerForBoss>();
        Collider = GetComponent<Collider2D>();
        _steerForBoss.enabled = false;

        Members.Add(this);

        healthSprite = DamageNumberManager.GetGuildHealthBar();
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
        {
            yield return new WaitForSeconds(2);
            var guildLeader = Members[1];
            DamageNumberManager.DisplayMessage("Okay, so here's the plan", guildLeader.transform);
            yield return new WaitForSeconds(3);
            DamageNumberManager.DisplayMessage("I'll run in and tank him first", guildLeader.transform);
            yield return new WaitForSeconds(1);

            DamageNumberManager.DisplayMessage("LEEEROY JEEENKIINS", transform);
        }
        else
            yield return new WaitForSeconds(8 + (float)_type * 0.25f);
        _steerForBoss.meleeRange = Config.MeleeRange;
        _steerForBoss.enabled = true;

        if (Config.HealAmount > 0)
            StartCoroutine(HealRoutine());
        if (Config.DamagePerTick > 0)
            StartCoroutine(DamageRoutine());

        if (LeeroyJenkins == this)
        {
            var guildLeader = Members[1];
            var follower = Members[12];
            var follower2 = Members[23];
            yield return new WaitForSeconds(1.5f);
            DamageNumberManager.DisplayMessage("Oh jeez", guildLeader.transform);
            yield return new WaitForSeconds(1f);
            DamageNumberManager.DisplayMessage("Oh jeez, let's go, let's go!", follower.transform);
            yield return new WaitForSeconds(.5f);
            DamageNumberManager.DisplayMessage("STICK TO THE PLAN!", guildLeader.transform);
            yield return new WaitForSeconds(1f);
            DamageNumberManager.DisplayMessage("Stick to the plan!", follower2.transform);
            yield return new WaitForSeconds(4f);
            DamageNumberManager.DisplayMessage("God damn it Leeroy.", guildLeader.transform);
            yield return new WaitForSeconds(4f);
            DamageNumberManager.DisplayMessage("At least I have chicken.", transform);
            
        }
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

    protected void Update()
    {
        if (healthSprite.gameObject.activeSelf)
        {
            // set health position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            // need to remove half the width and half the height since our NGUI 0, 0 is in the middle of the screen
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            screenPos.x -= (screenWidth / 2.0f);
            screenPos.y -= (screenHeight / 2.0f);

            screenPos.x *= (1920f / (float)Screen.width);
            screenPos.y *= (1080f / (float)Screen.height);

            healthSprite.transform.localPosition = screenPos + new Vector3(-healthSprite.width/2f, 40);
        }
    }

    private void UpdateHealthBar()
    {
        healthSprite.gameObject.SetActive(true);
        healthSprite.width = Health;

        if(Health >= Config.MaxHealth)
            StartCoroutine(HideHealthBar());
    }

    private IEnumerator HideHealthBar()
    {
        yield return new WaitForSeconds(1);
        if (Health >= Config.MaxHealth)
            healthSprite.gameObject.SetActive(false);
    }
}
