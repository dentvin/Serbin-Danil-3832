namespace TodoListApp;

public class DoneCommand : ICommand
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

        item.MarkDone();
        Console.WriteLine("Задача отмечена выполненной.");
    }
}
