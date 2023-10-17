using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Level : Node2D
{
    private Player _player = null;
    private Vector2 _playerCoords = Vector2.Zero;
    private TileMap _tileMap = null;
    public override void _Ready()
    {
        base._Ready();
        _player = Player.Instance;
        _tileMap = GetNode<TileMap>("TileMap");

        _playerCoords = GetPlayerMapPosition(_player, _tileMap);
    }
    public override void _Process(float delta)
    {
        base._Process(delta);
        _playerCoords = GetPlayerMapPosition(_player, _tileMap);
        setTilesAroundPlayer(32, 32, _playerCoords);
    }
    private Vector2 GetPlayerMapPosition(Player player, TileMap tileMap)
    {
        return tileMap.WorldToMap(ToLocal(player.GlobalTransform.origin));
    }
    private void setTilesAroundPlayer(int distanceX, int distanceY, Vector2 playerCoords)
    {
        int leftColumn = (int)playerCoords.x - distanceX;
        int rightColumn = (int)playerCoords.x + distanceX;

        int topRow = (int)playerCoords.y + distanceY;
        int bottomRow = (int)playerCoords.y - distanceY;

        List<Vector2> tiles = new List<Vector2>();
        for (int rowIndex = topRow; rowIndex > bottomRow; rowIndex--)
        {
            for (int columnIndex = leftColumn; columnIndex < rightColumn; columnIndex++)
            {
                Vector2 tilePosition = new Vector2(columnIndex, rowIndex);
                if (_tileMap.GetCellv(tilePosition) != 0)
                {
                    _tileMap.SetCellv(tilePosition, 0);
                }
            }
        }
    }

}
