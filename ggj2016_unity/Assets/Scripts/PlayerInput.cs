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
    public AudioClipContainer HurtSound;
    public AudioClipContainer DieSound;

    public int health = 1000000;

    public TimelineAction CurrentTimelineAction;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        Instance = this;

        Alive = true;
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
                    Debug.Log("Valid action!");
                }
                else
                {
                    Debug.Log("Invalid action");
                    CurrentTimelineAction = null;
                }
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
        DamageSprite.enabled = true;
    }


}
