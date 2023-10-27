using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Spawner
{
    public int Cooldown = 1000;
    private int _distance = 500;
    private List<PackedScene> _enemies = null;
    private Level _level = null;
    private bool _enabled = false;
    public Spawner(Level level, List<PackedScene> enemies)
    {
        _enemies = enemies;
        _level = level;
    }
    public async virtual void Enable()
    {
        _enabled = true;
        while(_enabled)
        {
            SpawnResourceOffScreen();
            await Task.Delay(Cooldown);
        }
    }
    public virtual void Disable()
    {
        _enabled = false;
    }
    private void SpawnResourceOffScreen()
    {
        if (_enemies.Count <= 0)
        {
            return;
        }
        Character enemy = _enemies[0].Instance<Character>();
        float randomAngle = GD.Randf() * Mathf.Tau;
        Vector2 position = (new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * _distance) + Player.Instance.GlobalPosition;
        enemy.GlobalPosition = position;
        _level.AddChild(enemy);
    }
}