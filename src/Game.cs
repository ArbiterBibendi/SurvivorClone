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
        if (_spawner == null)
        {
            List<PackedScene> enemiesList = new List<PackedScene>() 
            {
                ResourceLoader.Load<PackedScene>("scenes/characters/Slime.tscn"),
                ResourceLoader.Load<PackedScene>("scenes/characters/Mage.tscn"),
                ResourceLoader.Load<PackedScene>("scenes/characters/Golem.tscn"),
            };
            _spawner = new Spawner(Level, enemiesList);
        }
        _spawner.Enable();
    }
    private void Restart()
    {
        End();
        Start();
    }
    private void End()
    {
        _spawner.Disable();
        _spawner.DequeueAll();
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
