using Godot;
using System;
using System.Threading.Tasks;
public class Daggers : Node2D
{
    private bool _enabled = false;
    private int _cooldown = 2000;
    private float _damage = 25f;
    private int _distance = 100; 
    private Area2D _area2D;
    private AnimationPlayer _animationPlayer;


    public override async void _Ready()
    {
        _area2D = GetNode<Area2D>("Area2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Player player = GetParentOrNull<Player>();
        if (player != Player.Instance)
        {
            GD.Print(this.Name, " must be a child of player");
        }

        base._Ready();
        Disable();
        _area2D.Connect("area_entered", this, nameof(OnAreaEntered));
        while (true)
        {
            await Task.Delay(_cooldown);
            Enable();
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
    private async void Enable()
    {
        Visible = true;
        _enabled = true;
        // dagger specific
        LookAt(GlobalTransform.origin + Player.Instance.FacingDirection);
        _animationPlayer.Play("Attack");
        await Task.Delay((int)(_animationPlayer.CurrentAnimationLength * 1000));
        Disable();
    }
    private void Disable()
    {
        Visible = false;
        _enabled = false;
    }
    private void OnAreaEntered(Area2D area)
    {
        if (
            ! (area is Character character)
            || character == Player.Instance
            || _enabled == false
        )
        {
            return;
        }
        character.Damage(_damage);
    }
}
