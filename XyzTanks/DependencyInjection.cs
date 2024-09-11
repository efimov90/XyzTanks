using Microsoft.Extensions.DependencyInjection;
using XyzTanks.Engine;
using XyzTanks.Input;
using XyzTanks.Map;
using XyzTanks.Rendering;
using XyzTanks.Units;

namespace XyzTanks;
public static class DependencyInjection
{
    public static IServiceCollection AddGame(this IServiceCollection services) => services
        .AddSingleton<IInputReader, ConsoleInputReader>()
        .AddSingleton<IRenderer, ConsoleRenderer>()
        .AddSingleton<ILevelMapManager, LevelMapManager>()
        .AddSingleton<ShowTextState>()
        .AddSingleton<Game>()
        .AddUnits();

    public static IServiceCollection AddUnits(this IServiceCollection services) => services
        .AddTransient<EnemyTank>()
        .AddTransient<Tank>()
        .AddTransient<Projectile>();
}
