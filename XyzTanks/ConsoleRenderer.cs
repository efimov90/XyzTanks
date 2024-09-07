using System.Numerics;

namespace XyzTanks;
internal class ConsoleRenderer : IRenderer, IDisposable
{
    private const char _wallCharacter = '█';
    private const char _wallDamagedCharacter = '▒';
    private const char _emptyCharacter = ' ';
    private const char _projectileCharacter = '○';

    private const int _tileSizeX = 4;
    private const int _tileSizeY = 2;

    private char[][] _tankUpCharacters =
    [
        [ ' ', '╔', '╩', '╗'],
        [' ', '╚', '═', '╝']
    ];

    private char[][] _tankDownCharacters =
    [
        [' ', '╔', '═', '╗'],
        [' ', '╚', '╦', '╝']
    ];

    private char[][] _tankRightCharacters =
    [
        ['╔', '═', '╗', '_'],
        ['╚', '═', '╝', 'T']
    ];

    private char[][] _tankLeftCharacters =
    [
        ['_', '╔', '═', '╗'],
        ['T', '╚', '═', '╝']
    ];

    public ConsoleRenderer()
    {
        Console.CursorVisible = false;
    }

    public void RenderWalls(int height, int width)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(_wallCharacter, width));

        for (int y = 1; y < height - 1; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(_wallCharacter);
            Console.SetCursorPosition(width - 1, y);
            Console.Write(_wallCharacter);
        }

        Console.SetCursorPosition(0, height - 1);
        Console.Write(new string(_wallCharacter, width));
    }

    public void RenderCoordinates(Vector2 coordinate)
    {
        var consoleWindowHeight = Console.WindowHeight;
        Console.SetCursorPosition(0, consoleWindowHeight - 1);
        Console.Write($"X: {coordinate.X}, Y: {coordinate.Y}                     ");
    }

    public void RenderGameInfo(int level)
    {
        var consoleWindowHeight = Console.WindowHeight;
        Console.SetCursorPosition(0, consoleWindowHeight - 1);
        Console.Write($"Level: {level}                     ");
    }

    public void EraseAt(Vector2 coordinate)
    {
        Console.SetCursorPosition((int)coordinate.X, (int)coordinate.Y);
        Console.Write(_emptyCharacter);
    }

    public void Dispose()
    {
        Console.CursorVisible = true;
    }

    public void DrawTank(Vector2 position, TankOrientation tankOrientation)
    {
        var tankStartRenderPoint = position * new Vector2(_tileSizeX, _tileSizeY);

        int x = (int)tankStartRenderPoint.X;
        int y = (int)tankStartRenderPoint.Y;

        var tiles = GetTilesForOrientation(tankOrientation);

        for (; y < tankStartRenderPoint.Y + _tileSizeY; y++)
        {
            Console.SetCursorPosition(x, y);

            for (; x < tankStartRenderPoint.X + _tileSizeX; x++)
            {
                Console.Write(tiles[y % _tileSizeY][x % _tileSizeX]);
            }

            x = (int)tankStartRenderPoint.X;
        }
    }

    private char[][] GetTilesForOrientation(TankOrientation tankOrientation) => tankOrientation switch
    {
        TankOrientation.Up => _tankUpCharacters,
        TankOrientation.Down => _tankDownCharacters,
        TankOrientation.Left => _tankLeftCharacters,
        TankOrientation.Right => _tankRightCharacters,
        _ => throw new InvalidOperationException("Невозможная ориентация танка"),
    };
}
