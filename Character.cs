using Godot;
using System;

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

    protected float _health = 100f;
    protected float MIN_HEALTH = 0f;
    protected float MAX_HEALTH = 100f;

    private AudioStreamPlayer _audioStreamPlayer = null;

    public override void _Ready()
    {
        base._Ready();
        _audioStreamPlayer = GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
        Connect("area_entered", this, nameof(OnAreaEntered));
    }
    protected virtual void OnAreaEntered(Area2D area)
    {
    }
    protected virtual void Die()
    {
        Died?.Invoke(this, EventArgs.Empty);
        CanTakeDamage = false;
        PlayAudioStream(DeathSound);
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
        if (_health <= MIN_HEALTH)
        {
            Die();
        }
    }

    private void PlayAudioStream(AudioStream stream)
    {
        if (_audioStreamPlayer != null && stream != null)
        {
            _audioStreamPlayer.Stream = stream;
            _audioStreamPlayer.Playing = true;
        }
    }
    protected void Move(Vector2 velocity, float delta)
    {
        Transform2D transform = GlobalTransform;
        transform.origin += velocity * delta;
        GlobalTransform = transform;
    }
}

public class DamagedEventArgs : EventArgs
{
    public float HealthBefore {get;}
    public float HealthAfter {get;}
    public float Damage {get;}
    public DamagedEventArgs(float healthBeforeDamage, float healthAfterDamage, float damageValue)
    {
        HealthBefore = healthBeforeDamage;
        GD.Print(healthAfterDamage);
        HealthAfter = healthAfterDamage;
        Damage = damageValue;
    }
}