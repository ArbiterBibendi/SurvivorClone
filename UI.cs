using Godot;
using System;

public class UI : Control
{
    private Label _healthLabel = null;
    public override void _Ready()
    {
        _healthLabel = GetNode<Label>("Health");
        _healthLabel.Text = Player.Instance.Health.ToString();
        Player.Instance.Damaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(object sender, DamagedEventArgs e)
    {
        int health = (int)e.HealthAfter;
        _healthLabel.Text = health.ToString();
        if (health == 0)
        {
            _healthLabel.AddColorOverride("font_color", Color.ColorN("red"));
        }
    }
}
