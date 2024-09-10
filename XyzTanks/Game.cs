using System.Numerics;

namespace XyzTanks;
internal class Game
{
    private readonly LevelLoader _levelLoader;
    private readonly IInputReader _inputReader;
    private readonly IRenderer _renderer;

    private readonly ShowTextState _showTextState;

    private readonly TimeSpan _fireDelay = TimeSpan.FromSeconds(1);

    private readonly Random _random = new Random(DateTime.Now.Second);

    private bool _running = false;

    private double _tickSeconds = 1d;
    private double _secondsFromLastUpdate = 0d;

    private DateTime _lastUpdateTime = DateTime.Now;

    private DateTime _nextShotTime = DateTime.Now;

    private int _level = 1;
    private LevelMap _map;
    private Tank _tank;

    private Vector2 _lastTankPosition;

    private List<Projectile> _projectiles = new List<Projectile>();
    private Orientation _lastTankOrientation;

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
        _tank.Position = new Vector2(1, 1);
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
        _lastTankOrientation = _tank.Orientation;

        switch (e.InputAction)
        {
            case InputAction.Up:
                _tank.Orientation = Orientation.Up;
                if (_map.IsWalkableAtCoordinate(_tank.Upper))
                {
                    _tank.MoveUp();
                }
                break;
            case InputAction.Down:
                _tank.Orientation = Orientation.Down;
                if (_map.IsWalkableAtCoordinate(_tank.Lower))
                {
                    _tank.MoveDown();
                }
                break;
            case InputAction.Left:
                _tank.Orientation = Orientation.Left;
                if (_map.IsWalkableAtCoordinate(_tank.Lefter))
                {
                    _tank.MoveLeft();
                }
                break;
            case InputAction.Right:
                _tank.Orientation = Orientation.Right;
                if (_map.IsWalkableAtCoordinate(_tank.Righter))
                {
                    _tank.MoveRight();
                }
                break;

            case InputAction.Fire:
                if (_nextShotTime > DateTime.Now)
                {
                    return;
                }

                _projectiles.Add(new Projectile
                {
                    Position = _tank.Position,
                    Orientation = _tank.Orientation
                });

                _nextShotTime = DateTime.Now.Add(_fireDelay);
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

        if (_lastTankPosition != _tank.Position
            || _lastTankOrientation != _tank.Orientation)
        {
            _renderer.EraseAtMapCoordinate(_lastTankPosition);
            _renderer.DrawTank(_tank.Position, _tank.Orientation, true);
        }

        var projectilesToRemove = new List<Projectile>();

        foreach (var projectile in _projectiles)
        {
            var projectileLastPosition = projectile.Position;

            if (projectile.Position.X >= 0
                && projectile.Position.X <= LevelMap.LevelWidth - 1
                && projectile.Position.Y >= 0
                && projectile.Position.Y <= LevelMap.LevelHeight - 1
                && projectile.Position != _tank.Position)
            {
                _renderer.EraseAtMapCoordinate(projectileLastPosition);
            }

            projectile.Position = projectile.Orientation switch
            {
                Orientation.Up => projectile.Upper,
                Orientation.Down => projectile.Lower,
                Orientation.Left => projectile.Lefter,
                Orientation.Right => projectile.Righter,
                _ => throw new InvalidOperationException("Невозможное состояние")
            };

            if (projectile.Position.X < 0
                || projectile.Position.X > LevelMap.LevelWidth
                || projectile.Position.Y < 0
                || projectile.Position.Y > LevelMap.LevelHeight)
            {
                projectilesToRemove.Add(projectile);

                continue;
            }

            if(!_map.IsProjectilePassable(projectile.Position))
            {
                if (_map.IsDamageable(projectile.Position))
                {
                    _map.Damage(projectile.Position);
                    _renderer.EraseAtMapCoordinate(projectile.Position);
                }

                projectilesToRemove.Add(projectile);
                continue;
            }

            _renderer.DrawProjectileAt(projectile.Position);
        }

        _projectiles.RemoveAll(projectilesToRemove.Contains);

        _renderer.RenderGameInfo(_level, _tank.Health);

        _secondsFromLastUpdate = 0;
    }
}
