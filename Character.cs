using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Character : Area2D
{
    [Export]
    public AudioStreamMP3 DeathSound;
    [Export]
    public AudioStreamMP3 HitSound;
    [Export]
    public AudioStreamMP3 AttackSound;

    public event EventHandler Died;
    public event EventHandler<DamagedEventArgs> Damaged;
    public float Health { get => _health; }
    public bool CanTakeDamage { get; set; } = true;
    public bool MovementEnabled { get; set; } = true;

    protected float _health = 100f;
    protected float MIN_HEALTH = 0f;
    protected float MAX_HEALTH = 100f;

    private readonly int _pushAwaySpeed = 50;
    private AudioStreamPlayer _audioStreamPlayer = null;
    private AnimatedSprite _animatedSprite = null;
    private AnimationPlayer _animationPlayer = null;
    private Particles2D _deathParticles = null;

    public override void _Ready()
    {
        base._Ready();
        _audioStreamPlayer = GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
        if (_audioStreamPlayer == null)
        {
            _audioStreamPlayer = Utils.Load<AudioStreamPlayer>("DefaultAudioStreamPlayer.tscn");
            AddChild(_audioStreamPlayer);
        }
        _animatedSprite = Utils.GetChildOfType<AnimatedSprite>(this);
        _animatedSprite.Playing = true;
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        Utils.PlayAnimation(_animationPlayer, "RESET");
        _deathParticles = GetNodeOrNull<Particles2D>("DeathParticles");
        if (!IsConnected("area_entered", this, nameof(OnAreaEntered)))
        {
            Connect("area_entered", this, nameof(OnAreaEntered));
        }
        MovementEnabled = true;
    }

    protected virtual void OnAreaEntered(Area2D area)
    {
    }
    protected virtual void Die()
    {
        CanTakeDamage = false;
        MovementEnabled = false;
        PlayAudioStream(DeathSound);
        Utils.PlayAnimation(_animationPlayer, "Died", _Die);
    }
    private void _Die()
    {
        Despawn();
        Died?.Invoke(this, EventArgs.Empty);
    }
    public virtual void Damage(float value)
    {
        if (!CanTakeDamage)
        {
            return;
        }
        float healthBeforeTakeDamage = _health;
        _health = Mathf.Clamp(_health - value, MIN_HEALTH, MAX_HEALTH);
        Damaged?.Invoke(this, new DamagedEventArgs(healthBeforeTakeDamage, _health, value));
        PlayAudioStream(HitSound);
        Utils.PlayAnimation(_animationPlayer, "Damaged");
        if (_health <= MIN_HEALTH)
        {
            Die();
        }
    }

    protected virtual void Despawn()
    {
        Utils.QueueFree(this);
    }

    private void PlayAudioStream(AudioStream stream)
    {

        if (_audioStreamPlayer != null && stream != null)
        {
            _audioStreamPlayer.Stream = stream;
            _audioStreamPlayer.Play(0);
        }
    }
    protected void Move(Vector2 velocity, float delta)
    {
        velocity = MovementEnabled ? velocity : Vector2.Zero;
        Godot.Collections.Array areas = GetOverlappingAreas();
        if (areas.Count > 0)
        {
            foreach (Area2D area in areas)
            {
                if (!(area is Character character))
                {
                    continue;
                }
                Vector2 directionAway = (GlobalPosition - character.GlobalPosition).Normalized();
                velocity += directionAway * _pushAwaySpeed;
            }
        }
        GlobalPosition += velocity * delta;
    }
}

public class DamagedEventArgs : EventArgs
{
    public float HealthBefore { get; }
    public float HealthAfter { get; }
    public float Damage { get; }
    public DamagedEventArgs(float healthBeforeDamage, float healthAfterDamage, float damageValue)
    {
        HealthBefore = healthBeforeDamage;
        HealthAfter = healthAfterDamage;
        Damage = damageValue;
    }
}