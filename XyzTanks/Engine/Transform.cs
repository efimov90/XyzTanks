namespace XyzTanks.Engine;
public class Transform
{
    private Vector2Int _position;

    public Vector2Int Position
    {
        get => _position;
        set
        {
            if(_position != value)
            {
                PreviousPosition = _position;
                _position = value;
            }
        }
    }

    public Vector2Int? PreviousPosition { get; set; }
    public Orientation Orientation { get; set; }

    public Vector2Int Upper => Position + Vector2Int.Up;
    public Vector2Int Lower => Position + Vector2Int.Down;
    public Vector2Int Righter => Position + Vector2Int.Right;
    public Vector2Int Lefter => Position + Vector2Int.Left;

    public Orientation GetNextOrientationByNextPosition(Vector2Int nextPosition)
    {
        if (nextPosition == Upper)
        {
            return Orientation.Up;
        }
        else if (nextPosition == Lower)
        {
            return Orientation.Down;
        }
        else if (nextPosition == Lefter)
        {
            return Orientation.Left;
        }
        else if (nextPosition == Righter)
        {
            return Orientation.Right;
        }
        throw new InvalidOperationException("Нет ориентации");
    }

    public Vector2Int GetNextPositionByOrientation() => Orientation switch
    {
        Orientation.Up => Upper,
        Orientation.Down => Lower,
        Orientation.Right => Righter,
        Orientation.Left => Lefter,
        _ => throw new InvalidOperationException("Невозможное состояние")
    };

    public Vector2Int GetOppositeDirectionPosition() => Orientation switch
    {
        Orientation.Up => Lower,
        Orientation.Down => Upper,
        Orientation.Right => Lefter,
        Orientation.Left => Righter,
        _ => throw new InvalidOperationException("Невозможное состояние")
    };
}
