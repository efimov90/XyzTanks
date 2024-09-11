using XyzTanks.Engine;

namespace XyzTanks.Map;

public class RedrawRequiredAtArgs
{
    public RedrawRequiredAtArgs(Vector2Int position, RedrawType redrawType)
    {
        Position = position;
        RedrawType = redrawType;
    }

    public RedrawType RedrawType {  get; }

    public Vector2Int Position { get; }
}
