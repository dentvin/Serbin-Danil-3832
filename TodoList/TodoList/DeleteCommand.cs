namespace TodoListApp;

public class DeleteCommand : ICommand
{
    public int Index { get; set; }
    public TodoList TodoList { get; set; }

    public void Execute()
    {
        TodoList.Delete(Index - 1);
    }
}
