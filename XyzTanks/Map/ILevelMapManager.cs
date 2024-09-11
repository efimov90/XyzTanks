using XyzTanks.Engine;
using XyzTanks.Units;

namespace XyzTanks.Map;
public interface ILevelMapManager : IUpdateable
{
    IList<IList<StaticObject>> Map { get; }
    List<EnemyTank> EnemyTanks { get; }
    List<Projectile> Projectiles { get; }

    event EventHandler<RedrawRequiredAtArgs> RedrawRequired;

    void Clear();
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
    void SpawnProjectile(Vector2Int position, Orientation orientation);
}