using XyzTanks.Map;

namespace XyzTanks.Extensions;
public static class CharExtensions
{
    public static StaticObject ToStaticObject(this char character) => character switch
    {
        'W' => StaticObject.Wall,
        'D' => StaticObject.DamagedWall,
        'R' => StaticObject.River,
        _ => StaticObject.Empty
    };
}
