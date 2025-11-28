namespace TodoListApp;

public class UpdateCommand : ICommand
{
    public int Index { get; set; }
    public string NewText { get; set; }
    public TodoList TodoList { get; set; }

    public void Execute()
    {
        var item = TodoList.GetItem(Index - 1);
        if (item == null)
        {
            Console.WriteLine("Нет задачи с таким номером.");
            return;
        }

        item.UpdateText(NewText);
        Console.WriteLine("Задача обновлена.");
    }
}
