using System.Numerics;

namespace XyzTanks;
public interface IRenderer
{
    void DrawTank(Vector2 position, TankOrientation tankOrientation);
    void EraseAt(Vector2 coordinate);
    void RenderCoordinates(Vector2 coordinate);
    void RenderGameInfo(int level);
    void RenderWalls(int height, int width);
}
