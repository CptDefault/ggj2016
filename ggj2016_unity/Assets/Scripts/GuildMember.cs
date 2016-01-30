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
        get { return Configs[(int) _type]; }
    }

    public static Dictionary<GameObject, int> CachedGroups = new Dictionary<GameObject, int>();
    [SerializeField] private GuildMemberType _type;
    private SpriteRenderer _spriteRenderer;
    private SteerForBoss _steerForBoss;
    public int Grouping;

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
        }
    }

    protected void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _steerForBoss = GetComponent<SteerForBoss>();
        _steerForBoss.enabled = false;
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
        yield return new WaitForSeconds(8 + (float)_type * 0.25f);
        _steerForBoss.meleeRange = Configs[(int) _type].MeleeRange;
        _steerForBoss.enabled = true;
    }
}
