using TodoListApp;

public class Program
{
    public static void Main()
    {
        TodoList todoList = new TodoList();
        Profile profile = new Profile();

        Console.WriteLine("Добро пожаловать в TodoList! Введите 'help' для списка команд.");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            ICommand command = CommandParser.Parse(input, todoList, profile);
            command.Execute();

            if (command is ExitCommand)
                break;
        }

        Console.WriteLine("Пока!");
    }
}
