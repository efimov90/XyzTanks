using Microsoft.Extensions.DependencyInjection;
using XyzTanks;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IInputReader, ConsoleInputReader>()
    .AddSingleton<IRenderer, ConsoleRenderer>()
    .AddSingleton<ILevelMapManager, LevelMapManager>()
    .AddSingleton<ShowTextState>()
    .AddSingleton<Game>()
    .AddTransient<EnemyTank>()
    .BuildServiceProvider();

var game = serviceProvider.GetRequiredService<Game>();
await game.RunAsync();
