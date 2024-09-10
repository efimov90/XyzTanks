using System.Numerics;

namespace XyzTanks;
internal class Tank
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

    public Vector2 Upper => Position + new Vector2(0, -1);
    public Vector2 Lower => Position + new Vector2(0, 1);
    public Vector2 Righter => Position + new Vector2(1, 0);
    public Vector2 Lefter => Position + new Vector2(-1, 0);
}
