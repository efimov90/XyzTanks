using System.Numerics;
using XyzTanks.Engine;

namespace XyzTanks.Rendering;
public interface IRenderer
{
    void DrawProjectileAt(Vector2 position);
    void DrawTank(Vector2 position, Orientation tankOrientation, bool playerTank = false);
    void EraseAtMapCoordinate(Vector2 coordinate);
    void RenderGameInfo(int level, int health);
    void RenderWalls();
}
