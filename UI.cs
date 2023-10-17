using Godot;
using System;

public class UI : Control
{
    private Label _label = null;
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _label.Text = Player.Instance.Health.ToString();
        Player.Instance.PlayerDamaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(object sender, EventArgs e)
    {
        int health = (int)Player.Instance.Health;
        _label.Text = health.ToString();
        if (health == 0)
        {
            _label.AddColorOverride("font_color", Color.ColorN("red"));
        }
    }
}
