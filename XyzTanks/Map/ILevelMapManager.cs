using XyzTanks.Engine;

namespace XyzTanks.Map;
public interface ILevelMapManager
{
    IList<IList<StaticObject>> Map { get; }

    void Damage(int x, int y);
    void Damage(Vector2Int position);
    Vector2Int GetRandomTankPosition();
    bool IsDamageable(int x, int y);
    bool IsDamageable(Vector2Int position);
    bool IsProjectilePassable(int x, int y);
    bool IsProjectilePassable(Vector2Int position);
    bool IsWalkableAtCoordinate(int x, int y);
    bool IsWalkableAtCoordinate(Vector2Int position);
    void LoadLevel(string name);
    void Set(int x, int y, StaticObject staticObject);
    void Set(Vector2Int position, StaticObject staticObject);
}