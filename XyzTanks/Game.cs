using System.Numerics;

namespace XyzTanks;
internal class Game
{
    private readonly LevelLoader _levelLoader;
    private readonly IInputReader _inputReader;
    private readonly IRenderer _renderer;

    private readonly ShowTextState _showTextState;

    private readonly Random _random = new Random(DateTime.Now.Second);

    private bool _running = false;

    private double _tickSeconds = 1d;
    private double _secondsFromLastUpdate = 0d;



    private DateTime _lastUpdateTime = DateTime.Now;
    private int _level = 1;
    private LevelMap _map;
    private Tank _tank;

    private int _fieldSizeX = 13;
    private int _fieldSizeY = 13;
    private Vector2 _lastTankPosition;

    public Game(
        LevelLoader levelLoader,
        IInputReader inputReader,
        IRenderer renderer,
        ShowTextState showTextState)
    {
        _levelLoader = levelLoader;
        _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _showTextState = showTextState ?? throw new ArgumentNullException(nameof(showTextState));

        _inputReader.InputActionCalled += OnInputActionCalled;

        _map = _levelLoader.LoadLevel("Levels/level1.txt");

        _renderer.SetMap(_map);

        _tank = new Tank();
    }

    public async Task RunAsync()
    {
        _running = true;

        _renderer.RenderWalls();

        while (_running)
        {
            var currentTime = DateTime.Now;

            var deltaTime = currentTime - _lastUpdateTime;

            Update(deltaTime.TotalSeconds);

            _lastUpdateTime = currentTime;
        }

        _showTextState.RenderGameOverScreen();

        await Task.CompletedTask;
    }

    private void OnInputActionCalled(object? sender, InputEventArgs e)
    {
        _lastTankPosition = _tank.Position;

        switch (e.InputAction)
        {
            case InputAction.Up:
                _tank.Orientation = TankOrientation.Up;
                _tank.MoveUp();
                break;
            case InputAction.Down:
                _tank.Orientation = TankOrientation.Down;
                _tank.MoveDown();
                break;
            case InputAction.Left:
                _tank.Orientation = TankOrientation.Left;
                _tank.MoveLeft();
                break;
            case InputAction.Right:
                _tank.Orientation = TankOrientation.Right;
                _tank.MoveRight();
                break;
            case InputAction.Exit:
                _running = false;
                break;
        }
    }

    private void Update(double totalSeconds)
    {
        _secondsFromLastUpdate += totalSeconds;

        if (_secondsFromLastUpdate < _tickSeconds)
        {
            return;
        }

        _inputReader.Update();

        if (_lastTankPosition != _tank.Position)
        {
            _renderer.EraseAtMapCoordinate(_lastTankPosition);
            _renderer.DrawTank(_tank.Position, _tank.Orientation, true);
        }

        _renderer.RenderGameInfo(_level);

        _secondsFromLastUpdate = 0;
    }
}
