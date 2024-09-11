using XyzTanks.Engine;
using XyzTanks.Extensions;

namespace XyzTanks.Map;
public class LevelMapManager : ILevelMapManager
{
    private readonly Random _random = new Random(DateTime.Now.Second);
    public const int LevelHeight = 13;
    public const int LevelWidth = 13;

    private IList<IList<StaticObject>> _map;

    public IList<IList<StaticObject>> Map => _map;

    public LevelMapManager()
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

    public void LoadLevel(string name)
    {
        var rows = File.ReadAllLines(name);

        for (int y = 0; y < LevelHeight; y++)
        {
            for (int x = 0; x < LevelWidth; x++)
            {
                Set(x, y, rows[y][x].ToStaticObject());
            }
        }
    }

    public void Set(Vector2Int position, StaticObject staticObject)
        => Set(position.X, position.Y, staticObject);

    public void Set(int x, int y, StaticObject staticObject)
    {
        _map[x][y] = staticObject;
    }

    public bool IsWalkableAtCoordinate(Vector2Int position) =>
        IsWalkableAtCoordinate(position.X, position.Y);

    public bool IsWalkableAtCoordinate(int x, int y) =>
        IsOnMap(x, y)
        && _map[x][y] == StaticObject.Empty;

    private static bool IsOnMap(int x, int y) =>
        x >= 0
        && y >= 0
        && x < LevelWidth
        && y < LevelHeight;

    public bool IsProjectilePassable(Vector2Int position) =>
        IsProjectilePassable(position.X, position.Y);

    public bool IsProjectilePassable(int x, int y) =>
        IsOnMap(x, y)
        && (_map[x][y] == StaticObject.Empty
            || _map[x][y] == StaticObject.River);

    public bool IsDamageable(Vector2Int position) =>
        IsDamageable(position.X, position.Y);

    public bool IsDamageable(int x, int y) =>
        IsOnMap(x, y)
        && (_map[x][y] == StaticObject.Wall
            || _map[x][y] == StaticObject.DamagedWall);

    public void Damage(Vector2Int position)
        => Damage(position.X, position.Y);

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

    public Vector2Int GetRandomTankPosition()
    {
        int x;
        int y;
        do
        {
            x = _random.Next(LevelWidth);
            y = _random.Next(LevelHeight);
        }
        while (!IsWalkableAtCoordinate(x, y));

        return new Vector2Int(x, y);
    }
}
