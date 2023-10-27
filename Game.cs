using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    public Level Level = null;
    private Spawner _spawner;

    public override void _Ready()
    {
        Player.Instance.Died += OnPlayerDied;
        Level = GetNode<Level>("Level");

        List<PackedScene> enemiesList = new List<PackedScene>(){ResourceLoader.Load<PackedScene>("SlimeEnemy.tscn")};
        _spawner = new Spawner(Level, enemiesList);
        _spawner.Enable();
    }
    public void OnPlayerDied(object sender, EventArgs args)
    {

    }
}
