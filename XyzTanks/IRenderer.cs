using System.Numerics;

namespace XyzTanks;
public interface IRenderer
{
    void DrawTank(Vector2 position, TankOrientation tankOrientation, bool playerTank = false);
    void EraseAtMapCoordinate(Vector2 coordinate);
    void RenderCoordinates(Vector2 coordinate);
    void RenderGameInfo(int level);
    void RenderWalls();
    void SetMap(LevelMap map);
}
