using System;

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
                    ViewTasks(tasks, statuses, dates, taskCount);
                    break;
                case "done":
                    MarkTaskDone(command, ref statuses, ref dates, taskCount);
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
        return Console.ReadLine();
    }

    static void ShowHelp()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("help — показать список команд");
        Console.WriteLine("profile — показать данные пользователя");
        Console.WriteLine("add \"текст задачи\" — добавить новую задачу");
        Console.WriteLine("view — показать все задачи");
        Console.WriteLine("done <номер> — отметить задачу выполненной");
        Console.WriteLine("exit — выйти из программы");
    }

    static void ShowProfile(string firstName, string lastName, int birthYear)
    {
        Console.WriteLine($"{firstName} {lastName}, {birthYear}");
    }

    static void AddTask(string command, ref string[] tasks, ref bool[] statuses, ref DateTime[] dates, ref int taskCount)
    {
        if (command.Length <= 4)
        {
            Console.WriteLine("Ошибка: текст задачи не указан.");
            return;
        }

        string taskText = command.Substring(4).Trim();

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

    static void ViewTasks(string[] tasks, bool[] statuses, DateTime[] dates, int taskCount)
    {
        Console.WriteLine("Список задач:");
        for (int i = 0; i < taskCount; i++)
        {
            string status = statuses[i] ? "сделано" : "не сделано";
            Console.WriteLine($"{i + 1}. {tasks[i]} [{status}] {dates[i]}");
        }
    }

    static void MarkTaskDone(string command, ref bool[] statuses, ref DateTime[] dates, int taskCount)
    {
        string[] parts = command.Split(' ', 2);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int index))
        {
            Console.WriteLine("Ошибка: укажите номер задачи, например: done 2");
            return;
        }

        index--; // пользователь вводит 1, а индекс в массиве начинается с 0

        if (index < 0 || index >= taskCount)
        {
            Console.WriteLine("Ошибка: задачи с таким номером не существует.");
            return;
        }

        statuses[index] = true;
        dates[index] = DateTime.Now;

        Console.WriteLine($"Задача №{index + 1} отмечена как выполненная!");
    }
}
