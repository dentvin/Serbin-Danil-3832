using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    const int INITIAL_CAPACITY = 2;

    static void Main()
    {
        string userFirstName = GetInput("Введите имя: ");
        string userLastName = GetInput("Введите фамилию: ");
        int userBirthYear = int.Parse(GetInput("Введите год рождения: "));

        string[] tasks = new string[INITIAL_CAPACITY];
        bool[] statuses = new bool[INITIAL_CAPACITY];
        DateTime[] dates = new DateTime[INITIAL_CAPACITY];
        int taskCount = 0;

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
                    ShowProfile(userFirstName, userLastName, userBirthYear);
                    break;
                case "add":
                    AddTask(command, ref tasks, ref statuses, ref dates, ref taskCount);
                    break;
                case "view":
                    ViewTasks(tasks, statuses, dates, taskCount, command);
                    break;
                case "read":
                    ReadTask(command, tasks, statuses, dates, taskCount);
                    break;
                case "done":
                    MarkTaskDone(command, statuses, dates, taskCount);
                    break;
                case "update":
                    UpdateTask(command, tasks, dates, taskCount);
                    break;
                case "delete":
                    DeleteTask(command, tasks, statuses, dates, ref taskCount);
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

    static void ShowProfile(string firstName, string lastName, int birthYear)
    {
        int age = DateTime.Now.Year - birthYear;
        Console.WriteLine($"{firstName} {lastName}, {birthYear} (возраст: {age})");
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

    static void AddTask(string command, ref string[] tasks, ref bool[] statuses, ref DateTime[] dates, ref int taskCount)
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
                string line = Console.ReadLine();
                if (line == null) continue;
                if (line.Trim() == "!end") break;
                if (sb.Length > 0) sb.Append("\n");
                sb.Append(line);
            }
            taskText = sb.ToString();
        }
        else
        {
            if (command.Length <= 4)
            {
                Console.WriteLine("Ошибка: текст задачи не указан.");
                return;
            }
            taskText = command.Substring(4).Trim();
        }

        EnsureCapacity(ref tasks, ref statuses, ref dates, taskCount);
        tasks[taskCount] = taskText;
        statuses[taskCount] = false;
        dates[taskCount] = DateTime.Now;
        taskCount++;
        Console.WriteLine("Задача добавлена!");
    }

    static void EnsureCapacity(ref string[] tasks, ref bool[] statuses, ref DateTime[] dates, int taskCount)
    {
        if (taskCount >= tasks.Length)
        {
            int newSize = tasks.Length * 2;
            string[] newTasks = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];

            for (int i = 0; i < tasks.Length; i++)
            {
                newTasks[i] = tasks[i];
                newStatuses[i] = statuses[i];
                newDates[i] = dates[i];
            }

            tasks = newTasks;
            statuses = newStatuses;
            dates = newDates;
        }
    }

    static void ViewTasks(string[] tasks, bool[] statuses, DateTime[] dates, int taskCount, string command)
    {
        var flags = ParseFlags(command);
        bool showIndex = flags.Contains("index") || flags.Contains("i") || flags.Contains("all") || flags.Contains("a");
        bool showStatus = flags.Contains("status") || flags.Contains("s") || flags.Contains("all") || flags.Contains("a");
        bool showDate = flags.Contains("update-date") || flags.Contains("d") || flags.Contains("all") || flags.Contains("a");

        if (taskCount == 0)
        {
            Console.WriteLine("Список задач пуст.");
            return;
        }

        if (showIndex) Console.Write("| №  ");
        Console.Write("| Task Text                      ");
        if (showStatus) Console.Write("| Status ");
        if (showDate) Console.Write("| Updated Date        ");
        Console.WriteLine("|");

        for (int i = 0; i < taskCount; i++)
        {
            if (showIndex) Console.Write($"| {i + 1,-3}");
            string text = tasks[i].Length > 30 ? tasks[i].Substring(0, 30) + "..." : tasks[i].PadRight(30);
            Console.Write($"| {text}");
            if (showStatus)
            {
                string status = statuses[i] ? "Done" : "Not done";
                Console.Write($"| {status,-7}");
            }
            if (showDate) Console.Write($"| {dates[i]:dd.MM.yyyy HH:mm}");
            Console.WriteLine("|");
        }
    }

    static void ReadTask(string command, string[] tasks, bool[] statuses, DateTime[] dates, int taskCount)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат read <номер>");
            return;
        }

        index--;
        if (index < 0 || index >= taskCount)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        Console.WriteLine($"Task: {tasks[index]}");
        Console.WriteLine($"Status: {(statuses[index] ? "Done" : "Not done")}");
        Console.WriteLine($"Updated: {dates[index]:dd.MM.yyyy HH:mm}");
    }

    static void MarkTaskDone(string command, bool[] statuses, DateTime[] dates, int taskCount)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: укажите номер задачи, например: done 2");
            return;
        }

        index--;
        if (index < 0 || index >= taskCount)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        statuses[index] = true;
        dates[index] = DateTime.Now;

        Console.WriteLine($"Задача №{index + 1} отмечена как выполненная!");
    }

    static void UpdateTask(string command, string[] tasks, DateTime[] dates, int taskCount)
    {
        string[] parts = command.Split(' ', 3);
        if (parts.Length < 3 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат update <номер> \"новый текст\"");
            return;
        }

        index--;
        if (index < 0 || index >= taskCount)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        string newText = parts[2].Trim();
        if (string.IsNullOrEmpty(newText))
        {
            Console.WriteLine("Ошибка: текст задачи не может быть пустым.");
            return;
        }

        tasks[index] = newText;
        dates[index] = DateTime.Now;

        Console.WriteLine($"Задача №{index + 1} обновлена!");
    }

    static void DeleteTask(string command, string[] tasks, bool[] statuses, DateTime[] dates, ref int taskCount)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: используйте формат delete <номер>");
            return;
        }

        index--;
        if (index < 0 || index >= taskCount)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        for (int i = index; i < taskCount - 1; i++)
        {
            tasks[i] = tasks[i + 1];
            statuses[i] = statuses[i + 1];
            dates[i] = dates[i + 1];
        }

        taskCount--;
        Console.WriteLine($"Задача №{index + 1} удалена!");
    }
}
