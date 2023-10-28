using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Spawner
{
    public int Cooldown = 1000;
    public List<Node> Enemies = null;
    private int _distance = 500;
    private List<PackedScene> _enemiesToSpawn = null;
    private Level _level = null;
    private bool _enabled = false;
    public Spawner(Level level, List<PackedScene> enemiesToSpawn)
    {
        _enemiesToSpawn = enemiesToSpawn;
        _level = level;
        Enemies = new List<Node>();
    }
    public async void Enable()
    {
        if (_enabled)
            return;

        _enabled = true;
        while(_enabled)
        {
            SpawnResourceOffScreen();
            await Task.Delay(Cooldown);
        }
    }
    public void Disable()
    {
        if (!_enabled)
            return;

        _enabled = false;
    }
    private void SpawnResourceOffScreen()
    {
        if (_enemiesToSpawn.Count <= 0)
        {
            return;
        }
        Character enemy = _enemiesToSpawn[0].Instance<Character>();
        Enemies.Add(enemy);
        enemy.Died += OnEnemyDied;
        float randomAngle = GD.Randf() * Mathf.Tau;
        Vector2 position = (new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * _distance) + Player.Instance.GlobalPosition;
        enemy.GlobalPosition = position;
        _level.CallDeferred("add_child", enemy);
    }
    public void OnEnemyDied(object enemy, EventArgs e)
    {
        if (!(enemy is Character character))
        {
            return;
        }
        character.Died -= OnEnemyDied;
        Enemies.Remove(character);
    }
}