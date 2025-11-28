namespace TodoListApp;

public class InvalidCommand : ICommand
{
    private readonly string _message;

    public InvalidCommand(string message)
    {
        _message = message;
    }

    public void Execute()
    {
        Console.WriteLine(_message);
    }
}
