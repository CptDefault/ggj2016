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
        public Sprite[] Sprites;

        public GameObject AttackParticle;

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
    public static List<GuildMember> DeadMembers = new List<GuildMember>();

    public Collider2D Collider { get; private set; }


    [FMODUnity.EventRef]
    public string bossBeenHitSound = "event:/Boss/Boss_BeenHit";

    [SerializeField]
    private GuildMemberType _type;

    public SpriteRenderer SpriteRenderer;
    private SteerForBoss _steerForBoss;
    public int Grouping;
    public int Health = 100;
    private GuildMemberConfig _overrideConfig;
    public static GuildMember LeeroyJenkins;
    private Biped2D _biped2D;
    private float _lastFullHealth;
    public string Name;

    public Transform HealParticles;
    private float _jumpHeight;
    private float _jumpMult = 1;

    private static string[] RandomNames =
    {
        "CoolWizard69", "MLG-Lord", "Yeezus928", "xXx_JeffRocks_xXx", "MysteryMeat9821", "FererroRochel_95", "MarsBarWarrior", "AlphabetJack", "DeezNuts", "MicrosoftSam", "DolanTremp54", 
        "Scratch", "Doopy", "Poopy", "Sneezy-2873", "UnwisestDaisy", "MothQueen4eva", "OozyPlace", "TitMonster5000"
    };

    public GuildMemberType MemberType
    {
        get {return _type;}
        set
        {
            _type = value;
            if (SpriteRenderer != null)
                SpriteRenderer.sprite = Config.Sprites[Random.Range(0, Config.Sprites.Length)];
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
        SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
        _biped2D.Rigidbody.isKinematic = true;
        _biped2D.Collider.enabled = false;
        CachedGroups.Remove(gameObject);
        Members.Remove(this);
        DeadMembers.Add(this);
        enabled = false;

        StartCoroutine(DeadMessage());

        ChatBox.Instance.AddChatMessage(string.Format("[{0}]{1}[-] has died", ColorToHex(Config.Color), Name));

        healthSprite.gameObject.SetActive(false);
    }

    private IEnumerator DeadMessage()
    {
        yield return new WaitForSeconds(Random.Range(1, 3f));

        if (Members.Count == 0)
        {
            string[] messages = new[] { "you guys suck.", "wow. that went great.", "worst group na.", "this game sucks", "you ar the worst players ive ever scene", "There's no mistakes, just happy accidents." };
            DisplayChatMessage(messages, this);  

            yield break;
        }

        if (Random.value > 0.5f)
        {
            string[] messages = new[] { "wtf noobs", "res plz", "how i die?", "healer you noob", "wtf learn to tank", "wheres the heals", "filthy casuals", "bilzard plz nerf","At least I have chicken.", "tank uninstall" };
            DisplayChatMessage(messages, this);            
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
        if(SpriteRenderer == null) SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _biped2D = GetComponent<Biped2D>();
        _steerForBoss = GetComponent<SteerForBoss>();
        Collider = GetComponent<Collider2D>();
        _steerForBoss.enabled = false;

        Members.Add(this);

        healthSprite = DamageNumberManager.GetGuildHealthBar();

        _jumpHeight = Random.Range(0.1f, 0.15f);

    }

    protected void OnDestroy()
    {
        CachedGroups.Remove(gameObject);
        Members.Remove(this);
    }

    protected void Start()
    {
        Name = RandomNames[Random.Range(0, RandomNames.Length - 1)];
        
        StartCoroutine(IntroRoutine());
    }

    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public void EndGameMessage()
    {
        if(Health > 0)
            DisplayChatMessage("GG", this);
    }

    private void DisplayChatMessage(string message, GuildMember member)
    {
        DamageNumberManager.DisplayMessage(message, member.gameObject.transform);

        ChatBox.Instance.AddChatMessage(string.Format("[{0}]{1}[-]: {2}", ColorToHex(member.Config.Color), member.Name, message));
    }

    private void DisplayChatMessage(string[] messages, GuildMember member)
    {
        var message = messages[Random.Range(0, messages.Length)];

        DamageNumberManager.DisplayMessage(message, member.gameObject.transform);

        ChatBox.Instance.AddChatMessage(string.Format("[{0}]{1}[-]: {2}", ColorToHex(member.Config.Color), member.Name, message));
    }

    private IEnumerator IntroRoutine()
    {
        FunSystem.RaidStartTime = Time.time;
        
        if (LeeroyJenkins != null && LeeroyJenkins != this)
            yield return new WaitForSeconds(3);
        if (LeeroyJenkins == this)
        {
            Name = "Leeroy Jenkins";

            yield return new WaitForSeconds(2);
            var guildLeader = Members[1];
            DisplayChatMessage("Okay, so here's the plan", guildLeader);
            yield return new WaitForSeconds(3);
            DisplayChatMessage("I'll run in and tank him first", guildLeader);
            yield return new WaitForSeconds(1);

            DisplayChatMessage("LEEEROY JEEENKIINS", this);
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
            DisplayChatMessage("Oh jeez", guildLeader);
            yield return new WaitForSeconds(1f);
            DisplayChatMessage("Oh jeez, let's go, let's go!", follower);
            yield return new WaitForSeconds(.5f);
            DisplayChatMessage("STICK TO THE PLAN!", guildLeader);
            yield return new WaitForSeconds(1f);
            DisplayChatMessage("Stick to the plan!", follower2);
            yield return new WaitForSeconds(4f);
            DisplayChatMessage("God damn it Leeroy.", guildLeader);
            yield return new WaitForSeconds(4f);
            DisplayChatMessage("At least I have chicken.", this);
            
        }
    }

    private IEnumerator HealRoutine()
    {
        while (true)
        {
            var seconds = TimelineController.OffBeatBy() + OneBeat * 2;
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
            var seconds = OneBeat * 2;
            yield return new WaitForSeconds(seconds + Random.value);

            if (!enabled || PlayerInput.Instance.Health <= 0)
                yield break;

            PlayerInput.Instance.DealDamage(Config.DamagePerTick);
            if (Config.AttackParticle != null)
            {
                Instantiate(Config.AttackParticle, transform.position + Vector3.up*0.2f, Quaternion.identity);
            }
            if(Config.MeleeRange)
                FMODUnity.RuntimeManager.PlayOneShot(bossBeenHitSound, Camera.main.transform.position);
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

        _jumpMult = Mathf.Lerp(_jumpMult, 2 - (Health / Config.MaxHealth), Time.deltaTime * 4);

        var f = TimelineController.TimeInBeats * 2 * Mathf.PI;
        SpriteRenderer.transform.localPosition = new Vector3(0, 0.084f + Mathf.Abs(Mathf.Sin(f)) * _jumpHeight * _jumpMult, 0);
        SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(f) * 10 * _jumpMult);
    }

    private void UpdateHealthBar()
    {
        healthSprite.gameObject.SetActive(true);
        healthSprite.width = Health/2;

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
