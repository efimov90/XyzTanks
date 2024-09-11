using System.Numerics;

namespace XyzTanks.Map;
public interface ILevelMapManager
{
    IList<IList<StaticObject>> Map { get; }

    void Damage(int x, int y);
    void Damage(Vector2 position);
    Vector2 GetRandomTankPosition();
    bool IsDamageable(int x, int y);
    bool IsDamageable(Vector2 position);
    bool IsProjectilePassable(int x, int y);
    bool IsProjectilePassable(Vector2 position);
    bool IsWalkableAtCoordinate(int x, int y);
    bool IsWalkableAtCoordinate(Vector2 position);
    void LoadLevel(string name);
    void Set(int x, int y, StaticObject staticObject);
    void Set(Vector2 position, StaticObject staticObject);
}