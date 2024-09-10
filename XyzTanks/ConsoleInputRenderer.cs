namespace XyzTanks;
internal partial class ConsoleInputReader : IInputReader
{
    public event EventHandler<InputEventArgs> InputActionCalled = null!;

    public void Update()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo? readKeyInfo = null;

            while (Console.KeyAvailable)
            {
                readKeyInfo = Console.ReadKey(true);
            }

            switch (readKeyInfo?.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Up));
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Down));
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Left));
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Right));
                    break;

                case ConsoleKey.Spacebar:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Fire));
                    break;

                case ConsoleKey.Escape:
                    InputActionCalled?.Invoke(this, new InputEventArgs(InputAction.Exit));
                    break;

                default:
                    break;
            }
        }
    }
}
