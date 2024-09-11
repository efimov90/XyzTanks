using System.Numerics;

namespace XyzTanks;
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

    public List<Vector2> GetPossibleDirections()
    {
        var result = new List<Vector2>();

        if (_levelMapManager.IsWalkableAtCoordinate(Lefter))
        {
            result.Add(Lefter);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Righter))
        {
            result.Add(Righter);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Upper))
        {
            result.Add(Upper);
        }

        if (_levelMapManager.IsWalkableAtCoordinate(Lower))
        {
            result.Add(Lower);
        }

        return result;
    }
}
