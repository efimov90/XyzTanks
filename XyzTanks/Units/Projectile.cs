using System.Numerics;
using XyzTanks.Engine;

namespace XyzTanks.Units;

internal class Projectile
{
    public Vector2 Position { get; set; }
    public Orientation Orientation { get; set; }

    public Vector2 Upper => Position + new Vector2(0, -1);
    public Vector2 Lower => Position + new Vector2(0, 1);
    public Vector2 Righter => Position + new Vector2(1, 0);
    public Vector2 Lefter => Position + new Vector2(-1, 0);
}