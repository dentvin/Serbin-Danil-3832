using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        string firstName = GetInput("Введите имя: ");
        string lastName = GetInput("Введите фамилию: ");
        int birthYear;
        while (!int.TryParse(GetInput("Введите год рождения: "), out birthYear))
        {
            Console.WriteLine("Ошибка: введите корректный год.");
        }

        var profile = new Profile(firstName, lastName, birthYear);
        var todoList = new TodoList();

        while (true)
        {
            string command = GetInput("\nВведите команду (help для списка команд): ");
            if (string.IsNullOrWhiteSpace(command))
                continue;

            string cmd = command.Split(' ')[0].ToLower();

            switch (cmd)
            {
                case "help":
                    ShowHelp();
                    break;
                case "profile":
                    Console.WriteLine(profile.GetInfo());
                    break;
                case "add":
                    AddCommand(command, todoList);
                    break;
                case "view":
                    ViewCommand(command, todoList);
                    break;
                case "read":
                    ReadCommand(command, todoList);
                    break;
                case "done":
                    DoneCommand(command, todoList);
                    break;
                case "update":
                    UpdateCommand(command, todoList);
                    break;
                case "delete":
                    DeleteCommand(command, todoList);
                    break;
                case "exit":
                    Console.WriteLine("Программа завершена.");
                    return;
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }
    }

    static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }

    static void ShowHelp()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("help — показать список команд");
        Console.WriteLine("profile — показать данные пользователя");
        Console.WriteLine("add \"текст задачи\" — добавить новую задачу");
        Console.WriteLine("add --multiline / -m — добавить многострочную задачу");
        Console.WriteLine("view — показать все задачи");
        Console.WriteLine("view с флагами: -i, -s, -d, -a");
        Console.WriteLine("read <номер> — показать полную задачу");
        Console.WriteLine("done <номер> — отметить задачу выполненной");
        Console.WriteLine("update <номер> \"новый текст\" — обновить текст задачи");
        Console.WriteLine("delete <номер> — удалить задачу");
        Console.WriteLine("exit — выйти из программы");
    }

    static List<string> ParseFlags(string command)
    {
        var flags = new List<string>();
        string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (part.StartsWith("--"))
                flags.Add(part.Substring(2).ToLower());
            else if (part.StartsWith("-") && part.Length > 1)
            {
                for (int i = 1; i < part.Length; i++)
                    flags.Add(part[i].ToString().ToLower());
            }
        }
        return flags;
    }

    static void AddCommand(string command, TodoList todoList)
    {
        var flags = ParseFlags(command);
        bool multiline = flags.Contains("multiline") || flags.Contains("m");

        string taskText;

        if (multiline)
        {
            Console.WriteLine("Введите строки задачи. Для окончания введите !end");
            var sb = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine() ?? "";
                if (line.Trim() == "!end") break;
                if (sb.Length > 0) sb.Append("\n");
                sb.Append(line);
            }
            taskText = sb.ToString();
        }
        else
        {
            string[] parts = command.Split(' ', 2);
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            {
                Console.WriteLine("Ошибка: текст задачи не указан.");
                return;
            }
            taskText = parts[1].Trim();
        }

        todoList.Add(new TodoItem(taskText));
        Console.WriteLine("Задача добавлена!");
    }

    static void ViewCommand(string command, TodoList todoList)
    {
        var flags = ParseFlags(command);
        bool showIndex = flags.Contains("i") || flags.Contains("index") || flags.Contains("a") || flags.Contains("all");
        bool showDone = flags.Contains("s") || flags.Contains("status") || flags.Contains("a") || flags.Contains("all");
        bool showDate = flags.Contains("d") || flags.Contains("update-date") || flags.Contains("a") || flags.Contains("all");

        todoList.View(showIndex, showDone, showDate);
    }

    static void ReadCommand(string command, TodoList todoList)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат read <номер>");
            return;
        }

        todoList.Read(index - 1);
    }

    static void DoneCommand(string command, TodoList todoList)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат done <номер>");
            return;
        }

        var item = todoList.GetItem(index - 1);
        if (item == null)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        item.MarkDone();
        Console.WriteLine($"Задача №{index} отмечена как выполненная!");
    }

    static void UpdateCommand(string command, TodoList todoList)
    {
        string[] parts = command.Split(' ', 3);
        if (parts.Length < 3 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат update <номер> \"новый текст\"");
            return;
        }

        string newText = parts[2].Trim();
        if (string.IsNullOrEmpty(newText))
        {
            Console.WriteLine("Ошибка: текст задачи не может быть пустым.");
            return;
        }

        var item = todoList.GetItem(index - 1);
        if (item == null)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        item.UpdateText(newText);
        Console.WriteLine($"Задача №{index} обновлена!");
    }

    static void DeleteCommand(string command, TodoList todoList)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат delete <номер>");
            return;
        }

        todoList.Delete(index - 1);
    }
}
