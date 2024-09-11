namespace XyzTanks.Input;
public class InputEventArgs : EventArgs
{
    public InputEventArgs(InputAction inputAction)
    {
        InputAction = inputAction;
    }

    public InputAction InputAction { get; }
}
