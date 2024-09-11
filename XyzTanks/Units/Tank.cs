using System.Numerics;
using XyzTanks.Engine;

namespace XyzTanks.Units;
public class Tank
{
    public Vector2 Position { get; set; }
    public Orientation Orientation { get; set; } = Orientation.Up;

    public int Health { get; set; } = 2;

    public void MoveUp()
    {
        Position = Upper;
    }

    public void MoveDown()
    {
        Position = Lower;
    }

    public void MoveLeft()
    {
        Position = Lefter;
    }

    public void MoveRight()
    {
        Position = Righter;
    }

    internal Vector2 GetNextPositionByOrientation() => Orientation switch
    {
        Orientation.Up => Upper,
        Orientation.Down => Lower,
        Orientation.Right => Righter,
        Orientation.Left => Lefter,
        _ => throw new InvalidOperationException("Невозможное состояние")
    };

    internal Vector2 GetOppositeDirectionPosition() => Orientation switch
    {
        Orientation.Up => Lower,
        Orientation.Down => Upper,
        Orientation.Right => Lefter,
        Orientation.Left => Righter,
        _ => throw new InvalidOperationException("Невозможное состояние")
    };

    internal Orientation GetNextOrientationByNextPosition(Vector2 nextPosition)
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

    public Vector2 Upper => Position + new Vector2(0, -1);
    public Vector2 Lower => Position + new Vector2(0, 1);
    public Vector2 Righter => Position + new Vector2(1, 0);
    public Vector2 Lefter => Position + new Vector2(-1, 0);
}
