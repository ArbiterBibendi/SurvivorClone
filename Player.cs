using Godot;
using System;

public class Player : Character
{
    public static Player Instance;

    private AnimatedSprite _animatedSprite = null;
    private Vector2 _velocity = Vector2.Zero;
    private float _speed = 175f;
    private float ACCELERATION_RATE = 0.5f;
    private string _facingDirection = "up";

    public Player()
    {
        Instance = this;
    }
    public override void _Ready()
    {
        base._Ready();
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        HandleMovement(delta);
        FaceMovement();
        HandleSprite();
    }
    public override void Damage(float value)
    {
        base.Damage(value);
    }
    protected override void Die()
    {
        base.Die();
        GD.Print("Dead");
    }
    private void HandleMovement(float delta)
    {
        Vector2 input = Input.GetVector("left", "right", "up", "down");
        Vector2 targetVelocity = input * _speed;
        _velocity = _velocity.LinearInterpolate(targetVelocity, ACCELERATION_RATE);
        Move(_velocity, delta);
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
