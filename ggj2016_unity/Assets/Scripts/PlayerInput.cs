using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
    
    private CharacterController _characterController;
    private InputDevice _inputDevice;
	public SpriteRenderer DamageSprite;

    // Heading
    private Quaternion _heading;

    // Health + Damage
    public bool Alive { get; private set; }
    public UISprite healthBarSprite;
    public AudioClipContainer HurtSound;
    public AudioClipContainer DieSound;

    private int _maxHealth = 1000000;
    private int _health;

    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthBarSprite.fillAmount = _health/(float) _maxHealth;

            if (_health <= 0)
            {
                Die();

            }
        }
    }

    public TimelineAction CurrentTimelineAction;
    private BossAttacks _bossAttacks;

    private float[] _abilityCooldowns = new float[4];

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _bossAttacks = GetComponent<BossAttacks>();

        Instance = this;

        Alive = true;

        _health = _maxHealth;
    }

    protected void Start()
    {
        if(_inputDevice == null)
            _inputDevice = InputManager.Devices[0]; 
    }

    private void ExecuteAttackCooldown(int attackIndex)
    {
        _abilityCooldowns[attackIndex] = Time.time + 2f*TimelineController.Instance.ActionSpawnInterval;
        CooldownUI.Instance.abilityCooldownLabels[attackIndex].alpha = 1;
    }

    private bool CanUseAbility(int attackIndex)
    {
        // fixed actions
        if (CurrentTimelineAction != null && CurrentTimelineAction.IsFixed)
        {
            if (CurrentTimelineAction.fixedAction == attackIndex)
            {
                _abilityCooldowns[attackIndex] = -1;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        var result = _abilityCooldowns[attackIndex] <= 0 || _abilityCooldowns[attackIndex] - 0.2f < Time.time;

        if(result)
            ExecuteAttackCooldown(attackIndex);

        return result;
    }

    protected void Update()
    {

        if(!Alive) {
            _characterController.SetDesiredSpeed(Vector2.zero);
            return;
        }


        var vector2 = _inputDevice.LeftStick.Vector + _inputDevice.DPad.Vector;

        _characterController.SetDesiredSpeed(Vector2.ClampMagnitude(vector2, 1));

        if (CurrentTimelineAction != null)
        {
            if (_inputDevice.GetControl(InputControlType.Action1).WasPressed && CanUseAbility(0) && CurrentTimelineAction.IsValid())
               _bossAttacks.Attack(BossAttacks.Attacks.Dash);
            if (_inputDevice.GetControl(InputControlType.Action2).WasPressed && CanUseAbility(1) && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Throw);
            if (_inputDevice.GetControl(InputControlType.Action3).WasPressed && CanUseAbility(2) && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Whirlwind);
            if (_inputDevice.GetControl(InputControlType.Action4).WasPressed && CanUseAbility(3) && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Smash);
        }
        /*else
        {
            if (_inputDevice.GetControl(InputControlType.Action1).WasPressed && CanUseAbility(0))
            {
                _bossAttacks.Attack(BossAttacks.Attacks.Dash);
            }
            if (_inputDevice.GetControl(InputControlType.Action2).WasPressed && CanUseAbility(1))
                _bossAttacks.Attack(BossAttacks.Attacks.Throw);
            if (_inputDevice.GetControl(InputControlType.Action3).WasPressed && CanUseAbility(2))
                _bossAttacks.Attack(BossAttacks.Attacks.Whirlwind);
            if (_inputDevice.GetControl(InputControlType.Action4).WasPressed && CanUseAbility(3))
                _bossAttacks.Attack(BossAttacks.Attacks.Smash);
        }*/
        

        // update cooldowns
        for (int i = 0; i < _abilityCooldowns.Length; i++)
        {
            if (_abilityCooldowns[i] <= 0)
                continue;

            if (Time.time >= _abilityCooldowns[i])
            {
                _abilityCooldowns[i] = -1;
                CooldownUI.Instance.abilityCooldownLabels[i].alpha = 0;
            }
            else
            {
                CooldownUI.Instance.abilityCooldownLabels[i].text = ((int)(_abilityCooldowns[i] - Time.time) + 1).ToString();
            }
        }
    }

    public void SetController(InputDevice inputDevice)
    {
        _inputDevice = inputDevice;
    }

    private void Die() {

        if(Alive)
        {

        }
    }

    public void Respawn() {
        Alive = true;
        if (DamageSprite != null) DamageSprite.enabled = true;

        Health = _maxHealth;
        enabled = true;
        _characterController.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        _characterController.Renderer.transform.rotation = Quaternion.identity;
        GetComponent<Collider2D>().enabled = true;
        StartCoroutine(FlashIn());
    }

    private IEnumerator FlashIn()
    {
        for (int i = 0; i < 10; i++)
        {
            _characterController.Renderer.enabled = i % 2 != 0;
            yield return new WaitForSeconds(2f/(16 - i));
        }
    }


    public void DealDamage(int damagePerSecond)
    {
        Health -= damagePerSecond;

        if (Health < 0)
        {
            // die
            if (enabled)
            {
                StartCoroutine(EndGameMessages());
            }


            Health = 0;
            enabled = false;
            _characterController.enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            _characterController.Renderer.transform.rotation = Quaternion.Euler(0, 0, 90);

            return;
        }
        else
        {
//            DamageNumberManager.DisplayDamageNumber(-damagePerSecond, transform.position, boss: true);            
        }
    }

    private IEnumerator EndGameMessages()
    {
        foreach (var guildMember in GuildMember.Members)
        {
            guildMember.EndGameMessage();

            yield return new WaitForSeconds(Random.Range(0.1f, 1));
        }
    }
}
