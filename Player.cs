using Godot;
using System;

public class Player : Character
{
    public static Player Instance;
    public Vector2 FacingDirection {get; private set;} = Vector2.Up;
    public EventHandler Ready = null;

    private AnimatedSprite _animatedSprite = null;
    private Camera2D _camera = null;
    private Vector2 _velocity = Vector2.Zero;
    private float _speed = 175f;
    private float ACCELERATION_RATE = 0.5f;
    private string _facingDirectionString = "up";

    public Player()
    {
        Instance = this;
    }
    public override void _Ready()
    {
        base._Ready();
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _camera = GetNode<Camera2D>("Camera2D");
        _camera.Zoom = Vector2.One;
        _health = MAX_HEALTH;
        Position = Vector2.Zero;
        //AddAbility("Daggers");
        AddAbility("Candles");
        CanTakeDamage = true;

        Ready?.Invoke(this, EventArgs.Empty);
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        HandleMovement(delta);
        FaceMovement();
        HandleSprite();
        if (Input.IsKeyPressed((int)KeyList.O))
        {
            Die();
        }
    }
    public override void Damage(float value)
    {
        base.Damage(value);
    }
    protected override void Die()
    {
        base.Die();
        RemoveAbilities();
        GD.Print("Dead");
    }
    private void AddAbility(string name)
    {
        Ability ability = Utils.Load<Ability>($"abilities/{name}.tscn");
        if (ability != null)
        {
            AddChild(ability);
        }
    }
    private void RemoveAbilities()
    {
        DisableAbilities();
        foreach (Node ability in Utils.GetChildrenOfType<Ability>(this))
        {
            Utils.QueueFree(ability);
            GD.Print($"Removing {ability.Name}");
        }
    }
    private void DisableAbilities()
    {
        foreach (Node node in GetChildren())
        {
            if (node is Ability ability)
            {
                ability.Disable();
            }
        }
    }
    private void EnableAbilities()
    {
        foreach (Node node in GetChildren())
        {
            if (node is Ability ability)
            {
                ability.Enable();
            }
        }
    }
    protected override void Despawn()
    {
        
    }

    private void HandleMovement(float delta)
    {
        Vector2 input = Input.GetVector("left", "right", "up", "down");
        if (!input.Equals(Vector2.Zero))
        {
            FacingDirection = input.Normalized();
        }
        Vector2 targetVelocity = input * _speed;
        _velocity = _velocity.LinearInterpolate(targetVelocity, ACCELERATION_RATE);
        Move(_velocity, delta);
    }
    private void FaceMovement()
    {
        if (!MovementEnabled)
            return;
        if (Input.IsActionJustPressed("up"))
        {
            _facingDirectionString = "up";
        }
        else if (Input.IsActionJustPressed("down"))
        {
            _facingDirectionString = "down";
        }
        else if (Input.IsActionJustPressed("left"))
        {
            _facingDirectionString = "left";
        }
        else if (Input.IsActionJustPressed("right"))
        {
            _facingDirectionString = "right";
        }
    }
    private void HandleSprite()
    {
        bool isStopped = _velocity.DistanceTo(Vector2.Zero) < 1f;
        _animatedSprite.Playing = !isStopped && MovementEnabled;
        if (_facingDirectionString == "left")
        {
            _animatedSprite.FlipH = true;
        }
        else if (_facingDirectionString == "right")
        {
            _animatedSprite.FlipH = false;
        }
    }
}
