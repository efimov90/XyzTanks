using System.Numerics;

namespace XyzTanks;
internal class Tank
{
    public Vector2 Position { get; set; }
    public TankOrientation Orientation { get; set; } = TankOrientation.Up;

    public void MoveUp()
    {
        Position += new Vector2(0, -1);
    }

    public void MoveDown()
    {
        Position += new Vector2(0, 1);
    }

    public void MoveLeft()
    {
        Position += new Vector2(-1, 0);
    }

    public void MoveRight()
    {
        Position += new Vector2(1,0);
    }
}
