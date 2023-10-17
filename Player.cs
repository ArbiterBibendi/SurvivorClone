using Godot;
using System;

public class Player : KinematicBody2D
{
    public static Player Instance;
    public event EventHandler PlayerDamaged;
    public float Health
    {
        get => _health;
    }

    private float _health = 100f;
    private float MAX_HEALTH = 100f;
    private float MIN_HEALTH = 0f;
    private bool _canBeDamaged = true;

    private float _speed = 175f;
    private Vector2 _velocity = Vector2.Zero;
    private float ACCELERATION_RATE = 0.5f;

    private string _facingDirection = "up";

    private AnimatedSprite _animatedSprite = null;

    private AudioStreamPlayer _audioStreamPlayer = null;
    [Export]
    public AudioStreamMP3 DeathSound;
    [Export]
    public AudioStreamMP3 HitSound;
    [Export]
    public AudioStreamMP3 AttackSound;

    public Player()
    {
        Instance = this;
    }
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }
    public override void _PhysicsProcess(float delta)
    {
        HandleMovement(delta);
        FaceMovement();
        HandleSprite();
    }
    public void Damage(float value)
    {
        if (!_canBeDamaged)
        {
            return;
        }
        _health = Mathf.Clamp(_health - value, MIN_HEALTH, MAX_HEALTH);
        PlayerDamaged.Invoke(this, EventArgs.Empty);
        _audioStreamPlayer.Stream = HitSound;
        _audioStreamPlayer.Playing = true;
        if (_health == MIN_HEALTH)
        {
            Die();
        }
    }
    private void Die()
    {
        GD.Print("Dead");
        _canBeDamaged = false;
        _audioStreamPlayer.Stream = DeathSound;
        _audioStreamPlayer.Playing = true;
    }

    private void HandleMovement(float delta)
    {
        Vector2 input = Input.GetVector("left", "right", "up", "down");

        Vector2 targetVelocity = input * _speed;
        _velocity = _velocity.LinearInterpolate(targetVelocity, ACCELERATION_RATE);
        MoveAndSlide(_velocity);
    }
    private void FaceMovement()
    {
        if (Input.IsActionJustPressed("up"))
        {
            _facingDirection = "up";
        }
        else if (Input.IsActionJustPressed("down"))
        {
            _facingDirection = "down";
        }
        else if (Input.IsActionJustPressed("left"))
        {
            _facingDirection = "left";
        }
        else if (Input.IsActionJustPressed("right"))
        {
            _facingDirection = "right";
        }
    }
    private void HandleSprite()
    {
        _animatedSprite.Playing = !(_velocity.DistanceTo(Vector2.Zero) < 1f);
        if (_facingDirection == "left")
        {
            _animatedSprite.FlipH = true;
        }
        else if (_facingDirection == "right")
        {
            _animatedSprite.FlipH = false;
        }
    }
}
