using Godot;
using System;
using System.Threading.Tasks;

/* Any child of this should be an Area2D and will be considered a candle */

public class Candles : Ability
{
    [Export]
    public PackedScene CandleScene = null;
    private int _numberOfCandles = 4;
    private float _distance = 100;
    private float _rotationSpeed = 0.06f;
    public Candles()
    {
        Damage = 13f;
        InitialWaitTime = 300;
        Directional = false;
        Cooldown = 5000;
        AttackTime = 3000;
    }
    public void SetCandles(int numberOfCandles)
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
        _numberOfCandles = numberOfCandles;

        double angleOffset = Mathf.Tau / numberOfCandles;
        for (int i = 1; i <= numberOfCandles; i++)
        {
            Area2D candle = CandleScene.Instance<Area2D>();
            candle.GlobalPosition = new Vector2(
                (float)Math.Cos(angleOffset * i) * _distance,
                (float)Math.Sin(angleOffset * i) * _distance
            );
            CallDeferred("add_child", candle);
        }
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        SetCandles(_numberOfCandles);
    }
    public override void _Ready()
    {
        if (CandleScene == null)
        {
            GD.PrintErr("Candles has no scene specified as candle");
        }
        base._Ready();
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Rotate(_rotationSpeed);
        foreach (Area2D area in Areas)
        {
            area.GlobalRotation = 0f; // keep candle upright when rotating parent
        }
    }
}
