using Godot;
using System;

public class Enemy : Character
{
    [Export]
    public float DamageValue = 25f;
    [Export]
    public float Speed = 100f;
    protected Vector2 Velocity = Vector2.Zero;
    protected bool Dead = false;
    protected Player Player = null;

    private bool _canDamagePlayer = true;

    public Enemy()
    {
        MaxHealth = 25f;
    }
    public override void _Ready()
    {
        base._Ready();
        Player = Player.Instance;
        if (Player == null)
        {
            throw new Exception("Player not loaded");
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        HandleMovement(delta);
    }

    protected virtual void HandleMovement(float delta)
    {
        Velocity = (Player.GlobalPosition - GlobalPosition).Normalized() * Speed;
        Move(Velocity, delta);
    }

    protected override void OnAreaEntered(Area2D area)
    {
        if (area == Player.Instance && _canDamagePlayer && !Dead)
        {
            DamagePlayer();
        }
    }
    private async void DamagePlayer()
    {
        _canDamagePlayer = false;
        Player.Damage(DamageValue);
        await Utils.Delay(1000);
        _canDamagePlayer = true;
    }
}