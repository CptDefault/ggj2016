using System;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    public enum AnimType
    {
        Idle,
        Move,

        Smash,
        Throw,
        Dash,
        Whirlwind
    }

    [Serializable]
    public class Animation
    {
        public float FrameRate = 12;
        public Vector2 Heading;
        public Sprite[] Frames;

        public int LoopFromFrame = 0;

        public AnimType AnimType;
    }

    public float MoveSpeed = 5;
    public float Acceleration = 5;
    public bool ForceDiagonalMovement;

    public float DashSpeed = 5;
    public float DashAcceleration = 5;

    public Animation[] Anims;

    public SpriteRenderer Renderer;

    public AnimType WeaponAnimation {set { _currentAnim = value;
        _frameNumber = 0;
    }}

    private Rigidbody2D _rigid;
    private Vector2 _desiredSpeed;

    private int _frameNumber;
    private AnimType _currentAnim;
    private Vector2 _heading;
    public Vector2 Heading {get { return _heading; }}
    private float _frameTicker;
    private bool _isDashing;

    protected void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        if (Renderer == null)
            Renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetDesiredSpeed(Vector2 speed, bool isDashing = false)
    {
        if (_isDashing && ! isDashing)
            return;
        _isDashing = isDashing;
        _desiredSpeed = speed * (isDashing ? DashSpeed : MoveSpeed);

        if (ForceDiagonalMovement)
        {
            var x = Mathf.Abs(_desiredSpeed.x);
            var y = Mathf.Abs(_desiredSpeed.y);
            if (x < y / 2)
                _desiredSpeed.x = 0;
            else if (y < x / 2)
                _desiredSpeed.y = 0;
            else
            {
                _desiredSpeed.x *= (y + x) / (x + 1);
                _desiredSpeed.y *= (y + x) / (y + 1);
            }
        }
    }

    protected void FixedUpdate()
    {
        var speed = _rigid.velocity;

        speed += Vector2.ClampMagnitude(_desiredSpeed - speed, (_isDashing ? DashAcceleration : Acceleration)*Time.fixedDeltaTime);

        _rigid.velocity = speed;

        if(_currentAnim == AnimType.Idle || _currentAnim == AnimType.Move)
        if (speed.sqrMagnitude > 0.2f != (_currentAnim == AnimType.Move))
        {
            _frameNumber = 0;
            _frameTicker = 0;

            _currentAnim = speed.sqrMagnitude > 0.2f ? AnimType.Move : AnimType.Idle;
        }

        if (speed.sqrMagnitude > 0.1f)
        {
            _heading = speed.normalized;

            var localScale = Renderer.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (_heading.x < 0 ? -1 : 1);
            Renderer.transform.localScale = localScale;

            /*if (_heading.x < 0)
            {
                _heading.x = -_heading.x;
            }*/
        }

        var maxDot = float.MinValue;
        Animation a = null;
        var normHeading = _heading;
        if (normHeading.x < 0)
            normHeading.x = -_heading.x;

        foreach (var anim in Anims)
        {
            if(anim.AnimType != _currentAnim)
                continue;
            var f = Vector3.Dot(anim.Heading.normalized, normHeading);
            if (f > maxDot)
            {
                maxDot = f;
                a = anim;
            }
        }

        if (a != null)
        {
            _frameTicker += Time.deltaTime;
            if (_frameTicker > 63f/60/a.FrameRate)
            {
                _frameNumber++;
                _frameTicker -= 1 / a.FrameRate;
            }

            if (_frameNumber >= a.Frames.Length)
            {
                _frameNumber = a.LoopFromFrame;
            }

            //_frameNumber %= a.Frames.Length;
            Renderer.sprite = a.Frames[_frameNumber];
        }

        _isDashing = false;
    }

}
