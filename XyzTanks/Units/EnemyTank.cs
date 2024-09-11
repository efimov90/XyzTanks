using XyzTanks.Engine;
using XyzTanks.Map;

namespace XyzTanks.Units;
public class EnemyTank : Tank, IUpdateable
{
    private readonly ILevelMapManager _levelMapManager;

    public EnemyTank(ILevelMapManager levelMapManager)
    {
        _levelMapManager = levelMapManager ?? throw new ArgumentNullException(nameof(levelMapManager));
    }

    public void Update(double deltaSeconds)
    {

    }

    public List<Vector2Int> GetPossibleDirections()
    {
        var result = new List<Vector2Int>();

        if (_levelMapManager.IsWalkableAtCoordinate(Transform.Lefter))
        {
            result.Add(Transform.Lefter);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Transform.Righter))
        {
            result.Add(Transform.Righter);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Transform.Upper))
        {
            result.Add(Transform.Upper);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Transform.Lower))
        {
            result.Add(Transform.Lower);
        }

        return result;
    }
}
