using Godot;
using System;
using System.Threading.Tasks;

public class SlimeEnemy : Character
{
    private float _damage = 25f;
    private bool _canDamagePlayer = true;

    private readonly float INITIAL_HEALTH = 25f;

    private Vector2 _velocity = Vector2.Zero;
    private readonly float _jumpForce = 500f;
    private readonly int _jumpInterval = 3000; // milliseconds
    private bool _canJump = true;

    private Player _player = null;
    private ulong _playerID = 0;

    public override void _Ready()
    {
        base._Ready();
        _health = INITIAL_HEALTH;
        _player = Player.Instance;
        if (_player == null)
        {
            throw new Exception("Player not loaded");
        }
        _playerID = _player.GetInstanceId();
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        HandleMovement(delta);
    }
    protected override void Die()
    {
        Utils.QueueFree(this);
    }
    
    protected override void OnAreaEntered(Area2D area)
    {
        if (area == Player.Instance && _canDamagePlayer)
        {
            DamagePlayer();
        }
    }
    private async void DamagePlayer()
    {
        _canDamagePlayer = false;
        _player.Damage(_damage);
        await Task.Delay(1000);
        _canDamagePlayer = true;
    }
    private void HandleMovement(float delta)
    {
        if (_canJump)
        {
            Jump();
        }
        _velocity = _velocity.LinearInterpolate(Vector2.Zero, 0.05f);
        Move(_velocity, delta);
    }
    private async void Jump()
    {
        _canJump = false;
        Vector2 targetDirection = new Vector2(_player.GlobalTransform.origin - GlobalTransform.origin);
        Vector2 targetVelocity = targetDirection.Normalized() * _jumpForce;
        _velocity = targetVelocity;
        await Task.Delay(_jumpInterval);
        _canJump = true;
    }
}
