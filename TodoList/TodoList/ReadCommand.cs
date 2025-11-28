namespace TodoListApp;

public class ReadCommand : ICommand
{
    public int Index { get; set; }
    public TodoList TodoList { get; set; }

    public void Execute()
    {
        var item = TodoList.GetItem(Index - 1);
        if (item == null)
        {
            Console.WriteLine("Нет задачи с таким номером.");
            return;
        }

        Console.WriteLine(item.GetFullInfo());
    }
}
