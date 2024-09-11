using XyzTanks.Engine;
using XyzTanks.Map;

namespace XyzTanks.Units;

public class Projectile : IUpdateable, IDisposable
{
    private ILevelMapManager _levelMapManager;

    public Projectile(ILevelMapManager levelMapManager)
    {
        _levelMapManager = levelMapManager
            ?? throw new ArgumentNullException(nameof(levelMapManager));
    }

    public Transform Transform { get; } = new Transform();

    public bool IsAlive { get; set; } = true;

    public void Update(double deltaSeconds)
    {
        Transform.Position = Transform.GetNextPositionByOrientation();

        if (!_levelMapManager.IsProjectilePassable(Transform.Position))
        {
            if (_levelMapManager.IsDamageable(Transform.Position))
            {
                _levelMapManager.Damage(Transform.Position);
            }

            IsAlive = false;
        }
    }

    public void Dispose()
    {
        _levelMapManager = null!;
    }
}