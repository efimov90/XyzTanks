namespace XyzTanks.Input;
internal interface IInputReader
{
    event EventHandler<InputEventArgs> InputActionCalled;

    void Update();
}
