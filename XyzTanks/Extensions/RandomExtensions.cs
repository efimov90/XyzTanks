using XyzTanks.Engine;

namespace XyzTanks.Extensions;
public static class RandomExtensions
{
    public static Vector2Int Next(this Random random, int maxX, int maxY) =>
        new(random.Next(maxX), random.Next(maxY));
}
