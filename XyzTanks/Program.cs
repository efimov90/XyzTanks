using Microsoft.Extensions.DependencyInjection;
using XyzTanks;
using XyzTanks.Engine;

var serviceProvider = new ServiceCollection()
    .AddGame()
    .BuildServiceProvider();

var game = serviceProvider.GetRequiredService<Game>();
await game.RunAsync();
