namespace TodoListApp;

public static class CommandParser
{
    public static ICommand Parse(string input, TodoList todo, Profile profile)
    {
        string[] parts = input.Split(' ', 2);
        string cmd = parts[0].ToLower();

        switch (cmd)
        {
            case "add":
                var a = new AddCommand { TodoList = todo };
                if (input.Contains("--multiline") || input.Contains("-m"))
                    a.Multiline = true;
                else if (parts.Length > 1)
                    a.Text = parts[1];
                return a;

            case "view":
                var v = new ViewCommand { TodoList = todo };
                v.ShowIndex = input.Contains("-i") || input.Contains("-a");
                v.ShowStatus = input.Contains("-s") || input.Contains("-a");
                v.ShowDate = input.Contains("-d") || input.Contains("-a");
                return v;

            case "read":
                return new ReadCommand
                {
                    TodoList = todo,
                    Index = int.Parse(parts[1])
                };

            case "done":
                return new DoneCommand
                {
                    TodoList = todo,
                    Index = int.Parse(parts[1])
                };

            case "delete":
                return new DeleteCommand
                {
                    TodoList = todo,
                    Index = int.Parse(parts[1])
                };

            case "update":
                string[] p = parts[1].Split(' ', 2);
                return new UpdateCommand
                {
                    TodoList = todo,
                    Index = int.Parse(p[0]),
                    NewText = p[1]
                };

            default:
                Console.WriteLine("Неизвестная команда.");
                return new ViewCommand { TodoList = todo };
        }
    }
}
