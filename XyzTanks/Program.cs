using Microsoft.Extensions.DependencyInjection;
using XyzTanks;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IInputReader, ConsoleInputReader>()
    .AddSingleton<IRenderer, ConsoleRenderer>()
    .AddSingleton<ShowTextState>()
    .AddSingleton<LevelLoader>()
    .AddSingleton<Game>()
    .BuildServiceProvider();

var game = serviceProvider.GetRequiredService<Game>();
await game.RunAsync();
