using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildMember : MonoBehaviour
{
    [System.Serializable]
    public class GuildMemberConfig
    {
        public GuildMemberType Type;

        public Color Color;

        public bool MeleeRange;
        public bool IgnoreLightAoe;

        public int MaxHealth = 100;

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
        Health -= damage;
        DamageNumberManager.DisplayDamageNumber(-damage, transform.position);
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        Health += amount;

        UpdateHealthBar();
    }

    protected void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _steerForBoss = GetComponent<SteerForBoss>();
        Collider = GetComponent<Collider2D>();
        _steerForBoss.enabled = false;

        Members.Add(this);

        healthSprite = DamageNumberManager.GetGuildHealthBar();
    }

    protected void OnDestroy()
    {
        CachedGroups.Remove(gameObject);
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
    }

    public void PickUpGroup()
    {
        Config = Config.PickUpGroupVariant();
        Health = Config.MaxHealth;
    }

    protected void Update()
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

    private void UpdateHealthBar()
    {
        healthSprite.width = Health;
    }
}
