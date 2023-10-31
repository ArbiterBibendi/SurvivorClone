using Godot;
using System.Collections.Generic;
using System.Threading;
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
    private bool _ableToDamage = false;
    private bool _enabled = false;
    private Task _task = null;
    private CancellationTokenSource cancellationTokenSource = null;
    public override void _Ready()
    {
        base._Ready();
        Connect("child_entered_tree", this, nameof(OnChildEnteredTree));

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
        SetupAreaChildren();
        StopAttack();
        Enable();
    }

    public async void Enable()
    {
        _enabled = true;
        await Utils.Delay(InitialWaitTime);
        if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
        }
        cancellationTokenSource = new CancellationTokenSource();
        TaskAction();
    }
    public void Disable()
    {
        _enabled = false;
        cancellationTokenSource?.Cancel();
    }
    private async void TaskAction()
    {
        while (_enabled)
        {
            StartAttack();
            await Utils.Delay(AttackTime, cancellationTokenSource.Token);
            StopAttack();
            await Utils.Delay(Cooldown, cancellationTokenSource.Token);
        }
    }

    protected void SetupAreaChildren()
    {
        Areas = new List<Area2D>();
        foreach (var child in GetChildren())
        {
            if (!(child is Area2D area))
            {
                return;
            }

            if (!Areas.Contains(area))
            {
                Areas.Add(area);
            }
            if (!area.IsConnected("area_entered", this, nameof(OnAreaEntered)))
            {
                area.Connect("area_entered", this, nameof(OnAreaEntered));
            }
        }
    }

    protected virtual void StartAttack()
    {
        if (!IsInstanceValid(this) || Player.Instance == null)
        {
            return;
        }
        if (Directional)
        {
            LookAt(GlobalTransform.origin + Player.Instance.FacingDirection);
        }

        SetVisible(true);
        _ableToDamage = true;
        Utils.PlayAnimation(_animationPlayer, "Attack");
    }
    protected virtual void StopAttack()
    {
        SetVisible(false);
        _ableToDamage = false;
    }
    private void OnAreaEntered(Area2D area)
    {
        if (
            !(area is Character character)
            || character == Player.Instance
            || _ableToDamage == false
        )
        {
            return;
        }
        character.Damage(Damage);
    }
    protected void OnChildEnteredTree(Node child)
    {
        SetupAreaChildren();
    }
    private new void SetVisible(bool value)
    {
        if (IsInstanceValid(this)) // Object could be destroyed during wait
                Visible = value;
    }
}