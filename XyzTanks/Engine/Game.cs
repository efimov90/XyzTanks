using Microsoft.Extensions.DependencyInjection;
using XyzTanks.Input;
using XyzTanks.Map;
using XyzTanks.Rendering;
using XyzTanks.Units;

namespace XyzTanks.Engine;
internal class Game
{
    private readonly Random _random = new Random(DateTime.Now.Second);
    private readonly IServiceProvider _serviceProvider;
    private readonly IInputReader _inputReader;
    private readonly IRenderer _renderer;

    private readonly ShowTextState _showTextState;

    private readonly TimeSpan _fireDelay = TimeSpan.FromSeconds(1);

    private bool _running = false;

    private double _tickSeconds = 1d;
    private double _secondsFromLastUpdate = 0d;

    private DateTime _lastUpdateTime = DateTime.Now;

    private DateTime _nextShotTime = DateTime.Now;

    private int _level = 1;
    private ILevelMapManager _levelMapManager;
    private Tank _tank = null!;

    private readonly List<EnemyTank> _enemyTanks = new();

    private Vector2Int _lastTankPosition;

    private readonly List<Projectile> _projectiles = new();

    public Game(
        IServiceProvider serviceProvider,
        IInputReader inputReader,
        IRenderer renderer,
        ShowTextState showTextState,
        ILevelMapManager levelMapManager)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _showTextState = showTextState ?? throw new ArgumentNullException(nameof(showTextState));
        _levelMapManager = levelMapManager ?? throw new ArgumentNullException(nameof(levelMapManager));

        _inputReader.InputActionCalled += OnInputActionCalled;

        LoadLevel();
    }

    private void LoadLevel()
    {
        if (File.Exists($"Levels/level{_level}.txt"))
        {
            _projectiles.Clear();
            _levelMapManager.LoadLevel($"Levels/level{_level}.txt");
            _tank = _serviceProvider.GetRequiredService<Tank>();
            _tank.Transform.Position = _levelMapManager.GetRandomTankPosition();

            _tank.Transform.Orientation = GetRandomOrientation();
            _lastTankPosition = _tank.Transform.Position;
            _renderer.RenderWalls();
            _renderer.DrawTank(_tank.Transform.Position, _tank.Transform.Orientation, true);

            for (int enemyCount = 0; enemyCount < _level; enemyCount++)
            {
                var newEnemyTank = _serviceProvider.GetRequiredService<EnemyTank>();

                var position = _levelMapManager.GetRandomTankPosition();

                while (_enemyTanks.Any(et => et.Transform.Position == position) || _tank.Transform.Position == position)
                {
                    position = _levelMapManager.GetRandomTankPosition();
                }

                newEnemyTank.Transform.Position = position;
                newEnemyTank.Transform.Orientation = GetRandomOrientation();
                _enemyTanks.Add(newEnemyTank);
            }
        }
        else
        {
            _running = false;
        }
    }

    private Orientation GetRandomOrientation()
    {
        var orientationValues = Enum.GetValues(typeof(Orientation));
        return (Orientation)(orientationValues?.GetValue(_random.Next(orientationValues.Length)) ?? Orientation.Up);
    }

    public async Task RunAsync()
    {
        _running = true;

        while (_running)
        {
            var currentTime = DateTime.Now;

            var deltaTime = currentTime - _lastUpdateTime;

            Update(deltaTime.TotalSeconds);

            _lastUpdateTime = currentTime;
        }

        _showTextState.RenderGameOverScreen(_tank.Health > 0);

        await Task.CompletedTask;
    }

    private void OnInputActionCalled(object? sender, InputEventArgs e)
    {
        switch (e.InputAction)
        {
            case InputAction.Up:
                _tank.Transform.Orientation = Orientation.Up;
                if (_levelMapManager.IsWalkableAtCoordinate(_tank.Transform.Upper)
                    && !_enemyTanks.Any(et => et.Transform.Position == _tank.Transform.Upper))
                {
                    _tank.MoveUp();
                }
                break;
            case InputAction.Down:
                _tank.Transform.Orientation = Orientation.Down;
                if (_levelMapManager.IsWalkableAtCoordinate(_tank.Transform.Lower)
                    && !_enemyTanks.Any(et => et.Transform.Position == _tank.Transform.Lower))
                {
                    _tank.MoveDown();
                }
                break;
            case InputAction.Left:
                _tank.Transform.Orientation = Orientation.Left;
                if (_levelMapManager.IsWalkableAtCoordinate(_tank.Transform.Lefter)
                    && !_enemyTanks.Any(et => et.Transform.Position == _tank.Transform.Lefter))
                {
                    _tank.MoveLeft();
                }
                break;
            case InputAction.Right:
                _tank.Transform.Orientation = Orientation.Right;
                if (_levelMapManager.IsWalkableAtCoordinate(_tank.Transform.Righter)
                    && !_enemyTanks.Any(et => et.Transform.Position == _tank.Transform.Righter))
                {
                    _tank.MoveRight();
                }
                break;

            case InputAction.Fire:
                if (_nextShotTime > DateTime.Now)
                {
                    return;
                }

                SpawnProjectile(_tank.Transform.Position, _tank.Transform.Orientation);

                _nextShotTime = DateTime.Now.Add(_fireDelay);
                break;
            case InputAction.Exit:
                _running = false;
                break;
        }
    }

    private void SpawnProjectile(Vector2Int position, Orientation orientation)
    {
        var projectile = _serviceProvider.GetRequiredService<Projectile>();

        projectile.Transform.Position = position;
        projectile.Transform.Orientation = orientation;

        _projectiles.Add(projectile);
    }

    private void Update(double totalSeconds)
    {
        _secondsFromLastUpdate += totalSeconds;

        if (_secondsFromLastUpdate < _tickSeconds)
        {
            return;
        }

        _inputReader.Update();

        UpdateProjectiles();
        UpdatePlayerTank();
        UpdateEnemiesTanks();

        if (_enemyTanks.Count == 0)
        {
            _level++;
            LoadLevel();
        }

        _renderer.RenderGameInfo(_level, _tank.Health);

        _secondsFromLastUpdate = 0;
    }

    private void UpdateEnemiesTanks()
    {
        var listEnemyToDispose = new List<EnemyTank>();
        var listProjectilesToRemove = new List<Projectile>();

        foreach (var enemy in _enemyTanks)
        {
            UpdateEnemyTank(listEnemyToDispose, listProjectilesToRemove, enemy);
        }
        _projectiles.RemoveAll(listProjectilesToRemove.Contains);
        _enemyTanks.RemoveAll(listEnemyToDispose.Contains);
    }

    private void UpdateEnemyTank(List<EnemyTank> listEnemyToDispose, List<Projectile> listProjectilesToRemove, EnemyTank enemy)
    {
        var lastEnemyTankPosition = enemy.Transform.Position;
        var lastEnemyTankOrientation = enemy.Transform.Orientation;

        if (_projectiles.Any(x => x.Transform.Position == enemy.Transform.Position))
        {
            enemy.Health--;
            listProjectilesToRemove.Add(_projectiles.First(x => x.Transform.Position == enemy.Transform.Position));
            if (enemy.Health <= 0)
            {
                listEnemyToDispose.Add(enemy);
            }
        }

        if (enemy.Transform.Position.X == _tank.Transform.Position.X
                && (enemy.Transform.Orientation == Orientation.Up && enemy.Transform.Position.Y > _tank.Transform.Position.Y
                    || enemy.Transform.Orientation == Orientation.Down && enemy.Transform.Position.Y < _tank.Transform.Position.Y)
            || enemy.Transform.Position.Y == _tank.Transform.Position.Y
                && (enemy.Transform.Orientation == Orientation.Left && enemy.Transform.Position.X > _tank.Transform.Position.X
                    || enemy.Transform.Orientation == Orientation.Right && enemy.Transform.Position.X < _tank.Transform.Position.X))
        {
            SpawnProjectile(enemy.Transform.Position, enemy.Transform.Orientation);
        }

        IList<Vector2Int> nextPossiblePositions = enemy.GetPossibleDirections();

        Vector2Int nextPosition;

        if (nextPossiblePositions.Count == 0)
        {
            return;
        }

        var nextDefaultPosition = enemy.Transform.GetNextPositionByOrientation();

        if (nextPossiblePositions.Count > 2 || !_levelMapManager.IsWalkableAtCoordinate(nextDefaultPosition))
        {
            nextPosition = nextPossiblePositions[_random.Next(nextPossiblePositions.Count)];
            enemy.Transform.Orientation = enemy.Transform.GetNextOrientationByNextPosition(nextPosition);
        }
        else if (nextPossiblePositions.Count == 2 && !_levelMapManager.IsWalkableAtCoordinate(nextDefaultPosition))
        {
            nextPosition = nextPossiblePositions.Where(x => x != enemy.Transform.GetOppositeDirectionPosition()).First();

        }
        else if (nextPossiblePositions.Count == 1)
        {
            nextPosition = enemy.Transform.GetOppositeDirectionPosition();
        }
        else
        {
            nextPosition = nextDefaultPosition;
        }

        enemy.Transform.Position = nextPosition;

        _renderer.EraseAtMapCoordinate(lastEnemyTankPosition);

        _renderer.DrawTank(enemy.Transform.Position, enemy.Transform.Orientation);
    }



    private void UpdatePlayerTank()
    {
        var listProjectilesToRemove = new List<Projectile>();

        if (_lastTankPosition != _tank.Transform.Position)
        {
            _renderer.EraseAtMapCoordinate(_lastTankPosition);
        }

        if (_projectiles.Any(x => x.Transform.Position == _tank.Transform.Position))
        {
            _tank.Health--;
            listProjectilesToRemove.Add(_projectiles.First(x => x.Transform.Position == _tank.Transform.Position));

            if (_tank.Health <= 0)
            {
                _running = false;
            }
        }

        _projectiles.RemoveAll(listProjectilesToRemove.Contains);

        _renderer.DrawTank(_tank.Transform.Position, _tank.Transform.Orientation, true);

        _lastTankPosition = _tank.Transform.Position;
    }

    private void UpdateProjectiles()
    {
        var projectilesToRemove = new List<Projectile>();

        foreach (var projectile in _projectiles)
        {
            var projectileLastPosition = projectile.Transform.Position;

            if (projectile.Transform.Position.X >= 0
                && projectile.Transform.Position.X <= LevelMapManager.LevelWidth - 1
                && projectile.Transform.Position.Y >= 0
                && projectile.Transform.Position.Y <= LevelMapManager.LevelHeight - 1
                && projectile.Transform.Position != _tank.Transform.Position)
            {
                _renderer.EraseAtMapCoordinate(projectileLastPosition);
            }

            projectile.Transform.Position = projectile.Transform.Orientation switch
            {
                Orientation.Up => projectile.Transform.Upper,
                Orientation.Down => projectile.Transform.Lower,
                Orientation.Left => projectile.Transform.Lefter,
                Orientation.Right => projectile.Transform.Righter,
                _ => throw new InvalidOperationException("Невозможное состояние")
            };

            if (projectile.Transform.Position.X < 0
                || projectile.Transform.Position.X > LevelMapManager.LevelWidth
                || projectile.Transform.Position.Y < 0
                || projectile.Transform.Position.Y > LevelMapManager.LevelHeight)
            {
                projectilesToRemove.Add(projectile);

                continue;
            }

            if (!_levelMapManager.IsProjectilePassable(projectile.Transform.Position))
            {
                if (_levelMapManager.IsDamageable(projectile.Transform.Position))
                {
                    _levelMapManager.Damage(projectile.Transform.Position);
                    _renderer.EraseAtMapCoordinate(projectile.Transform.Position);
                }

                projectilesToRemove.Add(projectile);
                continue;
            }

            _renderer.DrawProjectileAt(projectile.Transform.Position);
        }

        _projectiles.RemoveAll(projectilesToRemove.Contains);
    }
}
