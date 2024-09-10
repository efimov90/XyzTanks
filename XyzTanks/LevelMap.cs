using System.Numerics;

namespace XyzTanks;
public class LevelMap
{
    public const int LevelHeight = 13;
    public const int LevelWidth = 13;

    private IList<IList<StaticObject>> _map;

    public IList<IList<StaticObject>> Map => _map;

    public LevelMap()
    {
        _map = new List<IList<StaticObject>>(LevelHeight);

        for (int i = 0; i < LevelHeight; i++)
        {
            var row = new List<StaticObject>(LevelWidth);

            for (int j = 0; j < LevelWidth; j++)
            {
                row.Add(StaticObject.Empty);
            }

            _map.Add(row);
        }
    }

    public void Set(Vector2 position, StaticObject staticObject)
        => Set((int)position.X, (int)position.Y, staticObject);

    public void Set(int x, int y, StaticObject staticObject)
    {
        _map[x][y] = staticObject;
    }

    public bool IsWalkableAtCoordinate(Vector2 position) =>
        IsWalkableAtCoordinate((int)position.X, (int)position.Y);

    public bool IsWalkableAtCoordinate(int x, int y) =>
        IsOnMap(x, y)
        && _map[x][y] == StaticObject.Empty;

    private static bool IsOnMap(int x, int y) =>
        x >= 0
        && y >= 0
        && x < LevelWidth
        && y < LevelHeight;

    public bool IsProjectilePassable(Vector2 position) =>
        IsProjectilePassable((int)position.X, (int)position.Y);

    public bool IsProjectilePassable(int x, int y) =>
        IsOnMap(x, y)
        && (_map[x][y] == StaticObject.Empty
            || _map[x][y] == StaticObject.River);

    public bool IsDamageable(Vector2 position) =>
        IsDamageable((int)position.X, (int)position.Y);

    public bool IsDamageable(int x, int y) =>
        IsOnMap(x, y)
        && (_map[x][y] == StaticObject.Wall
            || _map[x][y] == StaticObject.DamagedWall);

    public void Damage(Vector2 position)
        => Damage((int)position.X, (int)position.Y);

    public void Damage(int x, int y)
    {
        if (_map[x][y] == StaticObject.Wall)
        {
            Set(x, y, StaticObject.DamagedWall);
        }
        else if (_map[x][y] == StaticObject.DamagedWall)
        {
            Set(x, y, StaticObject.Empty);
        }
    }
}
