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

    public void Set(int x, int y, StaticObject staticObject)
    {
        _map[x][y] = staticObject;
    }

    public bool IsWalkableAtCoordinate(Vector2 vector) =>
        IsWalkableAtCoordinate((int)vector.X, (int)vector.Y);

    public bool IsWalkableAtCoordinate(int x, int y) =>
        x > 0
        && y > 0
        && x < LevelWidth
        && y < LevelHeight
        && _map[x][y] == StaticObject.Empty;
}
