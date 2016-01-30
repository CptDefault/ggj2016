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
            if (_inputDevice.GetControl(InputControlType.Action1).WasPressed)
            {
                // check timeline action is in valid position
                if (CurrentTimelineAction.IsValid())
                {
                    _bossAttacks.Attack(BossAttacks.Attacks.Dash);
                    // do attack 1
                }
            }
            if (_inputDevice.GetControl(InputControlType.Action2).WasPressed && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Throw);
            if (_inputDevice.GetControl(InputControlType.Action3).WasPressed && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Whirlwind);
            if (_inputDevice.GetControl(InputControlType.Action4).WasPressed && CurrentTimelineAction.IsValid())
                _bossAttacks.Attack(BossAttacks.Attacks.Smash);
        }
        else
        {
            if (_inputDevice.GetControl(InputControlType.Action2).WasPressed)
                _bossAttacks.Attack(BossAttacks.Attacks.Throw);
            if (_inputDevice.GetControl(InputControlType.Action3).WasPressed)
                _bossAttacks.Attack(BossAttacks.Attacks.Whirlwind);
            if (_inputDevice.GetControl(InputControlType.Action4).WasPressed)
                _bossAttacks.Attack(BossAttacks.Attacks.Smash);
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
        DamageSprite.enabled = true;
    }


}
