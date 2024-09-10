﻿using System.Numerics;

namespace XyzTanks;
internal class ConsoleRenderer : IRenderer, IDisposable
{
    private const char _wallCharacter = '█';
    private const char _wallDamagedCharacter = '▒';
    private const char _emptyCharacter = ' ';
    private const char _riverCharacter = '~';
    private const char _projectileCharacter = 'O';

    private readonly ConsoleColor _playerTankColor = ConsoleColor.Green;
    private readonly ConsoleColor _enemyTankColor = ConsoleColor.White;

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

    private LevelMap _map = null!;

    public ConsoleRenderer()
    {
        Console.CursorVisible = false;
    }

    public void RenderWalls()
    {
        Console.SetCursorPosition(0, 0);

        for (var y = 0; y < LevelMap.LevelHeight * _tileSizeY; y++)
        {
            Console.SetCursorPosition(0, y);

            for (var x = 0; x < LevelMap.LevelWidth * _tileSizeX; x++)
            {
                Console.ForegroundColor = MapColorFromStaticObject(_map.Map[x / _tileSizeX][y / _tileSizeY]);
                Console.Write(MapCharacterFromStaticObject(_map.Map[x / _tileSizeX][y / _tileSizeY]));
                Console.ResetColor();
            }
        }
    }

    private ConsoleColor MapColorFromStaticObject(StaticObject staticObject) => staticObject switch
    {
        StaticObject.Wall => ConsoleColor.DarkRed,
        StaticObject.DamagedWall => ConsoleColor.DarkRed,
        StaticObject.River => ConsoleColor.Blue,
        _ => ConsoleColor.Black
    };

    private char MapCharacterFromStaticObject(StaticObject staticObject) => staticObject switch
    {
        StaticObject.Wall => _wallCharacter,
        StaticObject.DamagedWall => _wallDamagedCharacter,
        StaticObject.River => _riverCharacter,
        _ => _emptyCharacter
    };

    public void RenderGameInfo(int level, int health)
    {
        var consoleWindowHeight = Console.WindowHeight;
        Console.SetCursorPosition(0, consoleWindowHeight - 1);
        Console.Write($"Level: {level} | Health {health}     ");
    }

    public void EraseAtMapCoordinate(Vector2 coordinate)
    {
        var positionCoordinateX = (int)coordinate.X * _tileSizeX;
        var positionCoordinateY = (int)coordinate.Y * _tileSizeY;

        Console.SetCursorPosition(positionCoordinateX, positionCoordinateY);

        for (var y = positionCoordinateY; y < positionCoordinateY + _tileSizeY; y++)
        {
            Console.SetCursorPosition(positionCoordinateX, y);

            for (var x = positionCoordinateX; x < positionCoordinateX +  _tileSizeX; x++)
            {
                Console.ForegroundColor = MapColorFromStaticObject(_map.Map[x / _tileSizeX][y / _tileSizeY]);
                Console.Write(MapCharacterFromStaticObject(_map.Map[x / _tileSizeX][y / _tileSizeY]));
                Console.ResetColor();
            }
        }
    }

    public void Dispose()
    {
        Console.CursorVisible = true;
    }

    public void DrawTank(Vector2 position, Orientation tankOrientation, bool playerTank = false)
    {
        if (playerTank)
        {
            Console.ForegroundColor = _playerTankColor;
        }
        else
        {
            Console.ForegroundColor = _enemyTankColor;
        }

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

        Console.ResetColor();
    }

    private char[][] GetTilesForOrientation(Orientation tankOrientation) => tankOrientation switch
    {
        Orientation.Up => _tankUpCharacters,
        Orientation.Down => _tankDownCharacters,
        Orientation.Left => _tankLeftCharacters,
        Orientation.Right => _tankRightCharacters,
        _ => throw new InvalidOperationException("Невозможная ориентация танка"),
    };

    public void SetMap(LevelMap map)
    {
        _map = map;
    }

    public void DrawProjectileAt(Vector2 position)
    {
        var tankStartRenderPoint = position * new Vector2(_tileSizeX, _tileSizeY);

        int x = (int)tankStartRenderPoint.X;
        int y = (int)tankStartRenderPoint.Y;

        Console.SetCursorPosition(x, y);

        Console.Write(_projectileCharacter);
    }
}
