using Godot;
using System;
using System.Threading.Tasks;

public class SlimeEnemy : KinematicBody2D
{
    private float _damage = 25f;
    private bool _canDamagePlayer = true;

    private Vector2 _velocity = Vector2.Zero;
    private float _jumpForce = 500f;
    private int _jumpInterval = 3000; // milliseconds
    private bool _canJump = true;

    private Player _player = null;
    private ulong _playerID = 0;

    public override void _Ready()
    {
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
        HandleMovement();
        KinematicCollision2D collision = GetLastSlideCollision();
        bool collidedWithPlayer = CollidedWithPlayer(collision);
        if (collidedWithPlayer == true && _canDamagePlayer)
        {
            DamagePlayer();
        }
    }
    private bool CollidedWithPlayer(KinematicCollision2D collision)
    {
        return collision?.ColliderId == _playerID;
    }
    private async void DamagePlayer()
    {
        _canDamagePlayer = false;
        _player.Damage(_damage);
        await Task.Delay(1000);
        _canDamagePlayer = true;
    }
    private void HandleMovement()
    {
        if (_canJump)
        {
            Jump();
        }
        _velocity = _velocity.LinearInterpolate(Vector2.Zero, 0.05f);
        MoveAndSlide(_velocity);
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
