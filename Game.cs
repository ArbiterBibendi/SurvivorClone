using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Game : Node2D
{
    public Level Level = null;
    private Spawner _spawner;

    public override void _Ready()
    {
        Player.Instance.Died += OnPlayerDied;
        Level = GetNode<Level>("Level");

        List<PackedScene> enemiesList = new List<PackedScene>() { ResourceLoader.Load<PackedScene>("SlimeEnemy.tscn") };
        _spawner = new Spawner(Level, enemiesList);
        _spawner.Enable();
    }
    private async void Restart()
    {
        End();
        await Task.Delay(5000);
        Start();
    }
    private void End()
    {
        _spawner.Disable();
        foreach (Node enemy in _spawner.Enemies)
        {
            if (IsInstanceValid(enemy))
            {
                enemy.QueueFree();
            }
        }
    }
    private void Start()
    {
        _spawner.Enable();
        Player.Instance._Ready();
    }
    public void OnPlayerDied(object sender, EventArgs args)
    {
        Restart();
    }
}
