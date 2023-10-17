using Godot;
using System;

public class Character : KinematicBody2D
{
    [Export]
    public AudioStreamMP3 DeathSound;
    [Export]
    public AudioStreamMP3 HitSound;
    [Export]
    public AudioStreamMP3 AttackSound;

    public event EventHandler Died;
    public event EventHandler Damaged;
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
    }
    protected virtual void Die()
    {
        Died?.Invoke(this, EventArgs.Empty);
        CanTakeDamage = false;
        PlayAudioStream(DeathSound);
    }
    public virtual void Damage(float value)
    {
        PlayAudioStream(HitSound);
        if (!CanTakeDamage)
        {
            return;
        }
        Damaged?.Invoke(this, EventArgs.Empty);
        _health = Mathf.Clamp(_health - value, MIN_HEALTH, MAX_HEALTH);
        if (_health <= MIN_HEALTH)
        {
            Die();
        }
    }

    private void PlayAudioStream(AudioStream stream)
    {
        GD.Print(stream);
        if (_audioStreamPlayer != null && stream != null)
        {
            _audioStreamPlayer.Stream = stream;
            _audioStreamPlayer.Playing = true;
        }
    }
}