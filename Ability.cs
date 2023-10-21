using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Ability : Node2D
{
    protected int InitialWaitTime = 0;
    protected float Damage = 25f;
    protected List<Area2D> Areas;
    protected bool Directional = true;
    protected int AttackTime = 0;
    protected int Cooldown = 2000;

    private AnimationPlayer _animationPlayer;
    private bool _enabled = false;

    public async void Enable()
    {
        _enabled = true;
        await Task.Delay(InitialWaitTime);
        while (_enabled)
        {
            await Task.Delay(Cooldown);
            StartAttack();
        }
    }
    public void Disable()
    {
        _enabled = false;
    }
    public override void _Ready()
    {
        base._Ready();

        Player player = GetParentOrNull<Player>();
        if (player != Player.Instance)
        {
            GD.Print(this.Name, " must be a child of player");
        }

        _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (_animationPlayer != null)
        {
            if (AttackTime != 0)
            {
                GD.PushWarning("Tried to set AttackTime on an ability controlled by an AnimationPlayer");
            }
            AttackTime = (int)(_animationPlayer.GetAnimation("Attack").Length * 1000);
        }

        Areas = new List<Area2D>();
        foreach (var item in GetChildren())
        {
            if (item is Area2D area)
            {
                Areas.Add(area);
                area.Connect("area_entered", this, nameof(OnAreaEntered));
            }
        }
        
        StopAttack();
        Enable();
    }
    protected async virtual void StartAttack()
    {
        if (Directional)
        {
            LookAt(GlobalTransform.origin + Player.Instance.FacingDirection);   
        }

        Visible = true;
        _enabled = true;
        _animationPlayer?.Play("Attack");
        await Task.Delay(AttackTime);
        StopAttack();
    }
    protected virtual void StopAttack()
    {
        Visible = false;
        _enabled = false;
    }
    private void OnAreaEntered(Area2D area)
    {
        if (
            !(area is Character character)
            || character == Player.Instance
            || _enabled == false
        )
        {
            return;
        }
        character.Damage(Damage);
    }
}