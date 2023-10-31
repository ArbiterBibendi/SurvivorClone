using Godot;
using System;
using System.Threading.Tasks;

public class Slime : Enemy
{
    private readonly float _jumpForce = 500f;
    private readonly int _jumpInterval = 2500; // milliseconds
    private bool _canJump = true;
    protected override void HandleMovement(float delta)
    {
        if (_canJump && !Dead)
        {
            Jump();
        }
        Velocity = Velocity.LinearInterpolate(Vector2.Zero, 0.1f);
        Move(Velocity, delta);
    }
    private async void Jump()
    {
        _canJump = false;
        Vector2 targetDirection = new Vector2(Player.GlobalTransform.origin - GlobalTransform.origin);
        Vector2 targetVelocity = targetDirection.Normalized() * _jumpForce;
        Velocity = targetVelocity;
        await Utils.Delay(_jumpInterval);
        _canJump = true;
    }
}
