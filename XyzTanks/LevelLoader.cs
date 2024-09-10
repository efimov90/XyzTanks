namespace XyzTanks;
internal class LevelLoader
{
    public LevelMap LoadLevel(string name)
    {
        var result = new LevelMap();

        var rows = File.ReadAllLines(name);

        for (int y = 0; y < LevelMap.LevelHeight; y++)
        {
            for (int x = 0; x < LevelMap.LevelWidth; x++)
            {
                result.Set(x, y, MapCharacter(rows[y][x]));
            }
        }

        return result;
    }

    public StaticObject MapCharacter(char character) => character switch
    {
        'W' => StaticObject.Wall,
        'D' => StaticObject.DamagedWall,
        'R' => StaticObject.River,
        _ => StaticObject.Empty
    };
}
