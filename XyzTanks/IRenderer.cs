using System.Numerics;

namespace XyzTanks;
public interface IRenderer
{
    void DrawProjectileAt(Vector2 position);
    void DrawTank(Vector2 position, Orientation tankOrientation, bool playerTank = false);
    void EraseAtMapCoordinate(Vector2 coordinate);
    void RenderGameInfo(int level, int health);
    void RenderWalls();
    void SetMap(LevelMap map);
}
