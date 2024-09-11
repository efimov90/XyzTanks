using System.Numerics;
using XyzTanks.Engine;

namespace XyzTanks.Rendering;
public interface IRenderer
{
    void DrawProjectileAt(Vector2Int position);
    void DrawTank(Vector2Int position, Orientation tankOrientation, bool playerTank = false);
    void EraseAtMapCoordinate(Vector2Int coordinate);
    void RenderGameInfo(int level, int health);
    void RenderWalls();
}
