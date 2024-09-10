using System.Numerics;

namespace XyzTanks;
internal class Game
{
    private readonly Random _random = new Random(DateTime.Now.Second);

    private readonly LevelLoader _levelLoader;
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
    private LevelMap _map;
    private Tank _tank;

    private List<Tank> _enemyTanks = new List<Tank>();

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

        LoadLevel();
    }

    private void LoadLevel()
    {
        if (File.Exists($"Levels/level{_level}.txt"))
        {
            _projectiles.Clear();
            _map = _levelLoader.LoadLevel($"Levels/level{_level}.txt");
            _renderer.SetMap(_map);
            _tank = new Tank();
            _tank.Position = _map.GetRandomTankPosition();

            _tank.Orientation = GetRandomOrientation();
            _lastTankPosition = _tank.Position;
            _lastTankOrientation = _tank.Orientation;
            _renderer.RenderWalls();
            _renderer.DrawTank(_tank.Position, _tank.Orientation, true);

            for (int enemyCount = 0; enemyCount < _level; enemyCount++)
            {
                var newEnemyTank = new Tank();

                var position = _map.GetRandomTankPosition();

                while(_enemyTanks.Any(et => et.Position == position) || _tank.Position == position)
                {
                    position = _map.GetRandomTankPosition();
                }

                newEnemyTank.Position = position;
                newEnemyTank.Orientation = GetRandomOrientation();
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
                _tank.Orientation = Orientation.Up;
                if (_map.IsWalkableAtCoordinate(_tank.Upper)
                    && !_enemyTanks.Any(et => et.Position == _tank.Upper))
                {
                    _tank.MoveUp();
                }
                break;
            case InputAction.Down:
                _tank.Orientation = Orientation.Down;
                if (_map.IsWalkableAtCoordinate(_tank.Lower)
                    && !_enemyTanks.Any(et => et.Position == _tank.Lower))
                {
                    _tank.MoveDown();
                }
                break;
            case InputAction.Left:
                _tank.Orientation = Orientation.Left;
                if (_map.IsWalkableAtCoordinate(_tank.Lefter)
                    && !_enemyTanks.Any(et => et.Position == _tank.Lefter))
                {
                    _tank.MoveLeft();
                }
                break;
            case InputAction.Right:
                _tank.Orientation = Orientation.Right;
                if (_map.IsWalkableAtCoordinate(_tank.Righter)
                    && !_enemyTanks.Any(et => et.Position == _tank.Righter))
                {
                    _tank.MoveRight();
                }
                break;

            case InputAction.Fire:
                if (_nextShotTime > DateTime.Now)
                {
                    return;
                }

                SpawnProjectile(_tank.Position, _tank.Orientation);

                _nextShotTime = DateTime.Now.Add(_fireDelay);
                break;
            case InputAction.Exit:
                _running = false;
                break;
        }
    }

    private void SpawnProjectile(Vector2 position, Orientation orientation)
    {
        _projectiles.Add(new Projectile
        {
            Position = position,
            Orientation = orientation
        });
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

        if(_enemyTanks.Count == 0)
        {
            _level++;
            LoadLevel();
        }

        _renderer.RenderGameInfo(_level, _tank.Health);

        _secondsFromLastUpdate = 0;
    }

    private void UpdateEnemiesTanks()
    {
        var listEnemyToDispose = new List<Tank>();
        var listProjectilesToRemove = new List<Projectile>();

        foreach(var enemy in _enemyTanks)
        {
            UpdateEnemyTank(listEnemyToDispose, listProjectilesToRemove, enemy);
        }
        _projectiles.RemoveAll(listProjectilesToRemove.Contains);
        _enemyTanks.RemoveAll(listEnemyToDispose.Contains);
    }

    private void UpdateEnemyTank(List<Tank> listEnemyToDispose, List<Projectile> listProjectilesToRemove, Tank enemy)
    {
        var lastEnemyTankPosition = enemy.Position;
        var lastEnemyTankOrientation = enemy.Orientation;

        if (_projectiles.Any(x => x.Position == enemy.Position))
        {
            enemy.Health--;
            listProjectilesToRemove.Add(_projectiles.First(x => x.Position == enemy.Position));
            if (enemy.Health <= 0)
            {
                listEnemyToDispose.Add(enemy);
            }
        }

        if((enemy.Position.X == _tank.Position.X
                && ((enemy.Orientation == Orientation.Up && enemy.Position.Y > _tank.Position.Y)
                    || (enemy.Orientation == Orientation.Down && enemy.Position.Y < _tank.Position.Y)))
            || (enemy.Position.Y == _tank.Position.Y
                && ((enemy.Orientation == Orientation.Left && enemy.Position.X > _tank.Position.X)
                    || (enemy.Orientation == Orientation.Right && enemy.Position.X < _tank.Position.X))))
        {
            SpawnProjectile(enemy.Position, enemy.Orientation);
        }

        IList<Vector2> nextPossiblePositions = GetPossibleDirections(enemy);

        Vector2 nextPosition;

        if (nextPossiblePositions.Count == 0)
        {
            return;
        }

        var nextDefaultPosition = enemy.GetNextPositionByOrientation();

        if (nextPossiblePositions.Count > 2 || !_map.IsWalkableAtCoordinate(nextDefaultPosition))
        {
            nextPosition = nextPossiblePositions[_random.Next(nextPossiblePositions.Count)];
            enemy.Orientation = enemy.GetNextOrientationByNextPosition(nextPosition);
        }
        else if(nextPossiblePositions.Count == 2 && !_map.IsWalkableAtCoordinate(nextDefaultPosition))
        {
            nextPosition = nextPossiblePositions.Where(x => x != enemy.GetOppositeDirectionPosition()).First();
            
        }
        else if (nextPossiblePositions.Count == 1)
        {
            nextPosition = enemy.GetOppositeDirectionPosition();
        }
        else
        {
            nextPosition = nextDefaultPosition;
        }

        enemy.Position = nextPosition;

        _renderer.EraseAtMapCoordinate(lastEnemyTankPosition);

        _renderer.DrawTank(enemy.Position, enemy.Orientation);
    }

    private List<Vector2> GetPossibleDirections(Tank enemy)
    {
        var result = new List<Vector2>();

        if (_map.IsWalkableAtCoordinate(enemy.Lefter))
        {
            result.Add(enemy.Lefter);
        }

        if (_map.IsWalkableAtCoordinate(enemy.Righter))
        {
            result.Add(enemy.Righter);
        }

        if (_map.IsWalkableAtCoordinate(enemy.Upper))
        {
            result.Add(enemy.Upper);
        }

        if (_map.IsWalkableAtCoordinate(enemy.Lower))
        {
            result.Add(enemy.Lower);
        }

        return result;
    }

    private void UpdatePlayerTank()
    {
        var listProjectilesToRemove = new List<Projectile>();

        if(_lastTankPosition != _tank.Position)
        {
            _renderer.EraseAtMapCoordinate(_lastTankPosition);
        }

        if (_projectiles.Any(x => x.Position == _tank.Position))
        {
            _tank.Health--;
            listProjectilesToRemove.Add(_projectiles.First(x => x.Position == _tank.Position));

            if (_tank.Health <= 0)
            {
                _running = false;
            }
        }

        _projectiles.RemoveAll(listProjectilesToRemove.Contains);

        _renderer.DrawTank(_tank.Position, _tank.Orientation, true);

        _lastTankPosition = _tank.Position;
        _lastTankOrientation = _tank.Orientation;
    }

    private void UpdateProjectiles()
    {
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

            if (!_map.IsProjectilePassable(projectile.Position))
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
    }
}
