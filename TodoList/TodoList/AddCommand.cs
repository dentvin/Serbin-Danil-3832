namespace TodoListApp;

public class AddCommand : ICommand
{
    public string Text { get; set; } = "";
    public bool Multiline { get; set; }
    public TodoList TodoList { get; set; }

    public void Execute()
    {
        if (Multiline)
        {
            Console.WriteLine("Введите многострочный текст. Пустая строка завершает ввод:");

            string full = "";
            while (true)
            {
                string? line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                full += line + "\n";
            }

            Text = full.Trim();
        }

        TodoList.Add(new TodoItem(Text));
        Console.WriteLine("Задача добавлена!");
    }
}
